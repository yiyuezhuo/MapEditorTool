
using Godot;
using System;

public class RegionLabel : Control
{
    [Export] NodePath labelPath;

    Label label;

    public override void _Ready()
    {
        label = (Label)GetNode(labelPath);
    }

    public string Text
    {
        get => label.Text; 
        set
        {
            label.Text = value;
            if(value.Length == 0)
                Hide();
            else
                Show();
        }
    }

    public void Sync()
    {

    }
}
