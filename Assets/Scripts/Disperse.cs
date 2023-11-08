using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disperse : MonoBehaviour //script made for the sole purpose of holding a function that causes tornadoes to be destroyed on impact with the character after an animation.
{
    void dispersed() //function removes the tornado from the list of objects spawned and destroys it after an animation is played
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Spawner.instances.Remove(this.gameObject.GetComponent<Rigidbody2D>());
        Spawner.initialVelocity.Remove(this.gameObject.GetComponent<Rigidbody2D>());
        Destroy(this.gameObject);
    }
}
