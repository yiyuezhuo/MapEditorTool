using Godot;
using System;
using System.Threading.Tasks;

public class OpenFileGeneral : Button
{
    [Export] NodePath fileDialogPath;
    [Export] bool load = true;

    FileDialog fileDialog;
    FileExchangerSharp html5file;

    TaskCompletionSource<ImageData> tcs = null;

    public event EventHandler<ImageData> readCompleted;
    public event EventHandler<Image> loadCompleted;

    public override void _Ready()
    {
        fileDialog = (FileDialog)GetNode(fileDialogPath);
        html5file = (FileExchangerSharp)GetNode("/root/FileExchangerSharp");

        //fileDialog.CurrentDir = );
        // fileDialog.CurrentDir = fileDialog.CurrentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        fileDialog.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

        Connect("pressed", this, nameof(OnPressed));
        fileDialog.Connect("confirmed", this, nameof(OnFileDialogConfirmed));
        fileDialog.Connect("file_selected", this, nameof(OnFileDialogFileSelected));
    }

    public bool IsHTML5() => OS.GetName() == "HTML5" && OS.HasFeature("JavaScript");

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
        // TODO: normalize type string here
        return imageData; // type = "jpg", "png", ...
    }

    void OnPressed()
    {
        var _ = OnPressedAsync(); // suppress a warning and an error of "Attempted to convert an unmarshallable managed type to Variant. Name: 'Task`1' Encoding: 21."
    }

    async Task OnPressedAsync()
    {
        GD.Print("OnPressed");

        // var result = PixelMapPreprocessor.Process(imageData);
        var imageData = await GetImageData();

        readCompleted?.Invoke(this, imageData);

        if(!load)
            return;

        // var image = Decode(imageData);
        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);

        loadCompleted?.Invoke(this, image);
    }

    void OnFileDialogConfirmed()
    {
        GD.Print("FileDialog Confirmed");
        ReadDataFromPath(fileDialog.CurrentPath);
    }

    void OnFileDialogFileSelected(string path)
    {
        GD.Print($"OnFileDialogFileSelected: {path}");
        var imageData = ReadDataFromPath(path);
        tcs.TrySetResult(imageData);
    }

    public static ImageData ReadDataFromPath(string path)
    {
        var bytes = ReadBytesFromPath(path);
        var type = System.IO.Path.GetExtension(path);
        type = type.Replace(".", ""); // solid?
        return new ImageData{data=bytes, type=type};
    }

    public static byte[] ReadBytesFromPath(string path)
    {
        // var resource = GD.Load(path);
        // var resource = GD.Load("absc");
        // GD.Print(resource);
        
        var file = new File();
        var error = file.Open(path, File.ModeFlags.Read);

        if(error != Error.Ok)
            throw new ArgumentException($"Failed to open file: {error}");
        
        var bytes = file.GetBuffer((long)file.GetLen());
        file.Close();

        return bytes;
        
        // return (byte[])(object)resource;
    }
}
