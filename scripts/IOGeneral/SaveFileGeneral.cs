using Godot;
using System;

public class SaveFileGeneral : IOFileGeneral
{
    byte[] data;
    /*
    public event EventHandler pressed;

    protected override void OnPressed()
    {
        GD.Print("OnPressed");

        pressed?.Invoke(this, EventArgs.Empty);
    }
    */

    /// <summary>
    /// name is download name for HTML5 version or initial name in desktop FileDialog
    /// </summary>
    public void StartSave(byte[] data, string name)
    {
        this.data = data;

        if(IsHTML5())
        {
            html5file.SaveData(data, name);
        }
        else
        {
            fileDialog.CurrentFile = name;
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

        data = null;
    }
}
