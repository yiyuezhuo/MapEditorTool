using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class RegionInfoWindow : VBoxContainer
{
    [Export] NodePath baseColorRowPath;
    [Export] NodePath remapColorRowPath;
    [Export] NodePath colorIDRowPath;
    [Export] NodePath centerRowPath;
    [Export] NodePath scaleRowPath;
    [Export] NodePath areaRowPath;
    [Export] NodePath neighborsRowPath;
    [Export] NodePath sideColorRowPath;
    [Export] NodePath idRowPath;
    [Export] NodePath nameRowPath;

    ColorRow baseColorRow;
    ColorRow remapColorRow;
    LineEditRow colorIDRow;
    LineEditRow areaRow;
    LineEditRow centerRow;
    LineEditRow scaleRow;
    LineEditRow neighborsRow;
    ColorRow sideColorRow;
    LineEditRow idRow;
    LineEditRow nameRow;

    public override void _Ready()
    {
        baseColorRow = (ColorRow)GetNode(baseColorRowPath);
        remapColorRow = (ColorRow)GetNode(remapColorRowPath);
        colorIDRow = (LineEditRow)GetNode(colorIDRowPath);
        areaRow = (LineEditRow)GetNode(areaRowPath);
        centerRow = (LineEditRow)GetNode(centerRowPath);
        scaleRow = (LineEditRow)GetNode(scaleRowPath);
        neighborsRow = (LineEditRow)GetNode(neighborsRowPath);

        sideColorRow = (ColorRow)GetNode(sideColorRowPath);
        idRow = (LineEditRow)GetNode(idRowPath);
        nameRow = (LineEditRow)GetNode(nameRowPath);

        Hide();
    }

    public void SetData(Region region)
    {
        if(region == null)
        {
            Hide();
            return;
        }
        Show();

        // var neighborsText = string.Join(",", region.neighbors.Select(x => x.ToColorId()));
        // var centerText = $"{region.center.x.ToString("0.00")}, {region.center.y.ToString("0.00")}";

        baseColorRow.SetColor(region.baseColor);
        remapColorRow.SetColor(region.remapColor);
        colorIDRow.SetText(region.ToColorId().ToString());
        areaRow.SetText(region.area.ToString());
        centerRow.SetText(StringFor(region.center));
        scaleRow.SetText(StringFor(region.scale));
        neighborsRow.SetText(StringFor(region.neighbors));
        if(region.side != null)
            sideColorRow.SetData(region.side.id, region.side.color);
        else
            sideColorRow.SetData("", new Color(0,0,0,0)); // dummy
        idRow.SetText(region.id);
        nameRow.SetText(region.name);
    }

    static string StringFor(Vector2 v) => $"{v.x.ToString("0.00")}, {v.y.ToString("0.00")}";
    static string StringFor(IEnumerable<Region> regions) => string.Join(",", regions.Select(x => x.ToColorId()));
}
