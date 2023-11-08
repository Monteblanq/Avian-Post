using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeSpawner : MonoBehaviour //A script that is responsible for spawning envelopes (collectables) and managing them.
{
    public Rigidbody2D envelope; //gets a reference to an envelope prefab (predefined objects for spawning)
    public Collider2D[] colliders; //variable to store colliders so that the envelope does not overlap with any other object
    static public List<Rigidbody2D> envelopes = new List<Rigidbody2D>(); //maintains a list of envelopes for spawning and despawning as well as maintaining the references of them.
    private Vector2 spawnPos; //position of spawning
    private float chance = 0.8f; //chance to spawn when it is time to spawn (81%)
    private float envelopeBufferTime = 0f; //time step for determining the time to spawn envelopes if envelopeBufferTime >= envelopeBuffer, then there is a chance an envelope will be spawned
    public float envelopeBuffer = 1f; //time threshold for spawning a envelope
    private Vector2 screenBoundaries; //to store the screen boundaries for spawning
    public static bool gameActive = false; //determines if the game is still active
    void Start()
    {
        screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //determines the screen boundaries
    }
    void spawnEnvelope() //function to spawn the envelope
    {
        spawnPos = new Vector2(screenBoundaries.x * 2, Random.Range(-3.8f, 3.9f)); //spawn position of the envelope. It always spawns at the same x location, but a random y location
        while (!PreventSpawnOverlap(spawnPos)) //checks if it collides with anything at the spawn point, if it overlaps, it will choose a different y location
        {
            spawnPos = new Vector2(screenBoundaries.x * 2, Random.Range(-3.8f, 3.9f));
        }
        float spawnOrNot = Random.Range(0f, 1f); //a random number to determine if the envelope spawns based on the chance
        if (spawnOrNot <= chance) //if the number is less than the chance number, then spawn envelope
        {
            Rigidbody2D envel = Instantiate(envelope) as Rigidbody2D; //spawn the envelope
            envelopes.Add(envel); // add the spawned envelope in the list of envelopes
            envel.transform.position = spawnPos; //set the location of the envelope at the checked location.
            envel.velocity = new Vector2(Globals.scrollSpeed, 0); //allow it to scroll
        }

    }
    bool PreventSpawnOverlap(Vector3 spawnPos) //checks if the spawn position overlaps with any other object
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, 1000f, 0); //1000 units in radius from the camera (middle of the screen), find all colliders in that circle

        for(int i = 0; i < colliders.Length; i++) //iterate through all the colliders
        {
            Vector3 centerPoint = colliders[i].bounds.center; //finds the center point of the collider
            float width = colliders[i].bounds.extents.x; //find the half width of the collider
            float height = colliders[i].bounds.extents.y; //find the half height of the collider

            //finds the bounding box of the collider, for all 4 cardinal directions
            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float upperExtent = centerPoint.y + height;
            float lowerExtent = centerPoint.y - height;

            //AABB collision check. If it collides then return false
            if(spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
            {
                if(spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                {
                    return false;
                }
            }
        }
        return true; //if it doesn't collide return true
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) //as long as the game is active
        {
            envelopeBufferTime += 1.0f * Time.deltaTime; //increase the time step
            if (envelopeBufferTime >= envelopeBuffer) //if the time step exceeds the threshold, it is now time to possibly spawn a envelope
            {
                //checks the weather to determine spawning speed, since wind can increase and decrease scrolling speed
                if (Weather.currentWeather == (int)Weather.weather.CLEAR) //if the weather is clear
                {
                    //spawn envelope as normal and reset the time step
                    spawnEnvelope();
                    envelopeBufferTime = 0;
                }
                else if (Weather.currentWeather == (int)Weather.weather.SOUTHWIND) //if the wind comes from the right
                {
                    if (envelopeBufferTime >= envelopeBuffer + 0.5) // needs a bit more time to spawn an envelope
                    {
                        spawnEnvelope();
                        envelopeBufferTime = 0;
                    }
                }
                else if (Weather.currentWeather == (int)Weather.weather.TAILWIND) //if wind comes from the left
                {
                    if (envelopeBufferTime >= envelopeBuffer - 0.5)// needs lesser time to spawn an envelope
                    {
                        spawnEnvelope();
                        envelopeBufferTime = 0;
                    }
                }
                
                
            }
        }
        else
        {
            foreach (Rigidbody2D instance in envelopes) //if game isn't active then set all the envelope to not scroll
            {
                instance.velocity = Vector3.zero;
            }
        }
    }
}
