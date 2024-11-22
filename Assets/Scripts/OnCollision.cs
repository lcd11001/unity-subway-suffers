using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public Character character;

    public HitPositionDetector detector;

    public List<string> ignoreTags = new List<string> { "Player", "Ground" };

    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreTags.Contains(collision.transform.tag))
        {
            //Debug.Log("Ignoring collision with " + collision.collider.name + " tag " + collision.collider.tag);
            return;
        }

        Debug.Log("OnCollisionEnter: " + collision.collider.name + " tag " + collision.collider.tag);

        ContactPoint contact = collision.GetContact(0);
        var hitPosition = detector.GetHitPosition(collision.collider, contact.point);

        character.OnDeath(hitPosition.hitX, hitPosition.hitY, hitPosition.hitZ);
    }

}
