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

        // openFileGeneral.readCompleted += OnReadCompleted;
        openFileGeneral.loadCompleted += OnLoadCompleted;
        saveFileGeneral.pressed += OnSaveFileGeneralPressed;
    }

    public void OnLoadCompleted(object sender, Image image)
    {
        GD.Print("OnLoadCompleted");

        this.image = image;
        
        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        textureRect.Texture = tex;
    }

    public void OnSaveFileGeneralPressed(object sender, EventArgs _)
    {
        if(image != null)
            saveFileGeneral.StartSave(image.SavePngToBuffer(), "export.png");
    }

    /*
    public void OnReadCompleted(object sender, ImageData imageData)
    {
        saveFileGeneral.BindData(imageData.data);
        saveFileGeneral.defaultName = $"download.{imageData.type}";
    }
    */
}
