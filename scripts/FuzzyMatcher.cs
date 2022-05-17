using System;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Match incoming regions with region extracted from map preprocessor using "feature vector" which consist of center and scale at this point.
/// Some region will be matched exactly, while other will match the one having minimal "distance".
/// FuzzyMatcher is used as the primary method to load json data, since minor revision to map is expected to happen many times, 
/// thus full exact matching is not that useful.
/// 
/// Since JSON introduce unavoidable precious error for float, all exact matching phases are dropped.
/// </summary>
public static class FuzzyMatcher
{
    public static List<Tuple<Region, Region, float>> Match(List<Region> regionList, List<Region> regionUpdateList)
    {
        var tupleList = new List<Tuple<Region, Region, float>>();
        foreach(var region in regionList)
            foreach(var regionUpdate in regionUpdateList)
            {
                var centerDelta = region.center - regionUpdate.center;
                var scaleDelta = region.scale - regionUpdate.scale;
                var dist = Math.Abs(centerDelta.x) + Math.Abs(centerDelta.y) + Math.Abs(scaleDelta.x) + Math.Abs(scaleDelta.y);

                tupleList.Add(new Tuple<Region, Region, float>(region, regionUpdate, dist));
            }

        tupleList.Sort((x,y) => x.Item3.CompareTo(y.Item3));

        var regionCloseSet = new HashSet<Region>();
        var regionUpdateCloseSet = new HashSet<Region>();
        var ret = new List<Tuple<Region, Region, float>>();
        foreach(var t in tupleList)
        {
            if(regionCloseSet.Contains(t.Item1) || regionUpdateCloseSet.Contains(t.Item2))
                continue;
            
            regionCloseSet.Add(t.Item1);
            regionUpdateCloseSet.Add(t.Item2);
            ret.Add(t);
        }

        return ret;
    }
    
}