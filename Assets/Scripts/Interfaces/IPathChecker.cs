using System.Collections.Generic;
using UnityEngine;
public interface IPathChecker
{
    bool IsPathClear(Vector3 start, Vector3 end, float levelHeight, float levelDepth);
}