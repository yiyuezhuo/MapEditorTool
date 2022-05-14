using Godot;
using System;

public class LineEditRow : HBoxContainer
{
    [Export] NodePath lineEditPath;

    LineEdit lineEdit;

    public event EventHandler<string> textChanged;
    
    public override void _Ready()
    {
        lineEdit = (LineEdit)GetNode(lineEditPath);
        lineEdit.Connect("text_changed", this, nameof(OnTextChanged));
    }

    public void SetText(string s)
    {
        lineEdit.Text = s;
    }

    void OnTextChanged(string newText)
    {
        textChanged?.Invoke(this, newText);
    }
}
