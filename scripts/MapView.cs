using Godot;
using System.Collections.Generic;
using System;

public class MapView : YYZ.MapKit.MapView<Region>
{
    // [Export] public NodePath mapShowerPath;

    [Export] NodePath labelContainerPath;
    [Export] PackedScene regionLabelScene;

    Node labelContainer;

    Dictionary<Region, RegionLabel> labelMap = new Dictionary<Region, RegionLabel>();
    public LabelMode labelMode;

    public override void _Ready()
    {
        base._Ready();

        labelContainer = (Node)GetNode(labelContainerPath);
    }

    public void CreateLabels(IEnumerable<Region> regions)
    {
        foreach(var region in regions)
        {
            var label = regionLabelScene.Instance<RegionLabel>();
            label.RectPosition = region.center; // problematic
            labelContainer.AddChild(label);
            labelMap[region] = label;

            label.Text = ""; // clear placeholder text
        }

        // SyncLabels();
    }

    void SyncLabels()
    {
        switch(labelMode)
        {
            case LabelMode.ID:
                foreach(var KV in labelMap)
                {
                    KV.Value.Show();
                    KV.Value.Text = KV.Key.id;
                }
                break;
            case LabelMode.Name:
                foreach(var KV in labelMap)
                {
                    KV.Value.Show();
                    KV.Value.Text = KV.Key.name;
                }
                break;
            case LabelMode.Hide:
                foreach(var KV in labelMap)
                    KV.Value.Hide();
                    // KV.Value.Text = "";
                break;
        }
    }

    public void OnLabelModeChanged(object sender, LabelMode labelMode)
    {
        if(labelMode == this.labelMode)
            return;
        
        this.labelMode = labelMode;
        SyncLabels();
    }

    public void OnRegionTextChanged(object sender, Region region)
    {
        switch(labelMode)
        {
            case LabelMode.ID:
                labelMap[region].Text = region.id;
                break;
            case LabelMode.Name:
                labelMap[region].Text = region.name;
                break;
            /*
            case LabelMode.Hide:
                break;
            */
        }
    }
}
