using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HIT_X { Left, Mid, Right, None }
public enum HIT_Y { Up, Mid, Down, None }
public enum HIT_Z { Front, Mid, Back, None }


public class CharacterCollider : MonoBehaviour
{
    public CharacterController characterController;

    public HIT_X hitX = HIT_X.None;
    public HIT_Y hitY = HIT_Y.None;
    public HIT_Z hitZ = HIT_Z.None;

    public HIT_X GetHitX(Collider col)
    {
        Bounds char_bounds = characterController.bounds;
        Bounds col_bounds = col.bounds;

        /*
        float minX = Mathf.Min(char_bounds.min.x, col_bounds.min.x);
        float maxX = Mathf.Max(char_bounds.max.x, col_bounds.max.x);
        float avgX = (minX + maxX) / 2 - col_bounds.min.x;

        
        if (avgX > col_bounds.size.x - 0.33f)
            return HIT_X.Right;
        if (avgX < 0.33f)
            return HIT_X.Left;
        return HIT_X.Mid;
        */

        float charCenterX = char_bounds.center.x;
        float colCenterX = col_bounds.center.x;

        float relativeX = charCenterX - colCenterX;

        if (relativeX > col_bounds.extents.x / 3)
            return HIT_X.Right;
        if (relativeX < -col_bounds.extents.x / 3)
            return HIT_X.Left;
        return HIT_X.Mid;
    }

    public void OnCharacterColliderHit(Collider col)
    {
        hitX = GetHitX(col);

        Debug.Log("OnCharacterColliderHit: " + hitX);
    }

    public void ResetHit()
    {
        hitX = HIT_X.None;
        hitY = HIT_Y.None;
        hitZ = HIT_Z.None;

        Debug.Log("ResetHit");
    }
}
