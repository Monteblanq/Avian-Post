using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Countdown : MonoBehaviour //This script does a countdown for starting the game or resuming the game
{
    public ParticleSystem rain; //gets the reference for the rain particle system
    public GameObject tutorial; //gets the reference for the tutorial canvas (group of UI for the tutorial)
    public GameObject stars; //gets the reference for the spinning stars above the character's head when he is confused
    public GameObject sweat; //gets the referebnce for the sweat drops above the character's head when he is hit with a rock
    public bool isStarting = false; //determines whether to do the countdown
    public float whichNum = 3; //determines which number to display on the screen
    public GameObject[] winds; //gets the reference for the wind objects (weather effects) at the top, bottom, left and right
    public GameObject[] numbers; //gets the reference for the number objects (sprites, mainly)
    public Movement MrPeliPost; //gets the reference for the character movement script
    public Parallax[] BG; //gets the reference for the script that does the parallax scrolling effect

    // Update is called once per frame
    void Update()
    {
        if(isStarting) //if the flag is true, it means the game is about to start and the countdown happens
        {
            whichNum = whichNum - 1 *  Time.deltaTime;  //the variable determines which number to show, and it decreases 1 every second
            if(Mathf.Ceil(whichNum) == 3) //if the number is more than 2 but less than 3
            {
                numbers[0].SetActive(true); // number 3 is shown
            }
            else if(Mathf.Ceil(whichNum) == 2) // if the number is more than 1 but less than 2
            {
                numbers[0].SetActive(false); //number 3 is not shown
                numbers[1].SetActive(true); //number 2 is shown
            }
            else if (Mathf.Ceil(whichNum) == 1) // if the number is more than 0 but less than 1
            {
                numbers[1].SetActive(false); //number 2 is not shown
                numbers[2].SetActive(true); //number 1 is shown
            }
            else //if the number is anything else (only <= 0 is possible)
            {
                numbers[2].SetActive(false); //number 1 is not shown
                isStarting = false; //the countdown is disabled
                whichNum = 3; //the number to show is reset
                MrPeliPost.lockMovement = false; // the character can now move
                //the four parts of the background scrolls with the values defined in "Globals"
                BG[0].parallax = Globals.frontParallax;
                BG[1].parallax = Globals.backParallax;
                BG[2].parallax = Globals.frontParallax;
                BG[3].parallax = Globals.backParallax;
                if(Weather.currentWeather == (int)Weather.weather.RAIN) //resumes rain if the game was paused during rain
                {
                    rain.Play();
                }
                if(MrPeliPost.confused) //resumes the sound effect for confusion if the game was paused while the character was confused
                {
                    SoundPlayer.PlayOnRepeatConfused("flap");
                }
                MrPeliPost.MrPeliPost.gameObject.GetComponent<Animator>().enabled = true;  //continue whatever animation of the character was playing when it was paused
                MrPeliPost.MrPeliPost.velocity = Movement.savedVelocity; //resumes the trajectory of the character when it was paused
                foreach (Rigidbody2D instance in Spawner.instances) //iterates through all the instances that the Spawner currently has spawned
                {
                    if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing")) //in the case that the object is a building or a post office
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0); //resume the scrolling with the speed defined in "Globals"

                    }
                    else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing")) //in the case that the object is a helicopter or a tornado
                    {
                        Vector2 initVel; //variable to store the saved velocity of the object before it was paused
                        Spawner.initialVelocity.TryGetValue(instance, out initVel); //the Spawner script stores a map of saved velocities before pausing. Passing the instance ID as a key, it tries to get the saved velocity and saves it in initVel
                        instance.velocity = initVel; //sets the velocity and trajectory of the instance before it was paused
                        instance.gameObject.GetComponent<Animator>().enabled = true; //resume whatever animation the object had before it was playing.
                    }
                }
                foreach(Rigidbody2D instance in Deliver.packages) //iterates through the list of packages spawned
                {
                    Vector2 savedVel; //variable to store the saved velocity of the object before it was paused
                    Deliver.savedVelocity.TryGetValue(instance, out savedVel); //the Deliver script stores a map of saved velocities before pausing. Passing the instance ID as a key, it tries to get the saved velocity and saves it in savedVel
                    instance.velocity = savedVel; //sets the velocity and trajectory of the instance before it was paused
                    Deliver.savedVelocity.Clear(); //removes all the saved velocities
                    instance.gravityScale = 1.0f; //the object is again affected by gravity
                    instance.gameObject.GetComponent<Animator>().enabled = true; //resume whatever animation the object had before it was playing.
                }
                foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes) //iterates through the list of envelopes spawned
                {
                    instance.velocity = new Vector2(Globals.scrollSpeed, 0); //resume the scrolling with the speed defined in "Globals"
                }
                foreach (Rigidbody2D instance in ChildAI.rocks) //iterates through the list of rocks spawned
                {
                    Vector2 rockSavedVel; //variable to store the saved velocity of the object before it was paused
                    ChildAI.rockSavedVelocity.TryGetValue(instance, out rockSavedVel); //the ChildAI script stores a map of saved velocities before pausing. Passing the instance ID as a key, it tries to get the saved velocity and saves it in rockSavedVel
                    instance.gravityScale = 1.0f; //the rock is again affected by gravity
                    instance.velocity = rockSavedVel; //sets the velocity and trajectory of the instance before it was paused
                    ChildAI.rockSavedVelocity.Clear(); //removes all the saved velocities

                }
                winds[0].GetComponent<Animator>().enabled = true; //resumes the animation of the winds on the left
                winds[1].GetComponent<Animator>().enabled = true; //resumes the animation of the winds to the right
                winds[2].GetComponent<Animator>().enabled = true; //resumes the animation of the winds at the bottom
                winds[3].GetComponent<Animator>().enabled = true; //resumes the animation of the winds at the top
                stars.GetComponent<Animator>().enabled = true; //resumes the animation of the stars spinning
                sweat.GetComponent<Animator>().enabled = true; //resumes the animation of the sweat coming out of the head
                // set the game status as active
                Movement.gameActive = true;
                Spawner.gameActive = true;
                EnvelopeSpawner.gameActive = true;
                ChildSpawner.gameActive = true;
                Spawner.gameStarting = false;
                Events.gameActive = true;
                tutorial.SetActive(false); //disable the tutorial UI
            }
        }
    }
}
