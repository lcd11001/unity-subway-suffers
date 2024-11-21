using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public CharacterCollider characterCollider;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter: " + collision.collider.name + " tag " + collision.collider.tag);
        if (collision.transform.tag == "Player")
        {
            return;
        }
        characterCollider.OnCharacterColliderHit(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit: " + collision.collider.name + " tag " + collision.collider.tag);
        if (collision.transform.tag == "Player")
        {
            return;
        }
        characterCollider.ResetHit();
    }
}
