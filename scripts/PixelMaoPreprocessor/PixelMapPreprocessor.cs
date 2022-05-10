using System;
// using System.IO;
using System.Collections.Generic;

public class Area<TC>
{
    public TC BaseColor;
    public TC RemapColor;
    public int Points;
    public float X;
    public float Y;
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

public interface IImageBackend<TC>
{
    IImage<TC> CreateImage(int width, int height);
    IImage<TC> Decode(byte[] data, string type);
    TC CreateColor(byte r, byte g, byte b, byte a);
    
}


public static class PixelMapPreprocessor<TC>
{
    public class Result
    {
        public byte[] data;
        public Dictionary<TC, Area<TC>> areaMap;
    }

    public static Result Process(IImageBackend<TC> backend, byte[] data, string type)
    {
        var img = backend.Decode(data, type);

        var width = img.Width;
        var height = img.Height;

        Console.WriteLine($"width:{width}, height:{height}");

        var idx = 0;
        var areaMap = new Dictionary<TC, Area<TC>>();

        var remapImg = backend.CreateImage(width, height);
        for(int y=0; y<height; y++)
            for(int x=0; x<width; x++)
            {
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
        
        foreach(var area in areaMap.Values)
        {
            area.X /= area.Points;
            area.Y /= area.Points;
        }

        if(idx > 256 * 256)
            throw new ArgumentException("The size of province color should be < 256*256");

        Console.WriteLine($"Area size: {areaMap.Count}");

        for(int y=0; y<height; y++)
            for(int x=0; x<width; x++)
            {
                var c1 = img[x, y];

                if(y < height-1)
                {
                    var c2 = img[x, y+1];
                    if(!c1.Equals(c2))
                        areaMap[c1].Connect(areaMap[c2]);
                }
                if(x < width - 1)
                {
                    var c3 = img[x+1, y];
                    if(!c1.Equals(c3))
                        areaMap[c1].Connect(areaMap[c3]);
                }
            }

        return new Result(){data = remapImg.ToPngBytes(), areaMap=areaMap};
    }
}