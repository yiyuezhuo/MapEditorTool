/*
using Godot;
using System;

public class SaveFileButton : SaveFileGeneral
{
    public event EventHandler pressed;

    public override void _Ready()
    {
        base._Ready();

        Connect("pressed", this, nameof(OnPressed));
    }

    protected void OnPressed()
    {
        GD.Print("OnPressed");

        pressed?.Invoke(this, EventArgs.Empty);
    }
}
*/