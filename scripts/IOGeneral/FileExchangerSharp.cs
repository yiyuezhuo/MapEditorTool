using Godot;
using System.Threading.Tasks;

public class ImageData
{
    public byte[] data;
    public string type;
}

/// <summary>
/// A C# wrapper for HTML5FileExchange, I had tried to implement it in C# but callback didn't work for some reason.
/// For experiment purpose, this class would be added as a singeton as well rather than working as a static class.
/// </summary>
public class FileExchangerSharp : Node
{
    Node html5file;
    
    public override void _Ready()
    {
        html5file = GetNode("/root/HTML5File"); // A GDScript singleton node
    }

    public async Task<Image> LoadImageAsync()
    {
        html5file.Call("load_image");
        var signalArgs = await ToSignal(html5file, "load_completed");
        return (Image)signalArgs[0];
    }

    public async Task<ImageData> LoadDataAsync()
    {
        html5file.Call("read_data");
        var signalArgs = await ToSignal(html5file, "read_completed");
        return new ImageData(){data = (byte[])signalArgs[0], type = (string)signalArgs[1]};
    }

    public void SaveImage(Image image, string fileName)
    {
        html5file.Call("save_image", image, fileName);
    }

    public void SaveData(byte[] buffer, string fileName)
    {
        html5file.Call("save_data", buffer, fileName);
    }
}