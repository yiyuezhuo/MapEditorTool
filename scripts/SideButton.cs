using Godot;
using System;

public class SideButton : Button
{
    [Export] NodePath windowDialogPath;
    [Export] NodePath sideCardContainerPath;

    WindowDialog windowDialog;
    public SideCardContainer sideCardContainer;

    public override void _Ready()
    {
        sideCardContainer = (SideCardContainer)GetNode(sideCardContainerPath);

        windowDialog = (WindowDialog)GetNode(windowDialogPath);
        Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        windowDialog.Popup_();
    }
}
