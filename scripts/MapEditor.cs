using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Region : YYZ.MapKit.Region, YYZ.MapKit.IRegion<Region>
{
    public new HashSet<Region> neighbors{get; set;}
    public Vector2 scale{get; set;}

    public string name = "";
    public string id = "";
    public SideData side;
}

public class MapData : YYZ.MapKit.MapDataCore<Region>
{
    public MapData(Image baseImage, Dictionary<Color, Region> areaMap) : base(baseImage)
    {
        this.areaMap = areaMap;
    }
    public MapData(Image baseImage, Dictionary<Color, Area<Color>> areaMap) : this(baseImage, ConvertAreaMap(baseImage, areaMap)) {}

    static Dictionary<Color, Region> ConvertAreaMap(Image baseImage, Dictionary<Color, Area<Color>> areaMap)
    {
        var offset = new Vector2(baseImage.GetWidth() / 2, baseImage.GetHeight() / 2); // Used to union world and map coordinates.

        var retMap = new Dictionary<Color, Region>();
        var remap = new Dictionary<Area<Color>, Region>();
        foreach(var KV in areaMap)
        {
            var a = KV.Value;
            retMap[KV.Key] = remap[KV.Value] = new Region(){
                baseColor=a.BaseColor, remapColor=a.RemapColor, 
                center=new Vector2(a.X, a.Y) - offset, scale = new Vector2(a.SX, a.SY),
                area=a.Points,
                neighbors = new HashSet<Region>()
            };
        }
        foreach(var a in areaMap.Values)
        {
            var r = remap[a];
            foreach(var na in a.Neighbors)
                r.neighbors.Add(remap[na]);
        }
        return retMap;
    }

    public Dictionary<Color, Region> ExtractRegionMap() => areaMap;
}

public class MapEditor : Control
{
    [Export] PackedScene mapViewScene;
    [Export] NodePath selectGeneralPath;
    [Export] NodePath regionInfoWindowPath;
    [Export] NodePath regionEditDialogPath;
    [Export] NodePath sideButtonPath;
    [Export] NodePath labelModeBoxPath;
    [Export] NodePath exportMenuButtonPath;
    [Export] NodePath saveFileGeneralPath;
    [Export] NodePath openFileGeneralPath;
    [Export] NodePath importJsonButtonPath;
    [Export] Color nullColor = new Color(1,1,1,1);

    RegionInfoWindow regionInfoWindow;
    RegionEditDialog regionEditDialog;
    RegionEdit regionEdit;
    SideCardContainer sideCardContainer;
    SaveFileGeneral saveFileGeneral;
    OpenFileGeneral openFileGeneral;

    // volatile UI
    MapView mapView;
    MapShower mapShower;

    // volatile state
    List<Region> regionList;
    List<SideData> _sideDataList;
    List<SideData> sideDataList
    {
        get => _sideDataList;
        set
        {
            _sideDataList = value;

            // bind data
            regionEdit.BindSideDataList(value);
            sideCardContainer.BindData(value);
        }
    }
    Image image;
    Image remapImage;

    public override void _Ready()
    {
        var selectGeneral = (SelectGeneral)GetNode(selectGeneralPath);
        
        regionInfoWindow = (RegionInfoWindow)GetNode(regionInfoWindowPath);
        regionEditDialog = (RegionEditDialog)GetNode(regionEditDialogPath);
        regionEdit = regionEditDialog.regionEdit;

        var sideButton = (SideButton)GetNode(sideButtonPath);
        sideCardContainer = sideButton.sideCardContainer;

        var labelModeBox = (LabelModeBox)GetNode(labelModeBoxPath);
        saveFileGeneral = (SaveFileGeneral)GetNode(saveFileGeneralPath);
        openFileGeneral = (OpenFileGeneral)GetNode(openFileGeneralPath);

        var exportMenuButton = (MenuButton)GetNode(exportMenuButtonPath);
        var importJsonButton = (Button)GetNode(importJsonButtonPath);

        // test placeholder
        sideDataList = new List<SideData>(){ 
            new SideData(){id="french", name="French", color = new Color(0,0,1)},
            new SideData(){id="alliance", name="Alliance", color = new Color(1,0,0)}
        };

        // bind events
        selectGeneral.selected += OnSelectGeneralSelected;

        // labelModeBox.labelModeUpdated += mapView.OnLabelModeChanged; // FIXME: Value does not fall within the expected range.???
        labelModeBox.labelModeUpdated += OnLabelModeUpdated;

        sideCardContainer.sideDataIdUpdated += regionEdit.OnSideDataIdUpdated; // TODO: sideCard or sideCardList invoke the event directly.
        sideCardContainer.dataListStructureUpdated += regionEdit.OnSideDataListStructureUpdated;
        sideCardContainer.sideDataColorUpdated += OnSideColorChanged;
        sideCardContainer.sideDeleted += OnSideDeleted;

        regionEdit.regionSideUpdated += OnRegionSideChanged;
        // regionEdit.regionIdUpdated += mapView.OnRegionTextChanged; // FIXME: Value does not fall within the expected range.???
        // regionEdit.regionNameUpdated += mapView.OnRegionTextChanged;
        regionEdit.regionIdUpdated += OnRegionTextChanged;
        regionEdit.regionNameUpdated += OnRegionTextChanged;

        openFileGeneral.readCompleted += OnOpenFileGeneralReadCompleted;

        exportMenuButton.GetPopup().Connect("id_pressed", this, nameof(OnExportMenuPopupPressed));
        importJsonButton.Connect("pressed", this, nameof(OnImportJsonButtonPressed));

        // load first premade map
        selectGeneral.Select(0);

        // test mapShower
        /*
        foreach(var region in regionMap.Values)
        {
            var regionInfo = mapShower.GetAreaInfo(region);
            regionInfo.foregroundColor = new Color(1,0,0,1);
        }
        mapShower.Flush(); // TODO: maybe we should let mapShower itself flush itself if in every frame change committed.
        */
    }

    void OnOpenFileGeneralReadCompleted(object sender, TypedData typedData)
    {
        // At this point, this.openFileGeneral is used by JSON importer exclusively (base image is loaded by another widget and its private openFileGeneral)
        var jsonString = System.Text.Encoding.UTF8.GetString(typedData.data);
        JsonImporter.FromJsonString(jsonString, out var sideDataUpdateList, out var regionUpdateList);
        // GD.Print($"sideDataList.Count={sideDataList.Count}, regionList.Count={regionList.Count}");

        foreach(var match in FuzzyMatcher.Match(regionList, regionUpdateList))
        {
            var region = match.Item1;
            var regionUpdate = match.Item2;
            
            region.side = regionUpdate.side;
            region.id = regionUpdate.id;
            region.name = regionUpdate.name;
            // Other attributes are derived from the map directly and should be respected.
        }
        mapView.SyncLabels();

        sideDataList = sideDataUpdateList; // placeholder for other nuisance process.

        // sync UI states. (events are used for "minor" update only.)
        foreach(var region in regionList)
            {
                var regionInfo = mapShower.GetAreaInfo(region);
                regionInfo.foregroundColor = GetColorFor(region);
            }
        
        mapShower.Flush();
    }

    void OnImportJsonButtonPressed()
    {
        var _ = openFileGeneral.StartRead(OpenFileGeneral.Accept.json);
    }

    void OnExportMenuPopupPressed(int idx)
    {
        // Layout:
        // Export JSON
        // Export Base Texture
        // Export RemapTexture
        GD.Print($"OnExportMenuPopupPressed: {idx}");
        switch(idx)
        {
            case 0: // Export JSON
                ExportJSON();
                break;
            case 1: // Export Base Texture
                ExportBaseTexture();
                break;
            case 2: // Export RemapTexture
                ExportRemapTexture();
                break;
        }
    }

    void ExportJSON()
    {
        var jsonString = JsonExporter.ToString(sideDataList, regionList);
        var name = "mapdata.json";
        var data = System.Text.Encoding.UTF8.GetBytes(jsonString);
        saveFileGeneral.StartSave(data, name);
    }

    void ExportBaseTexture()
    {
        /*
        if(image == null)
            return;
        */

        var data = image.SavePngToBuffer();
        saveFileGeneral.StartSave(data, "base_texture.png");
    }

    void ExportRemapTexture()
    {
        /*
        if(remapImage == null)
            return;
        */
        
        var data = remapImage.SavePngToBuffer();
        saveFileGeneral.StartSave(data, "remap_texture.png");
    }

    void OnRegionTextChanged(object sender, Region region)
    {
        mapView.OnRegionTextChanged(this, region);
    }
    
    void OnLabelModeUpdated(object sender, LabelMode labelMode)
    {
        GD.Print($"OnLabelModeUpdated: {labelMode}");

        mapView.OnLabelModeChanged(this, labelMode);
    }

    void OnSelectGeneralSelected(object sender, TypedData imageData)
    {
        if(mapView != null)
        {
            mapView.QueueFree();
            mapView = null;
        }

        image = ImageGodotBackend.Decode(imageData.data, imageData.type);
        image.Lock();
        CreateMapView(image);
    }

    void OnAreaSelected(object sender, Region region)
    {
        regionInfoWindow.SetData(region);
    }

    void OnAreaClicked(object sender, Region region)
    {
        // https://www.reddit.com/r/godot/comments/fps3e4/world_coordinates_from_control_node_on_canvaslayer/
        // https://docs.godotengine.org/en/3.2/tutorials/2d/2d_transforms.html
        // https://docs.godotengine.org/en/3.2/tutorials/2d/canvas_layers.html

        regionEdit.BindRegion(region);
        regionEdit.PreparePopup();

        // var rect2 = new Rect2(GetViewport().GetMousePosition(), regionEditDialog.regionEdit.RectSize);
        // var rect2 = new Rect2(GetViewport().GetMousePosition(), regionEditDialog.RectSize); 
        // GD.Print($"RectSize={regionEditDialog.RectSize}");
        var rect2 = new Rect2(GetViewport().GetMousePosition(), new Vector2(158, 88)); // TODO: Fix wrong rectSize in HTML5 export, hard code it here as a wordaround.
        regionEditDialog.Popup_(rect2);
    }

    void OnRegionSideChanged(object sender, Region region)
    {
        var regionInfo = mapShower.GetAreaInfo(region);
        /*
        if(region.side == null)
            regionInfo.foregroundColor = new Color(1,1,1);
        else
            regionInfo.foregroundColor = region.side.color;
        */
        regionInfo.foregroundColor = GetColorFor(region);
        
        mapShower.Flush();
    }

    void OnSideColorChanged(object sender, SideData side)
    {
        foreach(var region in regionList)
            if(region.side == side)
            {
                var regionInfo = mapShower.GetAreaInfo(region);
                regionInfo.foregroundColor = side.color;
            }
        mapShower.Flush();
    }

    void OnSideDeleted(object sender, SideData side)
    {
        foreach(var region in regionList)
            if(region.side == side)
            {
                region.side = null;
                var regionInfo = mapShower.GetAreaInfo(region);
                regionInfo.foregroundColor = nullColor;
            }
        
        mapShower.Flush();
    }

    Color GetColorFor(Region region) => region.side == null ? nullColor : region.side.color;

    void CreateMapView(Image initialImage)
    {
        var factory = new YYZ.MapKit.RegionMapFactory<YYZ.MapKit.RegionData, Region>();

        var backend = new ImageGodotBackend(); // trait?
        var result = PixelMapPreprocessor.Process(backend, new ImageGodotProxy(){image=initialImage});
        var mapData = new MapData(initialImage, result.areaMap);
        
        regionList = mapData.ExtractRegionMap().Values.ToList(); // TODO: Find a better place to do this.

        mapView = mapViewScene.Instance<MapView>();

        var baseTexture = new ImageTexture();
        baseTexture.CreateFromImage(initialImage, 0);
        // FIXME: disable filters

        mapShower = (MapShower)mapView.GetNode(mapView.mapShowerPath);
        mapShower.mapData = mapData;
        mapShower.Texture = baseTexture;

        var remapTexture = CreateRemapTexture(result.data);

        var material = (ShaderMaterial)mapShower.Material;
        material.SetShaderParam("base_texture", baseTexture);
        material.SetShaderParam("remap_texture", remapTexture);

        mapShower.areaSelectedEvent += OnAreaSelected;
        mapShower.areaClickEvent += OnAreaClicked;

        AddChild(mapView);

        // Create labels
        mapView.CreateLabels(regionList); // container is initialized after entering the tree.
    }

    Texture CreateRemapTexture(byte[] pngData) // debug shield
    {
        remapImage = new Image();
        remapImage.LoadPngFromBuffer(pngData);
        var remapTexture = new ImageTexture();
        remapTexture.CreateFromImage(remapImage, 0);
        // TODO: Provide a ToGodotImage API to remove the unnecessary encode & decode overhead when we're using ImageGodotBackend.
        return remapTexture;
    }
}
