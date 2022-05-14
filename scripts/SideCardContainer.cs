using Godot;
using System;
using System.Collections.Generic;


public class SideCardContainer : Node
{
    [Export] NodePath cardContainerPath;
    [Export] PackedScene cardScene;

    Control cardContainer;

    List<SideData> dataList = new List<SideData>();
    Dictionary<SideData, SideCard> cardMap = new Dictionary<SideData, SideCard>();

    public event EventHandler<SideData> sideDataIdUpdated; // TODO: Is it better that sideData property invoke this kind of events?
    public event EventHandler<SideData> sideDataColorUpdated;
    public event EventHandler<SideData> sideDeleted;
    public event EventHandler dataListStructureUpdated;

    public override void _Ready()
    {
        cardContainer = (Control)GetNode(cardContainerPath);

        Reset();
    }

    void Reset()
    {
        foreach(Node child in cardContainer.GetChildren()) // clear placeholder
            child.QueueFree();
        dataList.Clear();
        cardMap.Clear();
    }

    SideCard CreateCard()
    {
        var card = cardScene.Instance<SideCard>();
        
        card.upButtonPressed += OnCardUpButtonPressed;
        card.downButtonPressed += OnCardDownButtonPressed;
        card.addButtonPressed += OnCardAddButtonPressed;
        card.deleteButtonPressed += OnCardDeleteButtonPressed;

        card.idChanged += OnSideIdChanged;
        card.nameChanged += OnSideNameChanged;
        card.colorChanged += OnSideColorChanged;

        return card;
    }

    void OnSideIdChanged(object sender, string newText)
    {
        sideDataIdUpdated?.Invoke(this, ((SideCard)sender).data);
    }

    void OnSideNameChanged(object sender, string newText)
    {

    }

    void OnSideColorChanged(object sender, Color color)
    {
        sideDataColorUpdated?.Invoke(this, ((SideCard)sender).data);
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
    }

    void InsertData(SideData data, int index)
    {
        var card = AddCard(data);
        card.index = index;
        dataList.Insert(index, data);
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
        }
    }

    void EndStructureUpdate() // This method should be called on ending of UI callback handler.
    {
        ResetIndex();
        dataListStructureUpdated?.Invoke(this, EventArgs.Empty);
    }

    void ResetIndex()
    {
        for(var i=0; i<dataList.Count; i++)
        {
            var card = cardMap[dataList[i]];
            card.index = i;
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

            EndStructureUpdate();
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

            EndStructureUpdate();
        }
    }

    void OnCardAddButtonPressed(object sender, EventArgs _)
    {
        GD.Print("OnCardAddButtonPressed");
        var idx = ((SideCard)sender).index;

        // InsertData(new SideData(), idx);
        InsertData(new SideData(), idx + 1);

        EndStructureUpdate();
    }
    void OnCardDeleteButtonPressed(object sender, EventArgs _)
    {
        var card = (SideCard)sender;
        dataList.RemoveAt(card.index);
        cardMap.Remove(card.data);

        sideDeleted?.Invoke(this, card.data);

        if(dataList.Count == 0)
            AddData(new SideData());

        EndStructureUpdate();
    }

}
