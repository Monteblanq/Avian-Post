using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour //script that is responsible for spawning obstacles such as buildings, tornadoes and helicopters and also a delivery spot (post office)
{
    public static float interval = 10f; //the time interval needed for a set of objects to spawn. This changes as time elapses
    public float baseInterval = 10f; //the base time interval needed for a set of objects to spawn. This is the initial base time and is mostly constant.
    public float totalTime = 0f; //total time elapsed that will affect how long it will take for a set of objects to spawn
    public float time = 0f; //time step that determines how much longer is need before it reaches the interval in which objects spawn (time >= interval means a set of objects can spawn)
    private float pOBufferTime = 0f; //the post office building spawns after a certain buffer time is reached, this is the time step. (pOBufferTime >= postOfficeBuffer then a post office is allowed to spawn)
    public float chanceForOne = 0.5f; //chance that one object will spawn at the same time
    public float chanceForTwo = 0.3f; //chance that two objects will spawn at the same time
    public float chanceForThree = 0.2f; //chance that three objects will spawn at the same time
    //the chances should add up to 1.0f (100%)
    public static bool ended = false; //a flag that determines if the game has ended
    public Rigidbody2D[] objects; //reference for object prefabs (predefined objects for spawning)
    public float postOfficeBuffer = 20f; //buffer time threshold that allows a post office to spawn. If the time step is higher or equal to this number then a post office is allowed to spawn.
    public float aerialSpeedMax = 8f; //helicopters can have variable horizontal speed. This is the upper limit.
    private List<int> verticalPosNums = new List<int>(); //a list that is used to determine which tier a flying object will spawn (top, middle and bottom)
    private List<int> numbers = new List<int>(); // in the case that 2 or 3 objects will spawn at a time, this make sures there are no duplicate objects spawning, since there are 4 different things to spawn.
    static public List<Rigidbody2D> instances = new List<Rigidbody2D>(); //stores all the spawned objects in a list for spawning and despawning, as well as maintaining references for the objects
    static public Dictionary<Rigidbody2D, Vector2> initialVelocity = new Dictionary<Rigidbody2D,Vector2>(); //in the case that the game resumes from a pause, this map allows the velocity of the objects prior to pausing to be saved and reestablished to the respective objects when the game is resumed.
                                                                                                            //Key: instance ID, Value: stored velocity
    public static bool gameStarting = true; //initially the game is starting but game is not active. This ensures the end sequence doesn't play while the game is not active and identifies as the game starting up rather than ending
    public static bool gameActive = false; //Initially the game is no active until the countdown, so spawning is disabled.
    private float verticalHeight; //determines which teir the object will spawn at (top, middle, bottom row)
    private Vector2 screenBoundaries; //determines the screen boundaries so that it spawns a distance relative to the edge of the screen
    private Vector2 velocityForObs; //determines the velocity of the object when it is spawned



    // Start is called before the first frame update
    void Start()
    {
        screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //determines the screen boundaries
    }

    void spawnObjects() //responsible for determining what to spawn and prepares its data
    {

        float chance = Random.Range(0f, 1.0f); //determines whether to spawn one, two or three objects at one time. 0-0.5 is one, 0.5-0.8 is two, 0.8-1 is three
        if(chance< chanceForOne) //if the value is less than 0.5
        {

            int random = Random.Range(0, 4); //determines what object to spawn
            int verticalRandom = Random.Range(0, 2); //determines which tier to spawn the object at (if it is a flying object) 0 is top row, 1 is middle row
            if (random == 0) //spawn a building
            {
                verticalHeight = -2.874f; //initialise the set vertical position (bottom row)
                velocityForObs = new Vector2(Globals.scrollSpeed, 0); //initialise object velocity when spawned
            }
            else if(random == 1) //spawn a post office
            {
                verticalHeight = -2.321f; //initialise the set vertical position (bottom row)
                velocityForObs = new Vector2(Globals.scrollSpeed, 0);  //initialise object velocity when spawned
            }
            else if(random == 2) //spawn a helicopter
            {
                if (verticalRandom == 0) //spawn in the top row
                {
                    if (!verticalPosNums.Contains(verticalRandom)) //if something else is not already in the top row
                    {
                        verticalHeight = 3.57f; //initialise the set vertical position (top row)
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0); //initialise object velocity when spawned, flying object have variable horizontal speed
                    }
                    else //if something else is already on the top row, then spawn it in the middle row
                    {
                        verticalHeight = 0.99f; //initialise the set vertical position (middle row)
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0); //initialise object velocity when spawned, flying object have variable horizontal speed
                    }
                    
                }
                else  if (verticalRandom == 1) //spawn in the middle row
                {
                    if (!verticalPosNums.Contains(verticalRandom)) //if something else is not already in the middle row
                    {
                        verticalHeight = 0.99f; //initialise the set vertical position (middle row)
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0); //initialise object velocity when spawned, flying object have variable horizontal speed
                    }
                    else //if something else is already in the middle row, spawn on the top row
                    {
                        verticalHeight = 3.57f;  //initialise the set vertical position (top row)
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0); //initialise object velocity when spawned, flying object have variable horizontal speed
                    }

                }
            }
            else //spawn a tornado
            {
                //same process as above
                if (verticalRandom == 0)
                {
                    if (!verticalPosNums.Contains(verticalRandom))
                    {
                        verticalHeight = 3.57f;
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                    }
                    else
                    {
                        verticalHeight = 0.99f;
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                    }

                }
                else if (verticalRandom == 1)
                {
                    if (!verticalPosNums.Contains(verticalRandom))
                    {
                        verticalHeight = 0.99f;
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                    }
                    else
                    {
                        verticalHeight = 3.57f;
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                    }

                }
            }
            if (random == 1 && pOBufferTime >= postOfficeBuffer) //if preparations have been done to spawn a post office (verticla height, starting velocity) and a certain buffer time has already passed
            {
                Instantiating(random); //spawn the post office officially.
                pOBufferTime = 0; //it will take some more time before the next post office can spawn (postOfficeBuffer units of time)
            }
            else if(random != 1) //if it is not a post office that is prepared to spawn
            {
                Instantiating(random); //officially spawn whatever object is prepared to spawn.
            }
            else //if a post office is prepared to spawn but it is not time yet for another to be able to be spawned
            {
                while(random == 1) //pick another random object and repeat the object picking process again and again until it is not a post office.
                {
                    //same process as above
                    random = Random.Range(0, 4);
                    verticalRandom = Random.Range(0, 2);
                    if (random == 0)
                    {
                        verticalHeight = -2.874f;
                        velocityForObs = new Vector2(Globals.scrollSpeed, 0);
                    }
                    else if (random == 1)
                    {
                        
                        if (verticalRandom == 0)
                        {
                            if (!verticalPosNums.Contains(verticalRandom))
                            {
                                verticalHeight = 3.57f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }
                            else
                            {
                                verticalHeight = 0.99f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }

                        }
                        else if (verticalRandom == 1)
                        {
                            if (!verticalPosNums.Contains(verticalRandom))
                            {
                                verticalHeight = 0.99f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }
                            else
                            {
                                verticalHeight = 3.57f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }

                        }
                    }
                    else if (random == 2)
                    {
                        if (verticalRandom == 0)
                        {
                            if (!verticalPosNums.Contains(verticalRandom))
                            {
                                verticalHeight = 3.57f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }
                            else
                            {
                                verticalHeight = 0.99f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }

                        }
                        else if (verticalRandom == 1)
                        {
                            if (!verticalPosNums.Contains(verticalRandom))
                            {
                                verticalHeight = 0.99f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }
                            else
                            {
                                verticalHeight = 3.57f;
                                velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                            }

                        }
                    }
                    else
                    {
                        verticalHeight = 0.99f;
                        velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                    }
                }
                Instantiating(random); //since this is sure to spawn something that isn't a post office, it can be just spawned offcially immediately.
            }

        }
        else if (chance < chanceForOne + chanceForTwo) //if two object spawn at once (less than 0.8 but more than 0.5)
        {
            int instNumber = 0; //number of instances spawned
            while(instNumber < 2) //as long as the number of instances spawned is less than 2
            {
                //same process as above in selecting what to spawn, except this will keep running until 2 object has officially spawned
                int random = Random.Range(0, 4);
                int verticalRandom = Random.Range(0, 2);
                if (random == 0)
                {
                    verticalHeight = -2.874f;
                    velocityForObs = new Vector2(Globals.scrollSpeed, 0);
                }
                else if (random == 1)
                {
                    verticalHeight = -2.321f;
                    velocityForObs = new Vector2(Globals.scrollSpeed, 0);
                }
                else if (random == 2)
                {   
                    if (verticalRandom == 0)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                    else if (verticalRandom == 1)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                }
                else
                {
                    if (verticalRandom == 0)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                    else if (verticalRandom == 1)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                }
                if (random == 1 && pOBufferTime >= postOfficeBuffer)
                {
                    if (!numbers.Contains(random)) //if this object has not yet been spawned at this time (that is, to avoid duplicates)
                    {
                        Instantiating(random); //spawn the object officially
                        instNumber++; //number of instances spawned increases
                        
                    }
                    pOBufferTime = 0;
                }
                else if (!numbers.Contains(random) && random != 1) //if this object has not yet been spawned yet at this time (that is, to avoid duplicates) and it is not a post office
                {
                    Instantiating(random); //spawn the object officially
                    instNumber++; // number of instances spawned increase
                    if (random == 2 || random == 3) //if the object is a flying object
                    {
                        verticalPosNums.Add(verticalRandom); //record the row in which the object has spawned so that another object cannot spawn on the same row
                    }
                }

            }
        }
        else if ( chance < chanceForOne + chanceForTwo + chanceForThree) //if three objects spawn at once (if less than 1.0 but more than 0.8)
        {
            //does the same process as above, except this time it runs until 3 objects have been officially spawned
            int instNumber = 0;
            while (instNumber < 3)
            {
                int random = Random.Range(0, 4);
                int verticalRandom = Random.Range(0, 2);
                if (random == 0)
                {
                    verticalHeight = -2.874f;
                    velocityForObs = new Vector2(Globals.scrollSpeed, 0);
                }
                else if (random == 1)
                {
                    verticalHeight = -2.321f;
                    velocityForObs = new Vector2(Globals.scrollSpeed, 0);
                }
                else if (random == 2)
                {
                    if (verticalRandom == 0)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                    else if (verticalRandom == 1)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                }
                else
                {
                    if (verticalRandom == 0)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                    else if (verticalRandom == 1)
                    {
                        if (!verticalPosNums.Contains(verticalRandom))
                        {
                            verticalHeight = 0.99f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }
                        else
                        {
                            verticalHeight = 3.57f;
                            velocityForObs = new Vector2(-Random.Range(-Globals.scrollSpeed, aerialSpeedMax), 0);
                        }

                    }
                }
                if (random == 1 && pOBufferTime >= postOfficeBuffer)
                {
                    if (!numbers.Contains(random))
                    {
                        Instantiating(random);
                        instNumber++;
                    }
                    pOBufferTime = 0;
                }
                else if (!numbers.Contains(random) && random != 1)
                {
                    Instantiating(random);
                    instNumber++;
                    if (random == 2 || random == 3)
                    {
                        verticalPosNums.Add(verticalRandom);
                    }
                }

            }
        }
        //clear the intermediate lists so that it is ready for the next spawn time
        numbers.Clear();
        verticalPosNums.Clear();
    }

    void Instantiating(int random) //spawns the object officially
    {
        Rigidbody2D anObstacle = Instantiate(objects[random]) as Rigidbody2D; //spawns the object based on its ID (0 for building, 1 for post office, 2 for helicopter, 3 for tornado)
        instances.Add(anObstacle); //add this object to the list of objects spawned
        anObstacle.transform.position = new Vector2(screenBoundaries.x * 2, verticalHeight); //place the object in the word off screen to the right with a prepared height
        anObstacle.velocity = velocityForObs; //set the initial velocity for the object
        initialVelocity.Add(anObstacle, anObstacle.velocity); //save the initial velocity for the object in case the velocities need to be reestablished after a resume from a pause
        anObstacle.gravityScale = 0; //these objects are not affected by gravity as ground objects are already planted on the ground and flying objects fly
        numbers.Add(random); //this object has been spawned at this time, so it cannot be spawned again.
        if (random == 0) //If a building is spawned, then a post office cannot be spawned, vice versa
        {
            numbers.Add(1);
        }
        else if (random == 1)
        {
            numbers.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive == true) //as long as the game is still active
        {
            time += 1.0f * Time.deltaTime; //increase the time step to determine when to spawn an object
            totalTime += 1.0f * Time.deltaTime; //increase the time elapsed so that the time interval between object spawning decreases as this increases
            pOBufferTime += 1.0f * Time.deltaTime; //increase the time step to determine when a post office can be spawned
            interval = baseInterval - totalTime * 0.1f; //the time interval between object spawning is inversely propostional to the total time elapsed
            if (interval < 3.0f) //the time interval for object spawning cannot go lower than 3 units.
            {
                interval = 3.0f;
            }

            if (time >= interval) //if the time step has passed the time interval threshold then it is time to spawn an object
            {
                if (Weather.currentWeather == (int)Weather.weather.CLEAR) //if the weather is clear, then just spawn the objects as normal
                {
                    spawnObjects();
                    time = 0;
                }
                else if (Weather.currentWeather == (int)Weather.weather.SOUTHWIND) //if the wind is coming from the right, then it will take a bit longer to objects to spawn since the screen scrolls slower
                {
                    if (time >= interval + 1.0)
                    {
                        spawnObjects();
                        time = 0;
                    }
                }
                else if (Weather.currentWeather == (int)Weather.weather.TAILWIND) //if the wind is coming from the left, then it will be a bit faster for objects to spawn since the screen scrolls faster
                {
                    if (time >= interval - 1.0)
                    {
                        spawnObjects();
                        time = 0;
                    }
                }

            }


        }
        else if (!ended && !gameStarting) //if the game is not active, then has the game ended or is the game starting? Only when the game hasn't ended and the game isn't starting will this end sequence be run
        {
            //game is ending here, affecting all spawned objects.
            foreach (Rigidbody2D instance in instances) //for every object that is spawned
            {
                if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing")) //if the object is a building or a post office
                {
                    instance.velocity = Vector3.zero; //the object stays stationary
                    instance.isKinematic = true; //the object isn't affect by force anymore and becomes unmovable

                }
                else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing")) //if the object is a flying object
                {
                    instance.velocity = new Vector2(instance.velocity.x + (-Globals.scrollSpeed), 0); //it will fly forward without taking into account scrolling speed.
                }
                initialVelocity[instance] = instance.velocity; //the velocity minus the scroll speed is saved

            }
            ended = !ended; //the game has completely ended.
        }
    }
}
