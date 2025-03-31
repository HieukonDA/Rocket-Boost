using System.Collections.Generic;
using UnityEngine;

public class LevelVisualizer : ILevelVisualizer
{
    public void VisualizeSegments(List<Segment> segments)
    {
        for (int i = 0; i < segments.Count; i++)
        {
            Color color = i == 0 ? Color.green : (i == segments.Count - 1 ? Color.red : Color.yellow);
            Debug.DrawLine(segments[i].start, segments[i].end, color, 30f);
        }
    }
}