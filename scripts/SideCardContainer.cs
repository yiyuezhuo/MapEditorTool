using Godot;
using System;
using System.Collections.Generic;


public class SideCardContainer : Node
{
    [Export] NodePath cardContainerPath;
    [Export] PackedScene cardScene;

    Control cardContainer;

    // List<SideCard> cardList = new List<SideCard>();
    List<SideData> dataList = new List<SideData>();
    Dictionary<SideData, SideCard> cardMap = new Dictionary<SideData, SideCard>();

    public event EventHandler dataListIdUpdated;
    public event EventHandler dataListStructureUpdated;

    public override void _Ready()
    {
        cardContainer = (Control)GetNode(cardContainerPath);

        Reset();

        /*
        BindData(new List<SideData>(){
            new SideData(){id="french", name="French", color = new Color(0,0,1)},
            new SideData(){id="alliance", name="Alliance", color = new Color(1,0,0)}
        });
        */
    }

    void Reset()
    {
        foreach(Node child in cardContainer.GetChildren()) // clear placeholder
            child.QueueFree();
        dataList.Clear();
        cardMap.Clear();
        // cardList.Clear();
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
        cardMap[data] = card;

        return card;
    }

    void AddData(SideData data)
    {
        var card = AddCard(data);
        card.index = dataList.Count;
        dataList.Add(data);
        // cardMap[data] = card;
        // cardList.Add(card);
    }

    void InsertData(SideData data, int index)
    {
        var card = AddCard(data);
        card.index = index;
        dataList.Insert(index, data);
        // cardMap[data] = card;
        // cardList.Insert(index, card);
        cardContainer.MoveChild(card, index);

        ResetIndex();
    }

    public void BindData(List<SideData> dataList)
    { 
        this.dataList = dataList;
        cardMap.Clear();

        foreach(Node child in cardContainer.GetChildren()) // clear placeholder
            child.QueueFree();
        for(var i=0; i<dataList.Count; i++)
        {
            var data = dataList[i];
            var card = AddCard(data);
            card.index = i;
            // cardMap[data] = card;
        }
    }

    void OnCardUpButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;

        if(idx >= 1)
        {
            var upData = dataList[idx - 1];
            dataList[idx - 1] = dataList[idx];
            dataList[idx] = upData;
            cardContainer.MoveChild(cardMap[upData], idx);

            ResetIndex();
        }
    }

    void OnCardDownButtonPressed(object sender, EventArgs _)
    {
        var idx = ((SideCard)sender).index;

        if(idx <= dataList.Count-2)
        {
            var downData = dataList[idx + 1];
            dataList[idx + 1] = dataList[idx];
            dataList[idx] = downData;
            cardContainer.MoveChild(cardMap[downData], idx);

            ResetIndex();
        }
    }

    void OnCardAddButtonPressed(object sender, EventArgs _)
    {
        GD.Print("OnCardAddButtonPressed");
        var idx = ((SideCard)sender).index;

        // InsertData(new SideData(), idx);
        InsertData(new SideData(), idx + 1);

        ResetIndex();
    }
    void OnCardDeleteButtonPressed(object sender, EventArgs _)
    {
        var card = (SideCard)sender;
        dataList.RemoveAt(card.index);
        cardMap.Remove(card.data);

        if(dataList.Count == 0)
            AddData(new SideData());

        ResetIndex();
    }

    void ResetIndex()
    {
        /*
        var index = 0;
        foreach(var data in dataList)
        {
            card.index = index;
            index++;
        }
        */
        for(var i=0; i<dataList.Count; i++)
        {
            var card = cardMap[dataList[i]];
            card.index = i;
        }
    }
}
