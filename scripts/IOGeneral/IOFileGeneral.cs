using Godot;
using System;

public abstract class IOFileGeneral : Button
{
    [Export] NodePath fileDialogPath;

    protected FileDialog fileDialog;
    protected FileExchangerSharp html5file;

    public override void _Ready()
    {
        fileDialog = (FileDialog)GetNode(fileDialogPath);
        html5file = (FileExchangerSharp)GetNode("/root/FileExchangerSharp");

        fileDialog.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

        Connect("pressed", this, nameof(OnPressed));
        fileDialog.Connect("file_selected", this, nameof(OnFileDialogFileSelected));
    }

    protected abstract void OnPressed();
    protected abstract void OnFileDialogFileSelected(string path);

    public static bool IsHTML5() => OS.GetName() == "HTML5" && OS.HasFeature("JavaScript");
}
