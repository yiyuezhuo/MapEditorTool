
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Godot;

public class RegionJsonData
{
    public int[] baseColor;
    public int[] remapColor;
    public int[] neighbors;
    public float[] center;
    public float[] scale;
    public float area;
    public string name;
    public string id;
    public int side;
}

public class SideJsonData
{
    public string id;
    public string name;
    public int[] color;
}

public class JsonData
{
    public RegionJsonData[] regions;
    public SideJsonData[] sides;
}

public static class JsonExporter
{
    public static JsonData ToJsonData(IEnumerable<SideData> sides, IEnumerable<Region> regions)
    {
        var sideMap = new Dictionary<SideData, int>();
        var regionMap = new Dictionary<Region, int>();

        var idx = 0;
        foreach(var side in sides)
        {
            sideMap[side] = idx;
            idx++;
        }
        idx = 0;
        foreach(var region in regions)
        {
            regionMap[region] = idx;
            idx++;
        }

        var sideJsonDataIter = sideMap.Keys.Select(side => new SideJsonData()
        {
            id=side.id, name=side.name, color=Encode(side.color)
        });
        var regionJsonDataIter = regionMap.Keys.Select(region => new RegionJsonData()
        {
            baseColor = Encode(region.baseColor), remapColor = Encode(region.remapColor),
            neighbors = region.neighbors.Select(r => regionMap[r]).ToArray(),
            center = Encode(region.center), scale = Encode(region.scale), area=region.area,
            name = region.name, id = region.id,
            side = region.side == null ? -1 : sideMap[region.side]
        });

        return new JsonData(){regions = regionJsonDataIter.ToArray(), sides=sideJsonDataIter.ToArray()};
    }

    public static string ToString(IEnumerable<SideData> sides, IEnumerable<Region> regions)
    {
        var jsonData = ToJsonData(sides, regions);
        return JsonConvert.SerializeObject(jsonData);
    }

    public static int[] Encode(Color color) => ImageGodotBackend.EncodeColor(color);
    public static float[] Encode(Vector2 x) => new float[]{x.x, x.y};
}

public static class JsonImporter
{
    public static void FromJsonData(JsonData data, out List<SideData> sideDataList, out List<Region> regionList)
    {
        sideDataList = new List<SideData>();
        regionList = new List<Region>();

        var _sideDataList = sideDataList = data.sides.Select(sideJsonData => new SideData(){
            id=sideJsonData.id, name=sideJsonData.name,
            color=ToColor(sideJsonData.color)
        }).ToList();

        GD.Print($"_sideDataList={_sideDataList.Count}");

        var _regionList = regionList = data.regions.Select(regionJsonData => new Region(){
            // neighbors = new HashSet<Region>(),
            side = regionJsonData.side == -1 ? null : _sideDataList[regionJsonData.side],
            name = regionJsonData.name, id=regionJsonData.id,
            baseColor = ToColor(regionJsonData.baseColor), remapColor = ToColor(regionJsonData.remapColor),
            center = ToVector2(regionJsonData.center), scale = ToVector2(regionJsonData.scale), area=regionJsonData.area
        }).ToList();

        GD.Print($"_regionList={_regionList.Count}");

        for(var i=0; i<regionList.Count; i++)
        {
            var region = regionList[i];
            var regionJsonData = data.regions[i];
            region.neighbors = regionJsonData.neighbors.Select(idx => _regionList[idx]).ToHashSet();
        }
    }

    public static void FromJsonString(string jsonString, out List<SideData> sideDataList, out List<Region> regionList)
    {
        var jsonData = JsonConvert.DeserializeObject<JsonData>(jsonString);
        FromJsonData(jsonData, out sideDataList, out regionList);
    }

    static Color ToColor(int[] x) => Color.Color8((byte)x[0], (byte)x[1], (byte)x[2], (byte)x[3]);
    static Vector2 ToVector2(float[] x) => new Vector2(x[0], x[1]);
}