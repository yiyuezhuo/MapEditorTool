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

        openFileGeneral.loadCompleted += OnLoadCompleted;
        // saveFileGeneral.pressed += OnSaveFileGeneralPressed;
        saveFileGeneral.Connect("pressed", this, nameof(OnSaveFileGeneralPressed));
        openFileGeneral.Connect("pressed", this, nameof(OnOpenFileGeneralPressed));
    }

    public void OnLoadCompleted(object sender, Image image)
    {
        GD.Print("OnLoadCompleted");

        this.image = image;
        
        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        textureRect.Texture = tex;
    }

    public void OnOpenFileGeneralPressed()
    {
        var _ = openFileGeneral.OnPressedAsync();
    }

    public void OnSaveFileGeneralPressed()
    {
        if(image != null)
            saveFileGeneral.StartSave(image.SavePngToBuffer(), "export.png");
    }
}
