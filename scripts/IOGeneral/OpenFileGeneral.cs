using Godot;
using System;
using System.Threading.Tasks;

public class OpenFileGeneral : IOFileGeneral
{
    
    // [Export] bool load = true;

    TaskCompletionSource<TypedData> tcs = null;

    public static class Accept
    {
        public static string image = "image/png, image/jpeg, image/webp";
        public static string json = "application/JSON";
    }

    public event EventHandler<TypedData> readCompleted;
    // public event EventHandler<Image> loadCompleted;

    /*
    public override void _Ready()
    {
        base._Ready();
    }
    */

    async Task<TypedData> GetTypedData(string accept)
    {
        TypedData imageData;
        if(IsHTML5())
        {
            imageData = await html5file.ReadDataAsync(accept); // type = "image/jpg", "image/png", ..., "text/json"
            imageData.type = imageData.type.Replace("image/", "");
        }
        else
        {
            tcs = new TaskCompletionSource<TypedData>();
            fileDialog.Popup_();
            imageData = await tcs.Task; // type = "jpg", "png", ...
        }
        return imageData; // type = "jpg", "png", ...
    }

    public async Task StartRead(string accept)
    {
        GD.Print("OnPressed");

        var imageData = await GetTypedData(accept);

        readCompleted?.Invoke(this, imageData);

        /*
        if(!load)
            return;

        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);

        loadCompleted?.Invoke(this, image);
        */
    }

    /*
    public async Task ReadJsonAsync()
    {
        GD.Print("ReadJsonAsync")

        var jsonData = await GetJson
    }
    */

    protected override void OnFileDialogFileSelected(string path)
    {
        GD.Print($"OnFileDialogFileSelected: {path}");

        var typedData = ReadDataFromPath(path);
        tcs.TrySetResult(typedData);
    }

    public static TypedData ReadDataFromPath(string path)
    {
        var bytes = ReadBytesFromPath(path);
        var type = System.IO.Path.GetExtension(path);
        type = type.Replace(".", ""); // Is it a solid normalization?
        return new TypedData{data=bytes, type=type};
    }

    public static byte[] ReadBytesFromPath(string path)
    {        
        var file = new File();
        var error = file.Open(path, File.ModeFlags.Read);

        if(error != Error.Ok)
            throw new ArgumentException($"Failed to open file: {error}");
        
        var bytes = file.GetBuffer((long)file.GetLen());
        file.Close();

        return bytes;
    }
}
