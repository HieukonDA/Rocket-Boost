using System.Collections.Generic;

public interface ISegmentGenerator
{
    List<Segment> GenerateSegments(int levelNumber, float levelWidth, float levelHeight, float levelDepth);
}