using Godot;
using System.Threading.Tasks;

public class TypedData
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

    public async Task<TypedData> ReadDataAsync(string accept)
    {
        html5file.Call("read_data", accept);
        var signalArgs = await ToSignal(html5file, "read_completed");
        return new TypedData(){data = (byte[])signalArgs[0], type = (string)signalArgs[1]};
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