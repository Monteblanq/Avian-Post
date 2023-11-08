using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSpawner : MonoBehaviour
{
    public Rigidbody2D kid; //set the child entity prefab (predefined objects for spawning)
    static public List<Rigidbody2D> children = new List<Rigidbody2D>(); //maintains a list of children spawned for spawning and despawning purposes, and to maintain references to those children objects
    public static bool gameActive = false; //a flag that determines if the game is still active.
    public float chance = 0.4f; //chance of a child spawning after a certain time interval has passed (41%)
    public float interval = 7f; //an interval of time that a child may or may not spawn
    private float time = 0.0f; //time step (if this number is >= to interval, a child may or may not spawn
    Vector2 screenBounds;
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //determines the screen boundaries
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive == true) //if the game is still active
        {
            time += 1.0f * Time.deltaTime;  //add the time step 
            if (time >= interval) //if the time step is more than a certain interval then the block is excuted
            {
                float rollForChance = Random.Range(0.0f, 1.0f); //generate a random number
                if (rollForChance <= chance) //if the random number is less or equal than the chance determined the spawn a child
                {
                    Rigidbody2D child = Instantiate(kid); //spawn a child
                    children.Add(child); //add the spawned child in the list of children spawned
                    child.transform.position = new Vector2(screenBounds.x * 2, -4.448f); // spawn the child out of the screen on the right
                    child.velocity = new Vector2(Globals.scrollSpeed, 0f); //the child will scroll with the camera
                }
                time = 0; //reset the time step
            }
        }
        else
        {
            foreach (Rigidbody2D instance in children) //if the game has ended stop all children (from scrolling)
            {
                instance.velocity = Vector3.zero;
            }
        }
    }
}
