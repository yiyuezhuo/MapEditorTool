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

    TypedData currentImageData;

    public override void _Ready()
    {
        // var premadeMapOptions = (OptionButton)GetNode("ToolContainer/PremadeMapOptions");
        // openFileGeneral = (OpenFileGeneral)GetNode("ToolContainer/OpenFileGeneral");
        var selectGeneral = (SelectGeneral)GetNode("ToolContainer/SelectGeneral");
        var processButton = (Button)GetNode("ToolContainer/ProcessButton");

        previewRect = (TextureRect)GetNode("PreviewRect");
        remapRect = (TextureRect)GetNode("RemapRect");
        jsonOutput = (TextEdit)GetNode("JSONOutput");
        messageLabel = (Label)GetNode("ToolContainer/MessageLabel");

        // premadeMapOptions.Connect("item_selected", this, nameof(OnPremadeMapOptionsItemSelected));
        // openFileGeneral.readCompleted += OnCustomReadCompleted;
        selectGeneral.selected += OnSelectGeneralSelected;
        processButton.Connect("pressed", this, nameof(OnProcessButtonPressed));

        // OnPremadeMapOptionsItemSelected(0);
        selectGeneral.Select(0);
    }

    void Message(string s) => messageLabel.Text = s;

    void OnSelectGeneralSelected(object sender, TypedData imageData)
    {
        DoSelect(imageData);
    }

    void DoSelect(TypedData imageData)
    {
        var image = ImageGodotBackend.Decode(imageData.data, imageData.type);

        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        
        previewRect.Texture = tex;
        remapRect.Texture = null;
        jsonOutput.Text = "";

        currentImageData = imageData;
    }

    async Task DoProcessAsync(TypedData imageData)
    {
        var message = "Processing...";
        if(IOFileGeneral.IsHTML5())
            message += "   (Warning: HTML5 version is 20x slower than native for the processing)";

        Message(message);

        await ToSignal(GetTree(), "idle_frame"); // Wait for UI to refresh
        await ToSignal(GetTree(), "idle_frame"); // For some reason I need to wait 2 frames?

        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Restart();

        DoProcessCore(imageData);

        stopWatch.Stop();
        Message($"Processing Finished. Elapsed: {stopWatch.Elapsed}");
    }

    void DoProcessCore(TypedData imageData)
    {
        // var backend = new ImageSharpBackend();
        var backend = new ImageGodotBackend(); // trait?

        var result = PixelMapPreprocessor.Process(backend, imageData.data, imageData.type);
        // TODO: Provide a ToGodotImage API to remove the unnecessary encode & decode overhead when we're using ImageGodotBackend.

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
