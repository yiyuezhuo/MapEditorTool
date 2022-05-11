using Godot;
using System;

public class SideData
{
    public string id;
    public string name;
    public Color color;
}


public class SideCard : VBoxContainer
{
    [Export] NodePath indexLabelPath;
    [Export] NodePath upButtonPath;
    [Export] NodePath downButtonPath;
    [Export] NodePath addButtonPath;
    [Export] NodePath deleteButtonPath;
    [Export] NodePath idLineEditPath;
    [Export] NodePath nameLineEditPath;
    [Export] NodePath colorPickerButtonPath;

    Label indexLabel;
    LineEdit idLineEdit;
    LineEdit nameLineEdit;
    ColorPickerButton colorPickerButton;

    SideData _data;
    public SideData data
    {
        get => _data;
        set
        {
            if(_data == value)
                return;
            _data = value;
            SyncDataToUI();
        }
    }

    void SyncDataToUI()
    {
        idLineEdit.Text = data.id;
        nameLineEdit.Text = data.name;
        colorPickerButton.Color = data.color;
    }

    int _index;
    public int index
    {
        get => _index; 
        set
        {
            if(_index == value)
                return;
            _index = value;
            indexLabel.Text = value.ToString();
        }
    }

    public event EventHandler upButtonPressed;
    public event EventHandler downButtonPressed;
    public event EventHandler addButtonPressed;
    public event EventHandler deleteButtonPressed;

    /*
    public event EventHandler<string> idLineEditTextChanged;
    public event EventHandler<string> nameLineEditTextChanged;
    public event EventHandler<Color> colorPickerButtonColorChanged;
    */

    public override void _Ready()
    {
        indexLabel = (Label)GetNode(indexLabelPath);

        ((Button)GetNode(upButtonPath)).Connect("pressed", this, nameof(OnUpButtonPressed));
        ((Button)GetNode(downButtonPath)).Connect("pressed", this, nameof(OnDownButtonPressed));
        ((Button)GetNode(addButtonPath)).Connect("pressed", this, nameof(OnAddButtonPressed));
        ((Button)GetNode(deleteButtonPath)).Connect("pressed", this, nameof(OnDeleteButtonPressed));

        idLineEdit = (LineEdit)GetNode(idLineEditPath);
        // idLineEdit.Connect("text_changed", this, nameof(OnIdLineEditTextChanged));

        nameLineEdit = (LineEdit)GetNode(nameLineEditPath);
        // nameLineEdit.Connect("text_changed", this, nameof(OnNameLineEditTextChanged));

        colorPickerButton = (ColorPickerButton)GetNode(colorPickerButtonPath);
        // colorPickerButton.Connect("color_changed", this, nameof(OnColorPickerButtonColorChanged));
    }

    void OnUpButtonPressed() => upButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnDownButtonPressed() => downButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnAddButtonPressed() => addButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnDeleteButtonPressed()
    {
       QueueFree();
       deleteButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    /*
    void OnIdLineEditTextChanged(string newText) => idLineEditTextChanged?.Invoke(this, newText);
    void OnNameLineEditTextChanged(string newText) => nameLineEditTextChanged?.Invoke(this, newText);
    void OnColorPickerButtonColorChanged(Color color) => colorPickerButtonColorChanged?.Invoke(this, color);
    */
}
