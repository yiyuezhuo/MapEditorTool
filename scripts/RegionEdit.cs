using Godot;
using System;
using System.Collections.Generic;

public class RegionEdit : VBoxContainer
{
    [Export] NodePath optionButtonPath;
    [Export] NodePath idRowPath;
    [Export] NodePath nameRowPath;

    OptionButton optionButton;
    LineEditRow idRow;
    LineEditRow nameRow;

    Region region;
    IEnumerable<SideCard> cardIter;

    public override void _Ready()
    {
        optionButton = (OptionButton)GetNode(optionButtonPath);
        idRow = (LineEditRow)GetNode(idRowPath);
        nameRow = (LineEditRow)GetNode(nameRowPath);

        optionButton.Connect("item_selected", this, nameof(OnOptionButtonItemSelected));

        idRow.textChanged += OnIdRowTextChanged;
        nameRow.textChanged += OnNameRowTextChanged;
    }

    public void BindRegion(Region region)
    {
        this.region = region;
    }

    public void BindCardIter(IEnumerable<SideCard> cardIter)
    {
        this.cardIter = cardIter;
    }

    void SyncRegionToUI()
    {
        idRow.SetText(region.id);
        nameRow.SetText(region.name);
    }

    void SyncCardIterToUI()
    {
        optionButton.Clear();
        optionButton.AddItem("");
        foreach(var sideCard in cardIter)
        {
            optionButton.AddItem(sideCard.data.id);
        }
    }

    void OnOptionButtonItemSelected(int index)
    {
        GD.Print($"OnOptionButtonItemSelected: {index}");
    }

    void OnIdRowTextChanged(object sender, string newText)
    {
        GD.Print($"OnIdRowTextChanged: {newText}");
    }

    void OnNameRowTextChanged(object sender, string newText)
    {
        GD.Print($"OnNameRowTextChanged: {newText}");
    }
}
