using Godot;
using System;
using System.Collections.Generic;


public class SideCardContainer : VBoxContainer
{
    [Export] NodePath cardContainerPath;
    [Export] PackedScene cardScene;

    Control cardContainer;

    List<SideData> dataList = new List<SideData>();
    Dictionary<string, SideData> dataMap = new Dictionary<string, SideData>();

    public override void _Ready()
    {
        cardContainer = (Control)GetNode(cardContainerPath);

        // ResetIndex();
        foreach(Node child in cardContainer.GetChildren())
            child.QueueFree();

        AddCard(new SideData());
        AddCard(new SideData());
    }

    SideCard CreateCard()
    {
        var card = cardScene.Instance<SideCard>();
        
        card.upButtonPressed += OnCardUpButtonPressed;
        card.downButtonPressed += OnCardDownButtonPressed;
        card.addButtonPressed += OnCardAddButtonPressed;
        card.deleteButtonPressed += OnCardDeleteButtonPressed;

        /*
        card.idLineEditTextChanged += OnCardIdLineEditTextChanged;
        card.nameLineEditTextChanged += OnCardNameLineEditTextChanged;
        card.colorPickerButtonColorChanged += OnCardColorPickerButtonColorChanged;
        */

        return card;
    }

    void AddCard(SideData data)
    {
        var card = CreateCard();
        cardContainer.AddChild(card);
        card.data = data;
        card.index = cardContainer.GetChildCount() - 1;
    }

    public void BindData()
    {

    }

    void OnCardUpButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;
    }

    void OnCardDownButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;
    }

    void OnCardAddButtonPressed(object sender, EventArgs _)
    {
        GD.Print("OnCardAddButtonPressed");
        var idx = ((SideCard)sender).index;

        AddCard(new SideData());
        ResetIndex();
    }
    void OnCardDeleteButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;
        ResetIndex();
    }

    void ResetIndex()
    {
        var index = 0;
        foreach(SideCard card in cardContainer.GetChildren())
        {
            card.index = index;
            index++;
        }
    }

    /*
    void OnCardIdLineEditTextChanged(object sender, string s)
    {
        var idx = ((SideCard)sender).index;
    }

    void OnCardNameLineEditTextChanged(object sender, string s)
    {
        var idx = ((SideCard)sender).index;
    }
    void OnCardColorPickerButtonColorChanged(object sender, Color color)
    {
        var idx = ((SideCard)sender).index;
    }
    */
}
