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

public class ImageGodotBackend : IImageBackend<Color> // Rich
{
    public static Image CreateImage(int width, int height)
    {
        var image = new Image();
        image.Create(width, height, false, Image.Format.Rgba8);
        return image;
    }

    IImage<Color> IImageBackendWeak<Color>.CreateImage(int width, int height)
    {
        var image = CreateImage(width, height);
        image.Lock();
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
                imageError = image.LoadPngFromBuffer(data); // Breaking in method containing those functions will break debugger for some reason.
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
            default:
                throw new ArgumentException($"Unsupported image format: {type}");
        }
        // GD.Print(imageError); // Since the bug of debugger, we may need this output to determine error.  
        return image;
    }

    IImage<Color> IImageBackend<Color>.Decode(byte[] data, string type)
    {
        var image = Decode(data, type);
        image.Lock();
        return new ImageGodotProxy(){image=image};
    }

    public Color CreateColor(byte r, byte g, byte b, byte a) => Color.Color8(r, g, b, a);

    // public static int F2I(float x) => (int)(x * 255); // or (int)Mathf.Round(x * 255); ?

    // public static int[] EncodeColor(Color c) => new int[]{F2I(c.r), F2I(c.g), F2I(c.b), F2I(c.a)};
    public static int[] EncodeColor(Color c) => new int[]{c.r8, c.g8, c.b8, c.a8};
    int[] IImageBackendWeak<Color>.EncodeColor(Color c) => EncodeColor(c);

    public static string StringfyColor(Color c) => $"{c.r8},{c.g8},{c.b8},{c.a8}";
}
