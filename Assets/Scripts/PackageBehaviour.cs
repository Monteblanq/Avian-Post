using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PackageBehaviour : MonoBehaviour //script that determines how the package behaves with other objects as well as store instance based data
{
    public TextMeshProUGUI deliveredEnv; //text HUD that reflects how many envelopes have been delivered
    public int toBeDelivered = 0; // the amount of envelopes that this package contains

    private void Start()
    {
        deliveredEnv = TextReference.itself; // gets the reference of the text HUD of the delivered envelopes (because prefabs are not allowed to directly refer to scene objects in the editor and must do so in runtime)
    }

    private void OnCollisionEnter2D(Collision2D collision) //if the package collides with something
    {
        if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Obstacle")) //if it collides with the floor or a building
        {
            if (!this.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Yippee")) //If the package isn't already delivered (that is, the animation of it isn't playing)
            {
                this.gameObject.GetComponent<Animator>().Play("PackageLand"); //play the animation of the package landing 

                SoundPlayer.PlaySound("land"); //play the sound of the package landing
                if (Spawner.gameActive) //if the game is still active
                {
                    this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Globals.scrollSpeed, 0); //the package scrolls as other objects
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) //if it touches a trigger
    {
        if(collision.CompareTag("Refreshing")) //if the trigger is a post office, the package is delivered
        {
            SoundPlayer.PlaySound("refresh"); //play the delivered sound
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false; //the package no longer collides
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Globals.scrollSpeed, 0); // it scrolls as usual
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f; //it doesn't get pull down by gravity
            this.gameObject.GetComponent<Animator>().Play("Yippee"); //play the animation of the package when it is delivered
            Events.numberDelivered += toBeDelivered; // add the score of delivered envelopes
            deliveredEnv.text = "x " + Events.numberDelivered; // update the text to reflect the correct amount

        }
    }

    public void killSelf() //function that runs when an animation finishes
    {
        Deliver.packages.Remove(this.GetComponent<Rigidbody2D>()); //remove the object from the list of packages
        Destroy(this.gameObject); //destroy the package (despawn)
    }
}
