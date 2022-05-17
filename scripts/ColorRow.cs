using Godot;
using System;

public class ColorRow : HBoxContainer
{
    [Export] NodePath lineEditPath;
    [Export] NodePath colorRectPath;

    LineEdit lineEdit;
    ColorRect colorRect;

    public override void _Ready()
    {
        lineEdit = (LineEdit)GetNode(lineEditPath);
        colorRect = (ColorRect)GetNode(colorRectPath);  
    }

    public void SetColor(Color color) => SetData(ImageGodotBackend.StringfyColor(color), color);

    public void SetData(string text, Color color)
    {
        lineEdit.Text = text;
        colorRect.Color = color;
    }
}
