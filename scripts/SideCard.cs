using Godot;
using System;

public class SideData
{
    public string id="";
    public string name="";
    public Color color=new Color(1,1,1,1);
}


public class SideCard : VBoxContainer
{
    [Export] NodePath indexLabelPath;
    [Export] NodePath upButtonPath;
    [Export] NodePath downButtonPath;
    [Export] NodePath addButtonPath;
    [Export] NodePath deleteButtonPath;
    [Export] NodePath idLineEditRowPath;
    [Export] NodePath nameLineEditRowPath;
    [Export] NodePath colorPickerButtonPath;

    Label indexLabel;
    LineEditRow idLineEditRow;
    LineEditRow nameLineEditRow;
    ColorPickerButton colorPickerButton;

    SideData _data;
    public SideData data
    {
        get => _data;
        set // BindData 
        {
            if(_data == value)
                return;
            _data = value;
            SyncDataToUI();
        }
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

    public event EventHandler<string> idChanged;
    public event EventHandler<string> nameChanged;
    public event EventHandler<Color> colorChanged;

    public override void _Ready()
    {
        indexLabel = (Label)GetNode(indexLabelPath);

        ((Button)GetNode(upButtonPath)).Connect("pressed", this, nameof(OnUpButtonPressed));
        ((Button)GetNode(downButtonPath)).Connect("pressed", this, nameof(OnDownButtonPressed));
        ((Button)GetNode(addButtonPath)).Connect("pressed", this, nameof(OnAddButtonPressed));
        ((Button)GetNode(deleteButtonPath)).Connect("pressed", this, nameof(OnDeleteButtonPressed));

        idLineEditRow = (LineEditRow)GetNode(idLineEditRowPath);
        nameLineEditRow = (LineEditRow)GetNode(nameLineEditRowPath);
        colorPickerButton = (ColorPickerButton)GetNode(colorPickerButtonPath);

        colorPickerButton.Connect("color_changed", this, nameof(OnColorPickerButtonColorChanged));

        idLineEditRow.textChanged += OnIdLineEditRowTextChanged;
        nameLineEditRow.textChanged += OnNameLineEditRowTextChanged;
    }

    void OnIdLineEditRowTextChanged(object sender, string newText)
    {
        data.id = newText;

        idChanged?.Invoke(this, newText);
    }

    void OnNameLineEditRowTextChanged(object sender, string newText)
    {
        data.name = newText;

        nameChanged?.Invoke(this, newText);
    }

    void OnColorPickerButtonColorChanged(Color color)
    {
        data.color = color;
        
        colorChanged?.Invoke(this, color);
    }

    void SyncDataToUI()
    {
        idLineEditRow.SetText(data.id);
        nameLineEditRow.SetText(data.name);
        colorPickerButton.Color = data.color;
    }

    void OnUpButtonPressed() => upButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnDownButtonPressed() => downButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnAddButtonPressed() => addButtonPressed?.Invoke(this, EventArgs.Empty);

    void OnDeleteButtonPressed()
    {
       QueueFree();
       deleteButtonPressed?.Invoke(this, EventArgs.Empty);
    }
}
