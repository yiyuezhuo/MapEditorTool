using Godot;
using System;

public class ImageGodotProxy : IImage<Color>
{
    public Image image;

    public int Width{get => image.GetWidth();}
    public int Height{get => image.GetHeight();}
    public Color this[int x, int y]{get => image.GetPixel(x, y); set => image.SetPixel(x, y, value);}

    public byte[] ToPngBytes() => image.SavePngToBuffer();
}

public class ImageGodotBackend : IImageBackend<Color>
{
    public IImage<Color> CreateImage(int width, int height)
    {
        var image = new Image();
        image.Create(width, height, false, Image.Format.Rgba8);
        return new ImageGodotProxy(){image=image};
        // image.Fill(colorInit);
    }

    public static Image Decode(byte[] data, string type)
    {
        
        var image = new Image();
        Error imageError;

        switch(type)
        {
            case "png":
            case "PNG":
                imageError = image.LoadPngFromBuffer(data);
                break;
            case "jpg":
            case "jpeg":
            case "JPG":
            case "JPEG":
                imageError = image.LoadJpgFromBuffer(data);
                break;
            case "webp":
            case "WEBP":
                imageError = image.LoadWebpFromBuffer(data);
                break;
        }
        return image;
    }

    IImage<Color> IImageBackend<Color>.Decode(byte[] data, string type)
    {
        var image = Decode(data, type);
        return new ImageGodotProxy(){image=image};
    }

    public Color CreateColor(byte r, byte g, byte b, byte a) => new Color(r, g, b, a);

    public PixelMapPreprocessor<Color>.Result Process(byte[] data, string type)
    {
        return PixelMapPreprocessor<Color>.Process(this, data, type);
    }
}
