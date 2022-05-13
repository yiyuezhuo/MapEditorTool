using Godot;
using System;
using System.Collections.Generic;


public class SideCardContainer : VBoxContainer
{
    [Export] NodePath cardContainerPath;
    [Export] PackedScene cardScene;

    Control cardContainer;

    List<SideCard> cardList = new List<SideCard>();

    public override void _Ready()
    {
        cardContainer = (Control)GetNode(cardContainerPath);

        Reset();

        // AddData(new SideData(){id="french", name="French", color = new Color(0,0,1)});
        // AddData(new SideData(){id="alliance", name="Alliance", color = new Color(1,0,0)});
        BindData(new List<SideData>(){
            new SideData(){id="french", name="French", color = new Color(0,0,1)},
            new SideData(){id="alliance", name="Alliance", color = new Color(1,0,0)}
        });
    }

    void Reset()
    {
        foreach(Node child in cardContainer.GetChildren()) // clear placeholder
            child.QueueFree();
        cardList.Clear();
    }

    SideCard CreateCard()
    {
        var card = cardScene.Instance<SideCard>();
        
        card.upButtonPressed += OnCardUpButtonPressed;
        card.downButtonPressed += OnCardDownButtonPressed;
        card.addButtonPressed += OnCardAddButtonPressed;
        card.deleteButtonPressed += OnCardDeleteButtonPressed;

        return card;
    }

    SideCard AddCard(SideData data)
    {
        var card = CreateCard();
        cardContainer.AddChild(card);
        card.data = data;

        return card;
    }

    void AddData(SideData data)
    {
        var card = AddCard(data);
        card.index = cardList.Count;
        cardList.Add(card);
    }

    void InsertData(SideData data, int index)
    {
        var card = AddCard(data);
        card.index = index;
        cardList.Insert(index, card);
        cardContainer.MoveChild(card, index);

        ResetIndex();
    }

    public void BindData(List<SideData> dataList)
    {
        foreach(var data in dataList)
            AddData(data);
    }

    void OnCardUpButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;

        if(idx >= 1)
        {
            var upCard = cardList[idx - 1];
            cardList[idx - 1] = cardList[idx];
            cardList[idx] = upCard;
            cardContainer.MoveChild(upCard, idx);

            ResetIndex();
        }
    }

    void OnCardDownButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;

        if(idx <= cardList.Count-2)
        {
            var downCard = cardList[idx + 1];
            cardList[idx + 1] = cardList[idx];
            cardList[idx] = downCard;
            cardContainer.MoveChild(downCard, idx);

            ResetIndex();
        }
    }

    void OnCardAddButtonPressed(object sender, EventArgs _)
    {
        GD.Print("OnCardAddButtonPressed");
        var idx = ((SideCard)sender).index;

        InsertData(new SideData(), idx);

        ResetIndex();
    }
    void OnCardDeleteButtonPressed(object sender, EventArgs _)
    {
        var card = (SideCard)sender;
        cardList.RemoveAt(card.index);

        if(cardList.Count == 0)
            AddData(new SideData());

        ResetIndex();
    }

    void ResetIndex()
    {
        var index = 0;
        foreach(var card in cardList)
        {
            card.index = index;
            index++;
        }
    }
}
