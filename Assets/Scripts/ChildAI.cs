using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildAI : MonoBehaviour //this script determines the behavior of the child entity
{
    public Rigidbody2D kid; //reference to the a child entity prefab (predefined object to spawn)
    public Rigidbody2D rock; // reference to a rock entity prefab (predefined object to spawn)
    private bool thrownStone = false; //a child entity only throws one rock, this is a flag to determine whether the rock is thrown
    public static List<Rigidbody2D> rocks = new List<Rigidbody2D>(); //maintains a list of rocks thrown to manage the despawning and spawning of the rocks and to provide reference to the rock objects
    public static Dictionary<Rigidbody2D, Vector2> rockSavedVelocity = new Dictionary<Rigidbody2D, Vector2>(); //If the game is paused when the rock is thrown, this value resumes it trajectory.
    Vector2 screenBoundaries; //value to store the screen boundaries
    // Start is called before the first frame update
    void Start()
    {
        screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //establish the screen boundaries
    }

    // Update is called once per frame
    void Update()
    {
        if (kid != null) //if the prefab is set
        {
            if (kid.transform.position.x < screenBoundaries.x * 2) //As long as the child is toward the left of a whole screen length starting from the middle of the screen
            {
                if (Spawner.gameActive) //if the game is still active
                {
                    kid.velocity = new Vector2(Globals.scrollSpeed-2f, 0f); //child will move towards the left with the speed of the scroll
                }
                else
                {
                    kid.velocity = Vector2.zero; //the child will stop moving if the game has ended
                }
            }
            if (kid.transform.position.x < 1) // if the child is at the left side of a certain point (x unit = 1)
            {
                if (Spawner.gameActive) //as long as the game is still active
                {
                    kid.velocity = new Vector2(Globals.scrollSpeed, 0f); // the child will just scroll as normal
                    if (!thrownStone) //if the child hasn't thrown a stone then the stone is thrown
                    {
                        Rigidbody2D stone = Instantiate(rock) as Rigidbody2D; //create the rock object (spawn it)
                        rocks.Add(stone); //add the rock in the list of rocks thrown
                        stone.transform.position = transform.position; //the rock originates from the child, so its starting position is the child's position
                        stone.velocity = new Vector2(Globals.scrollSpeed - 3.0f, 10.0f); // it throws the rock in an arc as it is pulled by gravity
                        thrownStone = true; //set the flag that the child has thrown the rock so they don't throw it again
                    }
                }
                else
                {
                    kid.velocity = Vector2.zero; //the child will stop moving if the game has ended
                }
            }
        }
    }
}
