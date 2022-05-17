using Godot;
using System;

public class ImageShower : Node
{
    [Export] NodePath openFileGeneralPath;
    [Export] NodePath saveFileGeneralPath;
    [Export] NodePath textureRectPath;

    OpenFileGeneral openFileGeneral;
    SaveFileGeneral saveFileGeneral;
    TextureRect textureRect;

    Image image;

    public override void _Ready()
    {
        openFileGeneral = (OpenFileGeneral)GetNode(openFileGeneralPath);
        saveFileGeneral = (SaveFileGeneral)GetNode(saveFileGeneralPath);

        textureRect = (TextureRect)GetNode(textureRectPath);

        openFileGeneral.readCompleted += OnReadCompleted;
        saveFileGeneral.Connect("pressed", this, nameof(OnSaveFileGeneralPressed));
        openFileGeneral.Connect("pressed", this, nameof(OnOpenFileGeneralPressed));
    }

    public void OnReadCompleted(object sender, TypedData typedData)
    {
        GD.Print("ImageShower.OnReadCompleted");
        var image = ImageGodotBackend.Decode(typedData.data, typedData.type);

        this.image = image;
        
        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        textureRect.Texture = tex;
    }

    public void OnOpenFileGeneralPressed()
    {
        var _ = openFileGeneral.StartRead(OpenFileGeneral.Accept.image);
    }

    public void OnSaveFileGeneralPressed()
    {
        if(image != null)
            saveFileGeneral.StartSave(image.SavePngToBuffer(), "export.png");
    }
}
