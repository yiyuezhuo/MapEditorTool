using System;
// using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class Area<TC>
{
    public TC BaseColor;
    public TC RemapColor;
    public int Points;
    public float X;
    public float Y;
    public float SX; // scale of X
    public float SY;
    public HashSet<Area<TC>> Neighbors = new HashSet<Area<TC>>();
    public bool IsEdge;

    public void Connect(Area<TC> other)
    {
        Neighbors.Add(other);
        other.Neighbors.Add(this);
    }
}


public interface IImage<TC>
{
    int Width{get;}
    int Height{get;}
    TC this[int x, int y]{get;set;}

    byte[] ToPngBytes();
}

public interface IImageBackendWeak<TC>
{
    IImage<TC> CreateImage(int width, int height);
    TC CreateColor(byte r, byte g, byte b, byte a);
    int[] EncodeColor(TC color);

}

public interface IImageBackend<TC> : IImageBackendWeak<TC>
{
    IImage<TC> Decode(byte[] data, string type);
}


public static class PixelMapPreprocessor
{
    public static Result<TC> Process<TC>(IImageBackendWeak<TC> backend, IImage<TC> img)
    {
        var width = img.Width;
        var height = img.Height;

        Console.WriteLine($"width:{width}, height:{height}");

        var idx = 0;
        var areaMap = new Dictionary<TC, Area<TC>>();

        var remapImg = backend.CreateImage(width, height);

        // first pass: create area objects and compute mean
        for(int y=0; y<height; y++)
        {
            // System.Console.WriteLine(y);
            for(int x=0; x<width; x++)
            {
                // System.Console.WriteLine(x);
                // var baseColor = img.GetPixel(x, y);
                var baseColor = img[x, y]; // TODO: https://docs.sixlabors.com/articles/imagesharp/pixelbuffers.html
                Area<TC> area; // C# 8 requires Nullable reference type
                if(!areaMap.TryGetValue(baseColor, out area))
                {
                    var low = (byte)(idx % 256);
                    var high = (byte)(idx / 256);
                    var remapColor = backend.CreateColor(low, high, 0, 255);
                    // var remapColor = new Rgba32(low, high, 0, 255);
                    area = new Area<TC>(){BaseColor=baseColor, RemapColor=remapColor};
                    areaMap[baseColor] = area;
                    idx += 1;
                }
                // remapImg.SetPixel(x, y, area.RemapColor);
                remapImg[x, y] = area.RemapColor;
                area.Points += 1;
                area.X += x;
                area.Y += y;
                if(x == 0 || y == 0 || y == height - 1 || x == height - 1)
                    area.IsEdge = true;
            }
        }
        
        foreach(var area in areaMap.Values)
        {
            area.X /= area.Points;
            area.Y /= area.Points;
        }

        if(idx > 256 * 256)
            throw new ArgumentException("The size of province color should be < 256*256");

        Console.WriteLine($"Area size: {areaMap.Count}");

        // second pass: compute neighbors and variance
        for(int y=0; y<height; y++)
            for(int x=0; x<width; x++)
            {
                var c1 = img[x, y];
                var area = areaMap[c1];

                if(y < height-1)
                {
                    var c2 = img[x, y+1];
                    if(!c1.Equals(c2))
                        area.Connect(areaMap[c2]);
                }
                if(x < width - 1)
                {
                    var c3 = img[x+1, y];
                    if(!c1.Equals(c3))
                        area.Connect(areaMap[c3]);
                }
                var dx = area.X - x;
                var dy = area.Y - y;
                area.SX += dx * dx;
                area.SY += dy * dy;
            }

        foreach(var area in areaMap.Values)
        {
            area.SX = (float)Math.Sqrt(area.SX / area.Points); // biased standard deviation
            area.SY = (float)Math.Sqrt(area.SY / area.Points);
        }

        return new Result<TC>(){data = remapImg.ToPngBytes(), areaMap=areaMap};
    }

    public static Result<TC> Process<TC>(IImageBackend<TC> backend, byte[] data, string type)
    {
        var img = backend.Decode(data, type);
        return Process(backend, img);
    }

    public class Result<TC>
    {
        public byte[] data;
        public Dictionary<TC, Area<TC>> areaMap;
    }

    [Serializable]
    class AreaReduced<TC>
    {
        public int[] BaseColor;// = new int[4];
        public int[] RemapColor;// = new int[4];
        public int Points;
        public float X;
        public float Y;
        public List<int[]> Neighbors;
        public bool IsEdge;

        // static int[] EncodeColor(TC c) => new int[]{c.R, c.G, c.B, c.A};

        public AreaReduced(IImageBackend<TC> backend, Area<TC> area)
        {
            Neighbors = new List<int[]>();
            foreach(var neiArea in area.Neighbors)
                Neighbors.Add(backend.EncodeColor(neiArea.BaseColor));

            BaseColor = backend.EncodeColor(area.BaseColor);
            RemapColor = backend.EncodeColor(area.RemapColor);

            Points = area.Points;
            X = area.X;
            Y = area.Y;
            IsEdge = area.IsEdge;
        }
    }

    [Serializable]
    class JsonResult<TC>
    {
        public List<AreaReduced<TC>> Areas;// = new List<AreaReduced>();
        public JsonResult(List<AreaReduced<TC>> Areas) => this.Areas = Areas;
    }

    public static string ResultToJson<TC>(IImageBackend<TC> backend, Result<TC> result)
    {
        var reduceIter = from area in result.areaMap.Values select new AreaReduced<TC>(backend, area);
        var res = new JsonResult<TC>(reduceIter.ToList());
        var jsonString = JsonConvert.SerializeObject(res);
        // jsonString = JsonConvert.SerializeObject(res, Formatting.Indented); // Well, I would like a custom depth, though.
        return jsonString;
    }

}