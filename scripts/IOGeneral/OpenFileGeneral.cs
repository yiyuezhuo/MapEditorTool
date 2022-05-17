using Godot;
using System;
using System.Threading.Tasks;

public class OpenFileGeneral : IOFileGeneral
{
    
    [Export] bool load = true;

    TaskCompletionSource<ImageData> tcs = null;

    public event EventHandler<ImageData> readCompleted;
    public event EventHandler<Image> loadCompleted;

    /*
    public override void _Ready()
    {
        base._Ready();
    }
    */

    async Task<ImageData> GetImageData()
    {
        ImageData imageData;
        if(IsHTML5())
        {
            imageData = await html5file.LoadDataAsync(); // type = "image/jpg", "image/png", ...
            imageData.type = imageData.type.Replace("image/", "");
        }
        else
        {
            tcs = new TaskCompletionSource<ImageData>();
            fileDialog.Popup_();
            imageData = await tcs.Task; // type = "jpg", "png", ...
        }
        return imageData; // type = "jpg", "png", ...
    }

    /*
    protected override void OnPressed()
    {
        var _ = OnPressedAsync(); // suppress a warning and an error of "Attempted to convert an unmarshallable managed type to Variant. Name: 'Task`1' Encoding: 21."
    }
    */

    public async Task OnPressedAsync()
    {
        GD.Print("OnPressed");

        var imageData = await GetImageData();

        readCompleted?.Invoke(this, imageData);

        if(!load)
            return;

        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);

        loadCompleted?.Invoke(this, image);
    }

    protected override void OnFileDialogFileSelected(string path)
    {
        GD.Print($"OnFileDialogFileSelected: {path}");

        var imageData = ReadDataFromPath(path);
        tcs.TrySetResult(imageData);
    }

    public static ImageData ReadDataFromPath(string path)
    {
        var bytes = ReadBytesFromPath(path);
        var type = System.IO.Path.GetExtension(path);
        type = type.Replace(".", ""); // Is it a solid normalization?
        return new ImageData{data=bytes, type=type};
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
