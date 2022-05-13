using Godot;
using System;

public class LineEditRow : HBoxContainer
{
    [Export] NodePath lineEditPath;

    LineEdit lineEdit;
    public override void _Ready()
    {
        lineEdit = (LineEdit)GetNode(lineEditPath);
    }

    public void SetText(string s)
    {
        lineEdit.Text = s;
    }
}
