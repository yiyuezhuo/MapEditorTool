using Godot;
using System;
using System.Threading.Tasks;

public class ConsoleUI : VBoxContainer
{
    OpenFileGeneral openFileGeneral;

    TextureRect previewRect;
    TextureRect remapRect;
    TextEdit jsonOutput;
    Label messageLabel;

    ImageData currentImageData;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var premadeMapOptions = (OptionButton)GetNode("ToolContainer/PremadeMapOptions");
        openFileGeneral = (OpenFileGeneral)GetNode("ToolContainer/OpenFileGeneral");
        var processButton = (Button)GetNode("ToolContainer/ProcessButton");

        previewRect = (TextureRect)GetNode("PreviewRect");
        remapRect = (TextureRect)GetNode("RemapRect");
        jsonOutput = (TextEdit)GetNode("JSONOutput");
        messageLabel = (Label)GetNode("ToolContainer/MessageLabel");

        premadeMapOptions.Connect("item_selected", this, nameof(OnPremadeMapOptionsItemSelected));
        openFileGeneral.readCompleted += OnCustomReadCompleted;
        processButton.Connect("pressed", this, nameof(OnProcessButtonPressed));

        OnPremadeMapOptionsItemSelected(0);
    }

    void Message(string s) => messageLabel.Text = s;

    void OnPremadeMapOptionsItemSelected(int index)
    {
        GD.Print($"OnPremadeMapOptionsItemSelected: {index}");

        var pathVec = new string[]{ // TODO: hard code right now
            "res://textures/Généralités.png",
            "res://textures/Japan.png",
            "res://textures/TokyoBaseTex.png",
        };
        var path = pathVec[index];

        // var imageData = OpenFileGeneral.ReadDataFromPath(path);
        var image = (Image)GD.Load(path);
        var data = image.SavePngToBuffer(); // TODO: Is there way to "import" raw binary data? Godot import system is such a garbage.

        DoSelect(new ImageData(){data=data, type="png"});
    }

    void OnCustomReadCompleted(object sender, ImageData imageData)
    {
        GD.Print("OnCustomReadCompleted");

        DoSelect(imageData);
    }

    void DoSelect(ImageData imageData)
    {
        // var image = OpenFileGeneral.Decode(imageData);
        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);
        // var processor = new PixelMapPreprocessor<Color>(new ImageGodotBackend());
        // var image = processor.Decode()

        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        
        previewRect.Texture = tex;
        remapRect.Texture = null;
        jsonOutput.Text = "";

        currentImageData = imageData;
    }

    async Task DoProcessAsync(ImageData imageData)
    {
        var message = "Processing...";
        if(openFileGeneral.IsHTML5())
            message += "  (Warning: HTML5 version is 5x slower than native in the processing)";

        Message(message);

        await ToSignal(GetTree(), "idle_frame"); // Wait for UI to refresh
        await ToSignal(GetTree(), "idle_frame"); // For some reason I need to wait 2 frames?

        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Restart();

        DoProcessCore(imageData);

        stopWatch.Stop();
        Message($"Processing Finished. Elasped: {stopWatch.Elapsed}");
    }

    void DoProcessCore(ImageData imageData)
    {
        // var backend = new ImageSharpBackend();
        var backend = new ImageGodotBackend(); // trait?

        var result = PixelMapPreprocessor.Process(backend, imageData.data, imageData.type);

        var image = new Image();
        image.LoadPngFromBuffer(result.data);
        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);

        remapRect.Texture = tex;
        // jsonOutput.Text = $"Number of areas is {result.areaMap.Count}";
        jsonOutput.Text = PixelMapPreprocessor.ResultToJson(backend, result);
    }

    void OnProcessButtonPressed()
    {
        GD.Print("OnProcessButtonPressed");

        if(currentImageData != null)
        {
            var _ = DoProcessAsync(currentImageData);
        }
    }
}
