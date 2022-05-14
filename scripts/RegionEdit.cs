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
    List<SideData> sideDataList; // TODO: generalize it to Enumerable

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

        SyncRegionToUI();
    }

    public void BindSideDataList(List<SideData> sideDataList)
    {
        this.sideDataList = sideDataList;

        SyncCardIterToUI();
    }

    void SyncRegionToUI()
    {
        idRow.SetText(region.id);
        nameRow.SetText(region.name);

        if(region.side != null)
        {
            for(var i=0; i<sideDataList.Count; i++)
                if(region.side == sideDataList[i])
                {
                    optionButton.Selected = i + 1;
                    break;
                }
        }
        else
        {
            optionButton.Selected = 0; // no side placeholder
        }
    }

    void SyncCardIterToUI()
    {
        optionButton.Clear();
        optionButton.AddItem("");
        foreach(var sideData in sideDataList)
        {
            optionButton.AddItem(sideData.id);
        }
    }

    void OnOptionButtonItemSelected(int index)
    {
        GD.Print($"OnOptionButtonItemSelected: {index}");

        region.side = sideDataList[index - 1];
    }

    void OnIdRowTextChanged(object sender, string newText)
    {
        GD.Print($"OnIdRowTextChanged: {newText}");

        region.id = newText;
    }

    void OnNameRowTextChanged(object sender, string newText)
    {
        GD.Print($"OnNameRowTextChanged: {newText}");

        region.name = newText;
    }
}
