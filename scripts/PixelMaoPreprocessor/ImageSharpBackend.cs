
/*
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

class ImageSharpProxy : IImage<Rgba32>
{
    public Image<Rgba32> image;

    public int Width{get => image.Width;}
    public int Height{get => image.Height;}
    public Rgba32 this[int x, int y]{get => image[x, y]; set => image[x,y]=value;}

    public byte[] ToPngBytes()
    {
        using (var ms = new MemoryStream())
        {
            image.SaveAsPng(ms);
            return ms.ToArray();
        }
    }
}

public class ImageSharpBackend : IImageBackend<Rgba32>
{
    public IImage<Rgba32> CreateImage(int width, int height)
    {
        // return Image.Load<Rgba32>(data);
        var image = new Image<Rgba32>(width, height);
        return new ImageSharpProxy(){image=image};
    }

    public IImage<Rgba32> Decode(byte[] data, string type)
    {
        var image = Image.Load<Rgba32>(data);
        return new ImageSharpProxy(){image=image};
    }

    public Rgba32 CreateColor(byte r, byte g, byte b, byte a)
    {
        return new Rgba32(r, g, b, a);
    }

    public int[] EncodeColor(Rgba32 c) => new int[]{c.R, c.G, c.B, c.A};
}
*/