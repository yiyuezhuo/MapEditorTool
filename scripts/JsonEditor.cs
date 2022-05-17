using Godot;
using System;

public class JsonEditor : VBoxContainer
{
    [Export] NodePath openFileGeneralPath;
    [Export] NodePath saveFileGeneralPath;
    [Export] NodePath textEditPath;

    OpenFileGeneral openFileGeneral;
    SaveFileGeneral saveFileGeneral;
    TextEdit textEdit;

    public override void _Ready()
    {
        openFileGeneral = (OpenFileGeneral)GetNode(openFileGeneralPath);
        saveFileGeneral = (SaveFileGeneral)GetNode(saveFileGeneralPath);
        textEdit = (TextEdit)GetNode(textEditPath);

        openFileGeneral.Connect("pressed", this, nameof(OnOpenFileGeneralPressed));
        openFileGeneral.readCompleted += OnOpenFileGeneralReadCompleted;
    }

    void OnOpenFileGeneralPressed()
    {
        _ = openFileGeneral.StartRead(OpenFileGeneral.Accept.json);
    }

    void OnOpenFileGeneralReadCompleted(object sender, TypedData typedData)
    {
        GD.Print($"length={typedData.data.Length}, type={typedData.type}");

        var text = System.Text.Encoding.UTF8.GetString(typedData.data);
        textEdit.Text = text;
    }
}
