using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour //script that solely houses functions that play during the explosion animation for the helicopter
{

    void dieByExplosion() //destroy the object after the animation finishes
    {

        Destroy(this.gameObject);
    }

    void exploded() //the object collider is disabled, so it doesn't explode again
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void stopMovement() //the movement of the helicopter stops
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; //the helicopter stops scrolling
        Spawner.instances.Remove(this.gameObject.GetComponent<Rigidbody2D>()); //remove the helicopter from the spawned object list
        Spawner.initialVelocity.Remove(this.gameObject.GetComponent<Rigidbody2D>()); // remove the helicopter's stored starting velocity from the list
    }
}
