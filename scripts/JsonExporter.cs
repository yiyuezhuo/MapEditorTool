
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
            center = Encode(region.center), area=region.area,
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
