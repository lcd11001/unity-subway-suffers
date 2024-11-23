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
            // Get the object's transform
            Transform objectTransform = hitObject.transform;

            // Convert world hit point to local space
            Vector3 localHitPoint = objectTransform.InverseTransformPoint(hitPoint);

            // Get the object's local bounds
            Bounds localBounds = hitObject.bounds;
            Vector3 localCenter = objectTransform.InverseTransformPoint(localBounds.center);
            Vector3 localSize = localBounds.size;

            // Calculate local space boundaries
            Vector3 localMin = localCenter - localSize * 0.5f;
            Vector3 localMax = localCenter + localSize * 0.5f;

            // Calculate ratios in local space
            float xRatio = Mathf.InverseLerp(localMin.x, localMax.x, localHitPoint.x);
            float yRatio = Mathf.InverseLerp(localMin.y, localMax.y, localHitPoint.y);
            float zRatio = Mathf.InverseLerp(localMin.z, localMax.z, localHitPoint.z);

            // The Z-axis (forward) needs to be inverted because Unity's forward is positive Z
            // but we want "Front" to be the facing direction
            zRatio = 1f - zRatio;

            // Clamp ratios to handle edge cases
            xRatio = Mathf.Clamp01(xRatio);
            yRatio = Mathf.Clamp01(yRatio);
            zRatio = Mathf.Clamp01(zRatio);

            hitData = new HitPositionData
            {
                hitX = GetXPosition(xRatio),
                hitY = GetYPosition(yRatio),
                hitZ = GetZPosition(zRatio)
            };

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"Hit Position: {hitData} (Local Point: {localHitPoint}, Ratios: X={xRatio:F2}, Y={yRatio:F2}, Z={zRatio:F2})");
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
        const float lowThreshold = 0.15f;
        const float downThreshold = 0.5f;
        const float midThreshold = 0.85f;

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

    public HitPositionData ResetCollision()
    {
        hitData = new HitPositionData { hitX = HIT_X.None, hitY = HIT_Y.None, hitZ = HIT_Z.None };
        return hitData;
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void DrawDebugVisualization(Collider hitObject, Vector3 hitPoint)
    {
        // Draw hit point in world space
        Debug.DrawLine(hitPoint - Vector3.one * 0.1f, hitPoint + Vector3.one * 0.1f, Color.yellow, 2f);
        Debug.DrawLine(hitPoint - new Vector3(-0.1f, 0.1f, 0.1f), hitPoint + new Vector3(-0.1f, 0.1f, 0.1f), Color.yellow, 2f);

        // Draw object's forward direction
        Debug.DrawRay(hitObject.bounds.center, hitObject.transform.forward * 2f, Color.blue, 2f);

        // Draw local axes at hit point
        Debug.DrawRay(hitPoint, hitObject.transform.right * 0.5f, Color.red, 2f);
        Debug.DrawRay(hitPoint, hitObject.transform.up * 0.5f, Color.green, 2f);
        Debug.DrawRay(hitPoint, hitObject.transform.forward * 0.5f, Color.blue, 2f);
    }
#endif
}