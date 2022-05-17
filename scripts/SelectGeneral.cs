using Godot;
using System;

public class SelectGeneral : HBoxContainer
{
    [Export] NodePath premadeMapOptionsPath;
    [Export] NodePath openFileGeneralPath;

    OptionButton premadeMapOptions;
    OpenFileGeneral openFileGeneral;

    public event EventHandler<ImageData> selected;

    public override void _Ready()
    {
        premadeMapOptions = (OptionButton)GetNode(premadeMapOptionsPath);
        openFileGeneral = (OpenFileGeneral)GetNode(openFileGeneralPath);

        premadeMapOptions.Connect("item_selected", this, nameof(OnPremadeMapOptionsItemSelected));
        openFileGeneral.Connect("pressed", this, nameof(OnOpenFileGeneralPressed));

        openFileGeneral.readCompleted += OnCustomReadCompleted;
    }

    void OnOpenFileGeneralPressed()
    {
        var _ = openFileGeneral.OnPressedAsync();
    }

    void OnPremadeMapOptionsItemSelected(int index)
    {
        GD.Print($"OnPremadeMapOptionsItemSelected: {index}");

        var pathVec = new string[]{ // TODO: hard code right now
            "res://textures/Généralités.png",
            "res://textures/Japan.png",
            "res://textures/TokyoBaseTex.png",
        };
        var path = pathVec[index];

        var image = (Image)GD.Load(path);
        var data = image.SavePngToBuffer(); // TODO: Is there way to "import" raw binary data? Godot import system is such a garbage.

        selected?.Invoke(this, new ImageData(){data=data, type="png"});
    }

    void OnCustomReadCompleted(object sender, ImageData imageData)
    {
        GD.Print("OnCustomReadCompleted");

        selected?.Invoke(this, imageData);
    }

    public void Select(int index)
    {
        OnPremadeMapOptionsItemSelected(index);
    }
}
