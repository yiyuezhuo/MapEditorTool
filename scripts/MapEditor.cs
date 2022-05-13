using Godot;
using System;
using System.Collections.Generic;

public class Region : YYZ.MapKit.Region, YYZ.MapKit.IRegion<Region>
{
    public new HashSet<Region> neighbors{get; set;}
}

public class MapData : YYZ.MapKit.MapDataCore<Region>
{
    public MapData(Image baseImage, Dictionary<Color, Region> areaMap) : base(baseImage)
    {
        this.areaMap = areaMap;
    }
    public MapData(Image baseImage, Dictionary<Color, Area<Color>> areaMap) : this(baseImage, ConvertAreaMap(areaMap)) {}

    static Dictionary<Color, Region> ConvertAreaMap(Dictionary<Color, Area<Color>> areaMap)
    {
        var retMap = new Dictionary<Color, Region>();
        var remap = new Dictionary<Area<Color>, Region>();
        foreach(var KV in areaMap)
        {
            var a = KV.Value;
            retMap[KV.Key] = remap[KV.Value] = new Region(){
                baseColor=a.BaseColor, remapColor=a.RemapColor, center=new Vector2(a.X, a.Y),
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
}

/*
class MapDataRes : YYZ.MapKit.IMapDataRes<Region>
{
    MapData mapData;

    public MapDataRes(Image baseImage, Dictionary<Color, Region> areaMap)
    {
        mapData = new MapData(baseImage, areaMap);
    }

    public YYZ.MapKit.IMapData<Region> GetInstance() => mapData;
}
*/

public class MapEditor : Control
{
    [Export] PackedScene mapViewScene;
    [Export] NodePath selectGeneralPath;
    [Export] Resource initialImageRes;

    MapView mapView;
    MapShower mapShower;

    public override void _Ready()
    {
        var selectGeneral = (SelectGeneral)GetNode(selectGeneralPath);
        selectGeneral.selected += OnSelectGeneralSelected;

        selectGeneral.Select(0);

        /*
        var initialImage = (Image)initialImageRes;
        initialImage.Lock();
        CreateMapView(initialImage);
        */

        /*
        var factory = new YYZ.MapKit.RegionMapFactory<YYZ.MapKit.RegionData, Region>();

        var backend = new ImageGodotBackend(); // trait?
        var result = PixelMapPreprocessor.Process(backend, new ImageGodotProxy(){image=initialImage});
        var mapData = new MapData(initialImage, result.areaMap);

        mapView = mapViewScene.Instance<MapView>();

        var baseTexture = new ImageTexture();
        baseTexture.CreateFromImage(initialImage, 0);
        // FIXME: disable fitlers

        var remapImage = new Image();
        remapImage.LoadPngFromBuffer(result.data);
        var remapTexture = new ImageTexture();
        remapTexture.CreateFromImage(remapImage, 0);
        // TODO: Provide a ToGodotImage API to remove the unnecessary encode & decode overhead when we're using ImageGodotBackend.

        mapShower = (MapShower)mapView.GetNode(mapView.mapShowerPath);
        mapShower.mapData = mapData;
        mapShower.Texture = baseTexture;

        var material = (ShaderMaterial)mapShower.Material;
        material.SetShaderParam("base_texture", baseTexture);
        material.SetShaderParam("remap_texture", remapTexture);

        AddChild(mapView);
        */
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

    void CreateMapView(Image initialImage)
    {
        var factory = new YYZ.MapKit.RegionMapFactory<YYZ.MapKit.RegionData, Region>();

        var backend = new ImageGodotBackend(); // trait?
        var result = PixelMapPreprocessor.Process(backend, new ImageGodotProxy(){image=initialImage});
        var mapData = new MapData(initialImage, result.areaMap);

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

        AddChild(mapView);
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
