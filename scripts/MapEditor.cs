using Godot;
using System;
using System.Collections.Generic;

public class Region : YYZ.MapKit.Region, YYZ.MapKit.IRegion<Region>
{
    public new HashSet<Region> neighbors{get; set;}

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
                baseColor=a.BaseColor, remapColor=a.RemapColor, center=new Vector2(a.X, a.Y) - offset, area=a.Points,
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

    RegionInfoWindow regionInfoWindow;
    RegionEditDialog regionEditDialog;
    RegionEdit regionEdit;
    SideCardContainer sideCardContainer;
    // LabelModeBox labelModeBox;

    // volative UI
    MapView mapView;
    MapShower mapShower;

    // volative state
    Dictionary<Color, Region> regionMap; // TODO: Use a more proper object, but I don't have time to develop more in this Jam.

    public override void _Ready()
    {
        var selectGeneral = (SelectGeneral)GetNode(selectGeneralPath);
        selectGeneral.selected += OnSelectGeneralSelected;

        regionInfoWindow = (RegionInfoWindow)GetNode(regionInfoWindowPath);
        regionEditDialog = (RegionEditDialog)GetNode(regionEditDialogPath);
        regionEdit = regionEditDialog.regionEdit;

        var sideButton = (SideButton)GetNode(sideButtonPath);
        sideCardContainer = sideButton.sideCardContainer;

        var labelModeBox = (LabelModeBox)GetNode(labelModeBoxPath);

        // test placeholder
        var sideDataList = new List<SideData>(){ 
            new SideData(){id="french", name="French", color = new Color(0,0,1)},
            new SideData(){id="alliance", name="Alliance", color = new Color(1,0,0)}
        };

        // bind data
        regionEdit.BindSideDataList(sideDataList);
        sideCardContainer.BindData(sideDataList);

        // bind events
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

    void OnRegionTextChanged(object sender, Region region)
    {
        mapView.OnRegionTextChanged(this, region);
    }
    
    void OnLabelModeUpdated(object sender, LabelMode labelMode)
    {
        GD.Print($"OnLabelModeUpdated: {labelMode}");

        mapView.OnLabelModeChanged(this, labelMode);
    }

    void OnSelectGeneralSelected(object sender, ImageData imageData)
    {
        if(mapView != null)
        {
            mapView.QueueFree();
            mapView = null;
        }

        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);
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

        var rect2 = new Rect2(GetViewport().GetMousePosition(), regionEditDialog.RectSize);
        regionEditDialog.Popup_(rect2);
    }

    void OnRegionSideChanged(object sender, Region region)
    {
        var regionInfo = mapShower.GetAreaInfo(region);
        if(region.side == null)
            regionInfo.foregroundColor = new Color(1,1,1);
        else
            regionInfo.foregroundColor = region.side.color;
        
        mapShower.Flush();
    }

    void OnSideColorChanged(object sender, SideData side)
    {
        foreach(var region in regionMap.Values)
            if(region.side == side)
            {
                var regionInfo = mapShower.GetAreaInfo(region);
                regionInfo.foregroundColor = side.color;
            }
        mapShower.Flush();
    }

    void OnSideDeleted(object sender, SideData side)
    {
        foreach(var region in regionMap.Values)
            if(region.side == side)
            {
                region.side = null;
                var regionInfo = mapShower.GetAreaInfo(region);
                regionInfo.foregroundColor = new Color(1,1,1);
            }
        
        mapShower.Flush();
    }

    void CreateMapView(Image initialImage)
    {
        var factory = new YYZ.MapKit.RegionMapFactory<YYZ.MapKit.RegionData, Region>();

        var backend = new ImageGodotBackend(); // trait?
        var result = PixelMapPreprocessor.Process(backend, new ImageGodotProxy(){image=initialImage});
        var mapData = new MapData(initialImage, result.areaMap);
        
        regionMap = mapData.ExtractRegionMap(); // TODO: Find a better place to do this.

        mapView = mapViewScene.Instance<MapView>();

        var baseTexture = new ImageTexture();
        baseTexture.CreateFromImage(initialImage, 0);
        // FIXME: disable fitlers

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
        mapView.CreateLabels(regionMap.Values); // container is initialized after entering the tree.
    }

    void CreateLabelLayer()
    {

    }

    static Texture CreateRemapTexture(byte[] pngData) // debug shield
    {
        var remapImage = new Image();
        remapImage.LoadPngFromBuffer(pngData);
        var remapTexture = new ImageTexture();
        remapTexture.CreateFromImage(remapImage, 0);
        // TODO: Provide a ToGodotImage API to remove the unnecessary encode & decode overhead when we're using ImageGodotBackend.
        return remapTexture;
    }
}
