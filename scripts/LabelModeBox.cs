using Godot;
using System;


public enum LabelMode
{
    ID,
    Name,
    Hide
}


public class LabelModeBox : Node
{
    [Export] NodePath idCheckBoxPath;
    [Export] NodePath nameCheckBoxPath;
    [Export] NodePath hideCheckBoxPath;

    CheckBox idCheckBox;
    CheckBox nameCheckBox;
    CheckBox hideCheckBox;

    public event EventHandler<LabelMode> labelModeUpdated;
    // public event EventHandler nameModeEntered;
    // public event EventHandler hideModeEntered;

    public override void _Ready()
    {
        idCheckBox = (CheckBox)GetNode(idCheckBoxPath);
        nameCheckBox = (CheckBox)GetNode(nameCheckBoxPath);
        hideCheckBox = (CheckBox)GetNode(hideCheckBoxPath);

        idCheckBox.Connect("pressed", this, nameof(OnIdCheckBoxPressed));
        nameCheckBox.Connect("pressed", this, nameof(OnNameCheckBoxPressed));
        hideCheckBox.Connect("pressed", this, nameof(OnHideCheckBoxPressed));
    }

    void OnIdCheckBoxPressed() => labelModeUpdated?.Invoke(this, LabelMode.ID);
    void OnNameCheckBoxPressed() => labelModeUpdated?.Invoke(this, LabelMode.Name);
    void OnHideCheckBoxPressed() => labelModeUpdated?.Invoke(this, LabelMode.Hide);

}
