using Godot;
using System;

public class ConsoleUI : VBoxContainer
{
    TextureRect previewRect;
    TextureRect remapRect;
    TextEdit jsonOutput;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var premadeMapOptions = (OptionButton)GetNode("ToolContainer/PremadeMapOptions");
        var openFileGeneral = (OpenFileGeneral)GetNode("ToolContainer/OpenFileGeneral");
        var processButton = (Button)GetNode("ToolContainer/ProcessButton");

        previewRect = (TextureRect)GetNode("PreviewRect");
        remapRect = (TextureRect)GetNode("RemapRect");
        jsonOutput = (TextEdit)GetNode("JSONOutput");

        premadeMapOptions.Connect("item_selected", this, nameof(OnPremadeMapOptionsItemSelected));
        openFileGeneral.readCompleted += OnCustomReadCompleted;
        processButton.Connect("pressed", this, nameof(OnProcessButtonPressed));

        OnPremadeMapOptionsItemSelected(0);
    }

    void OnPremadeMapOptionsItemSelected(int index)
    {
        GD.Print($"OnPremadeMapOptionsItemSelected: {index}");

        var pathVec = new string[]{ // TODO: hard code at this point
            "res://textures/Généralités.png",
            "res://addons/MapKit/Sample/sample_map.png",
            "res://textures/TokyoBaseTex.png",
        };
        var path = pathVec[index];

        var imageData = OpenFileGeneral.ReadDataFromPath(path);

        DoPreview(imageData);
    }

    void OnCustomReadCompleted(object sender, ImageData imageData)
    {
        GD.Print("OnCustomReadCompleted");

        DoPreview(imageData);
    }

    void DoPreview(ImageData imageData)
    {
        var image = OpenFileGeneral.Decode(imageData);
        var tex = new ImageTexture();
        tex.CreateFromImage(image);
        previewRect.Texture = tex;
    }

    void DoProcess(ImageData imageData)
    {
        
    }

    void OnProcessButtonPressed()
    {
        GD.Print("OnProcessButtonPressed");
    }
}
