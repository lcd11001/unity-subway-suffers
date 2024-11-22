using UnityEngine;

public enum HIT_X { None, Left, Mid, Right }
public enum HIT_Y { None, Up, Mid, Down, Low }
public enum HIT_Z { None, Front, Mid, Back }

// Create a struct to hold hit position data
[System.Serializable]
public struct HitPositionData
{
    public HIT_X hitX;
    public HIT_Y hitY;
    public HIT_Z hitZ;

    public override string ToString()
    {
        return $"X: {hitX}, Y: {hitY}, Z: {hitZ}";
    }
}

public class HitPositionDetector : MonoBehaviour
{
    [SerializeField]
    private HitPositionData hitData;

    public HitPositionData GetHitPosition(Collider hitObject, Vector3 hitPoint)
    {
        if (hitObject == null)
        {
            Debug.LogWarning("HitPositionDetector: Collider is null!");
            return new HitPositionData { hitX = HIT_X.None, hitY = HIT_Y.None, hitZ = HIT_Z.None };
        }

        try
        {
            // Convert hit point to local space of the cube
            Vector3 localHitPoint = hitObject.transform.InverseTransformPoint(hitPoint);
            Bounds bounds = hitObject.bounds;

            // Calculate relative positions (0 to 1) with safety checks
            float xRatio = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.x, bounds.max.x, hitPoint.x));
            float yRatio = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.y, bounds.max.y, hitPoint.y));
            float zRatio = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.z, bounds.max.z, hitPoint.z));

            hitData = new HitPositionData
            {
                hitX = GetXPosition(xRatio),
                hitY = GetYPosition(yRatio),
                hitZ = GetZPosition(zRatio)
            };

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"Hit Position: {hitData}");
            DrawDebugVisualization(hitObject, hitPoint);
#endif

            return hitData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in HitPositionDetector: {e.Message}");
            return new HitPositionData { hitX = HIT_X.None, hitY = HIT_Y.None, hitZ = HIT_Z.None };
        }
    }

    private HIT_X GetXPosition(float ratio)
    {
        const float leftThreshold = 0.33f;
        const float rightThreshold = 0.66f;

        if (ratio < leftThreshold) return HIT_X.Left;
        if (ratio < rightThreshold) return HIT_X.Mid;
        return HIT_X.Right;
    }

    private HIT_Y GetYPosition(float ratio)
    {
        const float lowThreshold = 0.25f;
        const float downThreshold = 0.45f;
        const float midThreshold = 0.65f;

        if (ratio < lowThreshold) return HIT_Y.Low;
        if (ratio < downThreshold) return HIT_Y.Down;
        if (ratio < midThreshold) return HIT_Y.Mid;
        return HIT_Y.Up;
    }

    private HIT_Z GetZPosition(float ratio)
    {
        const float frontThreshold = 0.33f;
        const float midThreshold = 0.66f;

        if (ratio < frontThreshold) return HIT_Z.Front;
        if (ratio < midThreshold) return HIT_Z.Mid;
        return HIT_Z.Back;
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void DrawDebugVisualization(Collider hitObject, Vector3 hitPoint)
    {
        // Draw hit point
        Debug.DrawLine(hitPoint - Vector3.one * 0.1f, hitPoint + Vector3.one * 0.1f, Color.red, 2f);
        Debug.DrawLine(hitPoint - new Vector3(-0.1f, 0.1f, 0.1f), hitPoint + new Vector3(-0.1f, 0.1f, 0.1f), Color.red, 2f);
    }
#endif
}