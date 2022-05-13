using Godot;
using System;

public class SideButton : Button
{
    [Export] NodePath windowDialogPath;

    WindowDialog windowDialog;

    public override void _Ready()
    {
        windowDialog = (WindowDialog)GetNode(windowDialogPath);
        Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        windowDialog.Popup_();
    }
}
