using UnityEngine;

public class PathChecker : IPathChecker
{
    public bool IsPathClear(Vector3 start, Vector3 end, float levelHeight, float levelDepth)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, end - start, out hit, Vector3.Distance(start, end)))
        {
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                return false;
            }
        }

        float[] heights = { levelHeight / 4f, levelHeight / 2f, 3 * levelHeight / 4f };
        foreach (float height in heights)
        {
            Vector3 midPoint = new Vector3((start.x + end.x) / 2f, height, levelDepth);
            if (!Physics.Raycast(start, midPoint - start, out hit, Vector3.Distance(start, midPoint)) &&
                !Physics.Raycast(midPoint, end - midPoint, out hit, Vector3.Distance(midPoint, end)))
            {
                return true;
            }
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                continue;
            }
        }

        return true;
    }
}