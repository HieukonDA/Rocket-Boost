using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : ISegmentGenerator
    {
        public List<Segment> GenerateSegments(int levelNumber, float levelWidth, float levelHeight, float levelDepth)
        {
            List<Segment> segments = new List<Segment>();
            Vector3 currentPos = Vector3.zero;

            int segmentCount = Mathf.Clamp(levelNumber, 1, 3);
            for (int i = 0; i < segmentCount; i++)
            {
                Segment segment = new Segment();
                segment.start = currentPos;

                string direction = "Horizontal";
                if (i > 0)
                {
                    int randomDirection = UnityEngine.Random.Range(0, 3);
                    if (randomDirection == 1) direction = "Vertical";
                    else if (randomDirection == 2) direction = "Diagonal";
                }

                if (direction == "Horizontal")
                {
                    float segmentLength = levelWidth / segmentCount;
                    segment.end = new Vector3(currentPos.x + segmentLength, currentPos.y, levelDepth);
                }
                else if (direction == "Vertical")
                {
                    float segmentLength = levelHeight / 2f;
                    segment.end = new Vector3(currentPos.x, currentPos.y + segmentLength, levelDepth);
                }
                else
                {
                    float segmentLengthX = levelWidth / (segmentCount * 2f);
                    float segmentLengthY = levelHeight / (segmentCount * 2f);
                    segment.end = new Vector3(currentPos.x + segmentLengthX, currentPos.y + segmentLengthY, levelDepth);
                }

                segment.direction = direction;
                segments.Add(segment);
                currentPos = segment.end;

                Debug.Log($"Segment {i + 1}: Direction = {segment.direction}, Start = {segment.start}, End = {segment.end}");
            }

            Segment lastSegment = segments[segments.Count - 1];
            lastSegment.end = new Vector3(levelWidth - 2f, lastSegment.end.y, levelDepth);
            segments[segments.Count - 1] = lastSegment;

            return segments;
        }
    }