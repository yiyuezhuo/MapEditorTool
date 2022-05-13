using Godot;
using System;

public class ImageShower : Node
{
    OpenFileGeneral openFileGeneral;
    SaveFileGeneral saveFileGeneral;
    TextureRect textureRect;

    public override void _Ready()
    {
        openFileGeneral = (OpenFileGeneral)GetNode("TopBar/OpenFileGeneral");
        saveFileGeneral = (SaveFileGeneral)GetNode("TopBar/SaveFileGeneral");

        textureRect = (TextureRect)GetNode("TextureRect");

        openFileGeneral.readCompleted += OnReadCompleted;
        openFileGeneral.loadCompleted += OnLoadCompleted;
    }

    public void OnLoadCompleted(object sender, Image image)
    {
        GD.Print("OnLoadCompleted");
        
        var tex = new ImageTexture();
        tex.CreateFromImage(image, 0);
        textureRect.Texture = tex;
    }

    public void OnReadCompleted(object sender, ImageData imageData)
    {
        saveFileGeneral.BindData(imageData.data);
        saveFileGeneral.defaultName = $"download.{imageData.type}";
    }
}
