using Godot;
using System;

public class ImageShower : Node
{
    OpenFileGeneral openFileGeneral;
    TextureRect textureRect;

    public override void _Ready()
    {
        openFileGeneral = (OpenFileGeneral)GetNode("OpenFileGeneral");
        textureRect = (TextureRect)GetNode("TextureRect");

        openFileGeneral.loadCompleted += OnLoadCompleted;
    }

    public void OnLoadCompleted(object sender, Image image)
    {
        GD.Print("OnLoadCompleted");
        
        var tex = new ImageTexture();
        tex.CreateFromImage(image);
        textureRect.Texture = tex;
    }
}
