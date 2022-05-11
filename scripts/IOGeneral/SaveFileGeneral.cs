using Godot;
using System;

public class SaveFileGeneral : IOFileGeneral
{
    [Export] public string defaultName = "download"; // download name or default name for FileDialog

    byte[] data;
    bool neverBinded = true;

    public override void _Ready()
    {
        base._Ready();
    }

    public void BindData(byte[] data)
    {
        neverBinded = false;
        this.data = data;
    }

    protected override void OnPressed()
    {
        GD.Print("OnPressed");

        if(neverBinded)
            return;

        if(IsHTML5())
        {
            html5file.SaveData(data, defaultName);
        }
        else
        {
            fileDialog.CurrentFile = defaultName;
            fileDialog.Popup_();
        }
    }

    protected override void OnFileDialogFileSelected(string path)
    {
        GD.Print($"OnFileDialogFileSelected: {path}");

        var file = new File();
        var error = file.Open(path, File.ModeFlags.Write);

        if(error != Error.Ok)
            throw new ArgumentException($"Failed to save file: {error}");
        
        file.StoreBuffer(data);
        file.Close();
    }
}
