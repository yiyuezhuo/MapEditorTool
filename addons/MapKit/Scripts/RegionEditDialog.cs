using Godot;
using System;

public class RegionEditDialog : PopupPanel
{
    [Export] NodePath regionEditPath;

    public RegionEdit regionEdit;
    public override void _Ready()
    {
        regionEdit = (RegionEdit)GetNode(regionEditPath);
    }
}
