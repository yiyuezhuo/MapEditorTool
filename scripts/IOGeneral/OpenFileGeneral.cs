using Godot;
using System;
using System.Threading.Tasks;

public class OpenFileGeneral : IOFileGeneral
{
    TaskCompletionSource<TypedData> tcs = null;

    public class Accept
    {
        // public string image = "image/png, image/jpeg, image/webp";
        // public string json = "application/JSON";

        public abstract class Filter
        {
            public abstract string ToHTML5();
            public abstract string[] ToFileDialog();
        }

        public class Image : Filter
        {
            public override string ToHTML5() => "image/png, image/jpeg, image/webp";
            public override string[] ToFileDialog() => new string[]{"*.png", "*.jpg", "*.webp"};
        }

        public class Json : Filter
        {
            public override string ToHTML5() => "application/JSON";
            public override string[] ToFileDialog() => new string[]{"*.json"};
        }

        public static Image image = new Image();
        public static Json json = new Json();
    }

    public event EventHandler<TypedData> readCompleted;

    async Task<TypedData> GetTypedData(Accept.Filter accept)
    {
        TypedData imageData;
        if(IsHTML5())
        {
            imageData = await html5file.ReadDataAsync(accept.ToHTML5()); // type = "image/jpg", "image/png", ..., "text/json"
            imageData.type = imageData.type.Replace("image/", "");
        }
        else
        {
            tcs = new TaskCompletionSource<TypedData>();
            fileDialog.Filters = accept.ToFileDialog();
            fileDialog.Popup_();
            imageData = await tcs.Task; // type = "jpg", "png", ...
        }
        return imageData; // type = "jpg", "png", ...
    }

    public async Task StartRead(Accept.Filter accept)
    {
        GD.Print("OnPressed");

        var imageData = await GetTypedData(accept);

        readCompleted?.Invoke(this, imageData);
    }

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
