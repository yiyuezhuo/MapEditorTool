namespace references
{


using Godot;
using System.Threading.Tasks;

public class HTML5FileExchange : Godot.Object // We need Godot.Object to use JavaScript.CreateCallback
{

    JavaScriptObject jsCallback;
    JavaScriptObject jsInterface;

    TaskCompletionSource<bool> tcs = null;

    public HTML5FileExchange()
    {
        // if(OS.GetName() == "HTML5" && OS.HasFeature("JavaScript"))
        InitializeEngine();
    }

    bool supported {get => OS.GetName() == "HTML5" && OS.HasFeature("JavaScript");}

    void InitializeEngine()
    {
        JavaScript.Eval(@"
        var _HTML5FileExchange = {};
        _HTML5FileExchange.upload = function(gd_callback) {
            canceled = true;
            var input = document.createElement('INPUT'); 
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/png, image/jpeg, image/webp');
            input.click();
            input.addEventListener('change', event => {
                if (event.target.files.length > 0){
                    canceled = false;}
                var file = event.target.files[0];
                var reader = new FileReader();
                this.fileType = file.type;
                reader.readAsArrayBuffer(file);
                reader.onloadend = (evt) => { // Since here's it's arrow function, 'this' still refers to _HTML5FileExchange
                    if (evt.target.readyState == FileReader.DONE) {
                        this.result = evt.target.result;
                        console.log('before gd_callback', gd_callback);
                        gd_callback(); // It's hard to retrieve value from callback argument, so it's just for notification
                        console.log('after gd_callback', gd_callback);
                    }
                }
            });
        }", true);
        jsInterface = JavaScript.GetInterface("_HTML5FileExchange");
        jsCallback = JavaScript.CreateCallback(this, nameof(LoadHandler));
    }

    public void LoadHandler()
    {
        GD.Print("LoadHandler");
        tcs.TrySetResult(true);
    }

    public async Task<Image> LoadImageAsync()
    {
        tcs = new TaskCompletionSource<bool>();
        jsInterface.Call("upload", jsCallback);
        await tcs.Task;
        GD.Print("after await tcs.Task");
        var imageType = (string)jsInterface.Get("fileType");
        var imageData = (byte[])JavaScript.Eval("_HTML5FileExchange.result", true);

        var image = new Image();
        Error? imageError = null;
        switch(imageType)
        {
            case "image/png":
                imageError = image.LoadPngFromBuffer(imageData);
                break;
            case "image/jpeg":
                imageError = image.LoadJpgFromBuffer(imageData);
                break;
            case "image/webp":
                imageError = image.LoadWebpFromBuffer(imageData);
                break;
            default:
                GD.Print($"Unsupported file format - {imageType}.");
                return null;
        }

        if(imageError != null)
        {
            GD.Print("An error occurred while trying to display the image.");
            return null;
        }

        return image;
    }

    public void SaveImage(Image image, string fileName)
    {
        image.ClearMipmaps();
        var buffer = image.SavePngToBuffer();
        JavaScript.DownloadBuffer(buffer, fileName);
    }
}


}