using Godot;
using System.Threading.Tasks;

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
        return (Image)((await ToSignal(html5file, "load_completed"))[0]);
    }

    public void SaveImage(Image image, string fileName)
    {
        html5file.Call("save_image", image, fileName);
    }
}