using System;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Area
{
    public Rgba32 BaseColor;
    public Rgba32 RemapColor;
    public int Points;
    public float X;
    public float Y;
    public HashSet<Area> Neighbors = new HashSet<Area>();
    public bool IsEdge;

    public void Connect(Area other)
    {
        Neighbors.Add(other);
        other.Neighbors.Add(this);
    }
}


static class PixelMapPreprocessor
{
    public class Result
    {
        public byte[] data;
        public Dictionary<Rgba32, Area> areaMap;
    }

    static byte[] ImageToPngBytes(Image image)
    {
        using (var ms = new MemoryStream())
        {
            image.SaveAsPng(ms);
            return ms.ToArray();
        }
    }

    public static Result Process(byte[] data)
    {
        // var stopWatch = new System.Diagnostics.Stopwatch();
        // stopWatch.Restart();

        var img = Image.Load<Rgba32>(data); // ImageSharp can detect format from the binary only?..
        
        var width = img.Width;
        var height = img.Height;

        // stopWatch.Stop();

        Console.WriteLine($"width:{width}, height:{height}");

        // stopWatch.Restart();

        var idx = 0;
        var areaMap = new Dictionary<Rgba32, Area>();

        var remapImg = new Image<Rgba32>(width, height);
        for(int y=0; y<height; y++)
            for(int x=0; x<width; x++)
            {
                // var baseColor = img.GetPixel(x, y);
                var baseColor = img[x, y]; // TODO: https://docs.sixlabors.com/articles/imagesharp/pixelbuffers.html
                Area area; // C# 8 requires Nullable reference type
                if(!areaMap.TryGetValue(baseColor, out area))
                {
                    var low = (byte)(idx % 256);
                    var high = (byte)(idx / 256);
                    var remapColor = new Rgba32(low, high, 0, 255);
                    area = new Area(){BaseColor=baseColor, RemapColor=remapColor};
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
        {
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
        }

        return new Result(){data = ImageToPngBytes(remapImg), areaMap=areaMap};
    }
}