using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Deliver : MonoBehaviour //A script that manages package delivering and spawning packages to be delivered to post offices
{
    public Rigidbody2D package; //gets the reference for the package entity prefab (predefined objects for spawning)
    public static List<Rigidbody2D> packages = new List<Rigidbody2D>(); //maintains a list of spawned packages for spawning and despawning and maintaining references to those objects
    public static Dictionary<Rigidbody2D, Vector2> savedVelocity = new Dictionary<Rigidbody2D, Vector2>(); //a map of saved velocities to resume the velocity of the package objects after the game is resumed from a pause. Key: instance ID, Value: saved velocity
    public Movement MrPeli; //gets the reference of the movement script for the character
    public TextMeshProUGUI numEnvelope; // gets the reference for the number of envelope HUD text
    public TextMeshProUGUI weight; // gets the reference for the weight HUD text

    public void drop() //function that will execute when the player drops the envelopes gathered (pressing a button)
    {
        if(Events.numberOfEnv > 0 && !Events.lost) //if the character has gathered at least one envelope and the player hasn't lost the game yet
        {
            Rigidbody2D packageVar = Instantiate(package) as Rigidbody2D; //spawn the package and store it in the variable
            packages.Add(packageVar); //add the package to the list of packages spawned
            foreach(Rigidbody2D instance in Spawner.instances) //iterate through the list of spawned obstacle instances
            {
                if(instance.CompareTag("FlyingObstacle")) //if the object is a helicopter
                {
                    Physics2D.IgnoreCollision(packageVar.GetComponent<BoxCollider2D>(), instance.GetComponent <BoxCollider2D>()); //ignore the collision of the helicopter and the package
                }
            }
            packageVar.transform.position = MrPeli.transform.position; //the package originates from the character so the package's position starts from the character
            packageVar.velocity = new Vector2(Globals.scrollSpeed, 0); //the package scrolls with the camera
            packageVar.gameObject.GetComponent<PackageBehaviour>().toBeDelivered = Events.numberOfEnv; //stores the number of envelopes gathered into the package itself by the use of the PackageBehaviour script
            SoundPlayer.PlaySound("drop"); //play the sound effect of dropping the package
            Events.numberOfEnv = 0; //reset the number of envelopes gather to 0
            MrPeli.weight = 1.0f; //reset the weight of the character
            //updating the relevant text
            numEnvelope.text = "x " + Events.numberOfEnv;
            weight.text = "x " + MrPeli.weight.ToString("F1");
        }
    }
}
