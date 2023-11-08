using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Weather : MonoBehaviour //script responsible for all the wather effects in the game
{
    public enum weather //an enumerator to determine what weather patterns there are
    {
        CLEAR = 0, //nothing happens
        RAIN = 1, //vision obscured
        UPDRAFT = 2, //propulsion increased
        DOWNWIND = 3, //propulsiond decreased
        TAILWIND = 4, //scroll speed increased
        SOUTHWIND = 5 //scroll speed decreased
    }
    public static int currentWeather = (int) weather.CLEAR; //the intial weather is clear when the game starts
    public float chanceForWeather = 0.3f; //whenever the time for a weather pattern to occur there is a chance it won't occur (70% chance of not occuring)
    public float interval = 30; // the time interval between chances of weather patterns occuring
    public float time = 0; //time step to determine how much time is left before a chance of a weather pattern occuring (time >= interval means a weather pattern may occur)
    public float lasts = 10; //the amount of time that the weather pattern will last
    public float lastsTime = 0; //the time step and elapsed time of the weather pattern and determines when to end the weather pattern (lastsTime >= lasts means the weather pattern stops)
    public Movement mrPeliPost; //gets the script for the character movement as the weather patterns interfere with flying
    //references for the object and sprites for the wind in all four cardinal directions
    public GameObject windLeft;
    public GameObject windRight;
    public GameObject windDown;
    public GameObject windUp;
    public ParticleSystem rain; //gets the reference for the particle system in order to make rain drops appear
    public GameObject[] BG; //gets the reference for the background props as it interferes with scroll speed
    public GameObject sky; //gets the reference for the sky background as it changes colour when it rains
    public Light2D globalLight; //gets the reference for the global light that will decrease and make things dark when it rains
    public Light2D pointLight; //gets the reference for the point light will will illuminate the darkness when it rains in a radius around the character
    public bool transitioning; //the state in which the weather is transitioning to rain or from rain to create a transition effect
    public float alpha = 0; //determines how much the objects are tinted blue (higher alpha means how much the object is tinted blue and becomes darker)
    public float globalIntensity = 1; //initial global light is on full strength
    public float pointIntensity = 0; //initial point light is invisible
    void Update()
    {
        if (Spawner.gameActive) //if the game is still active
        {
            float chance = Random.Range(0f, 1.0f); //determines whether the weather pattenr will occur when a certain time interval has passed
            if (chance < chanceForWeather && Spawner.interval < 5.0f && time >= interval) // if the number generated is less than the percentage chance of the weather pattern happening
                                                                                          // ...and a certain amount of time has passed (so that the time interval between spawning objects is < 5 units)
                                                                                          // as well as the fact that it is time for a chance the weather pattern to occur
            {
                int whichWeather = Random.Range(0, 5); //determines which weather pattern to happen
                time = 0; //resets the time step after the weather pattern occurs, so it takes time again before the next pattern occurs
                lastsTime = 0; //the time in which the pattern lasts is reset
                if (whichWeather == 0) //in case of rain
                {
                    currentWeather = (int)weather.RAIN; //set the current weather to rain
                    rain.Play(); // play the particle system to cause rain drops to fall
                    SoundPlayer.PlayOnRepeatWeather("rain"); //play the rain sound effect
                }
                else if (whichWeather == 1) //in case the wind co0mes from the bottom
                {
                    currentWeather = (int)weather.UPDRAFT; //set the current weather to updraft
                    windDown.SetActive(true); // makes the wind sprite at the bottom appear and animate
                    mrPeliPost.propelVelocity = Globals.propulsion + 2.0f; //increase the amount in which the character flies up with each tap
                    SoundPlayer.PlayOnRepeatWeather("wind"); //play the wind sound effect
                }
                else if (whichWeather == 2) //in case the wind comes from the top
                {
                    currentWeather = (int)weather.DOWNWIND; //set the current weather to downwind
                    windUp.SetActive(true); // makes the wind sprite at the top appear and animate
                    mrPeliPost.propelVelocity = Globals.propulsion - 2.0f; //decrease the amount in which the character flies up with each tap
                    SoundPlayer.PlayOnRepeatWeather("wind"); //play the wind sound effect
                }
                else if (whichWeather == 3) //in the case the wind comes from the left
                {
                    currentWeather = (int)weather.TAILWIND; //set the current weather to tailwind
                    windLeft.SetActive(true);  // makes the wind sprite at the left appear and animate
                    float prevVelocity = Globals.scrollSpeed;  //save the previous velocity to subtract from flying objects speed
                    Globals.scrollSpeed = -8.0f; // increases the scroll speed (moving to the left)
                    foreach (Rigidbody2D instance in Spawner.instances) //for all the objects spawned so far
                    {
                        if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing")) //in the case of buildings and post offices
                        {
                            instance.velocity = new Vector2(Globals.scrollSpeed, 0); // reset the scrolling speed to the new speed
                        }
                        else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing")) //in the case of flying objects
                        {
                            instance.velocity = new Vector2(instance.velocity.x - prevVelocity + Globals.scrollSpeed, 0); //reset the scrolling speed to adjust to the new speed
                            Spawner.initialVelocity[instance] = instance.velocity; //save the initial velocity of the objects so that it can be reset when the weather effect ends
                        }
                    }
                    foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes) //for each envelope that is spawned
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0); //reset the scrolling speed to the new speed
                    }
                    foreach(Rigidbody2D instance in Deliver.packages) //for each package that is spawned
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0); //reset the scrolling speed to the new speed
                    }
                    //the background props also adjusts to the new scroll speed by adding a certain about to the parallax scroll
                    BG[0].GetComponent<Parallax>().parallax = Globals.frontParallax + 1.5f;
                    BG[1].GetComponent<Parallax>().parallax = Globals.backParallax + 0.5f;
                    BG[2].GetComponent<Parallax>().parallax = Globals.frontParallax + 1.5f;
                    BG[3].GetComponent<Parallax>().parallax = Globals.backParallax + 0.5f;
                    SoundPlayer.PlayOnRepeatWeather("wind"); //play the wind sound effect
                }
                else if (whichWeather == 4) //in the case the wind comes from the right
                {
                    currentWeather = (int)weather.SOUTHWIND; //set the current weather to southwin
                    windRight.SetActive(true); // makes the wind sprite at the right appear and animate
                    float prevVelocity = Globals.scrollSpeed; //save the previous velocity to subtract from flying objects speed
                    Globals.scrollSpeed = -2.0f; // increases the scroll speed (moving to the left)
                    //same as tailwind except whatever is increased is decreased and whatever is decreased is increased.
                    foreach (Rigidbody2D instance in Spawner.instances)
                    {
                        if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing"))
                        {
                            instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                        }
                        else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing"))
                        {
                            instance.velocity = new Vector2(instance.velocity.x - prevVelocity + Globals.scrollSpeed, 0);
                            Spawner.initialVelocity[instance] = instance.velocity;
                        }
                    }
                    foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes)
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                    }
                    foreach (Rigidbody2D instance in Deliver.packages)
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                    }
                    BG[0].GetComponent<Parallax>().parallax = Globals.frontParallax - 1.5f;
                    BG[1].GetComponent<Parallax>().parallax = Globals.backParallax - 0.5f;
                    BG[2].GetComponent<Parallax>().parallax = Globals.frontParallax - 1.5f;
                    BG[3].GetComponent<Parallax>().parallax = Globals.backParallax - 0.5f;
                    SoundPlayer.PlayOnRepeatWeather("wind");  //play the wind sound effect
                }


            }
            else if (time >=  interval) //even if the weather pattern doesn't happen, it will take another time interval before another chance of it happening comes again
            {
                time = 0;
            }
            if (currentWeather == (int)weather.RAIN && !transitioning) //when it goes from clear to rain it starts to transition. (transition is a toggle, transition = false is for clear to rain, transition = true is for rain to clear)
            {

                if (globalIntensity > 0.5) //if the global light intensity is more than half
                {
                    globalIntensity -= Time.deltaTime/2; //decrease the global light intensity until it is half
                    if(globalIntensity < 0.5)
                    {
                        globalIntensity = 0.5f;
                    }
                }
                if (pointIntensity < 1) //if the point light intensity is less than 1
                {
                    pointIntensity += Time.deltaTime/2; //increase the point light intensity until it is full
                    if(pointIntensity > 1)
                    {
                        pointIntensity = 1;
                    }
                }
                else
                {
                    transitioning = !transitioning; //the weather pattern is now read to transition back to clear
                }
                if (alpha < 1) //if the tint amount is less than full
                {
                    alpha += Time.deltaTime/2; //then it increases the tint until it is in full effect
                    if(alpha > 1)
                    {
                        alpha = 1;
                    }
                }
                
                //update the tint in the shaders of the background props
                BG[0].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                BG[1].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                BG[2].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                BG[3].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                sky.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                //set the light sources' intensity to the ones updated above
                globalLight.intensity = globalIntensity;
                pointLight.intensity = pointIntensity;

            }

            //in the case of retransitioning back to clear weather it is the same as the above, except in reverse
            if(currentWeather == (int)weather.CLEAR && transitioning) 
            {
                    if (globalIntensity < 1) //if the global light intensity is less than half
                    {
                        globalIntensity += Time.deltaTime/2; //increase the global light intensity until it is full
                    if (globalIntensity > 1)
                        {
                            globalIntensity = 1;
                        }
                    }
                    if (pointIntensity > 0) //if the point light intensity is more than 0
                    {
                        pointIntensity -= Time.deltaTime/2; //decrease the point light intensity until it is invisible
                    if (pointIntensity < 0)
                        {
                            pointIntensity = 0;
                        }
                    }
                    else
                    {
                        transitioning = !transitioning; //the weather pattern is now read to transition back to rain whenever
                    }
                    if (alpha > 0) //if there is any tint at all
                    {
                        alpha -= Time.deltaTime/2; // decrease the tint until it disappears
                        if(alpha < 0)
                        {
                            alpha = 0;
                        }
                    }
                    //update the tint in the shaders of the background props
                    BG[0].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                    BG[1].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                    BG[2].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                    BG[3].GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                    sky.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", alpha);
                    //set the light sources' intensity to the ones updated above
                    globalLight.intensity = globalIntensity;
                    pointLight.intensity = pointIntensity;
            }

            if (lastsTime >= lasts) //when the weather pattern elapsed time surpassed how long it should last, revert everything back to clear conditions
            {
                if (currentWeather == (int)weather.RAIN) //in case of rain
                {
                    
                    rain.Stop(); //stop the rain particle system
                }
                if(currentWeather == (int)weather.UPDRAFT) //in the case the wind comes from the bottom
                {
                    windDown.SetActive(false); //make the wind sprite object disappear
                    mrPeliPost.propelVelocity = Globals.propulsion; //revert the flying amount back to normal
                }
                if (currentWeather == (int)weather.DOWNWIND) //in the case the wind comes from the top
                {
                    windUp.SetActive(false); //make the wind sprite object disappear
                    mrPeliPost.propelVelocity = Globals.propulsion; //revert the flying amount back to normal
                }
                if (currentWeather == (int)weather.TAILWIND) //in the case the wind comes from the left
                {
                    windLeft.SetActive(false); //make the wind sprite object disappear
                    float prevVelocity = Globals.scrollSpeed; //save the previous velocity to subtract from flying objects speed
                    Globals.scrollSpeed = -5.0f; //reset the scrollspeed back to normal
                    foreach (Rigidbody2D instance in Spawner.instances) //for all the objects that has been spawned in the game
                    {
                        if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing")) // for buildings and post offices
                        {
                            instance.velocity = new Vector2(Globals.scrollSpeed, 0); // reset the scrol speed back to normal
                        }
                        else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing")) //for flying objects
                        {
                            instance.velocity = new Vector2(instance.velocity.x - prevVelocity + Globals.scrollSpeed, 0); //revert the speed back to orginal speeds
                            Spawner.initialVelocity[instance] = instance.velocity; // save the initial velocity of the object in case of resuming
                        }
                    }
                    foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes) //for all envelopes spawned
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0); //revert the scroll speed back to normal
                    }
                    foreach (Rigidbody2D instance in Deliver.packages) //for all packages spawned
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0); //revert the scroll speed back to normal
                    }
                    //the background props is also readjusted back to normal
                    BG[0].GetComponent<Parallax>().parallax = Globals.frontParallax;
                    BG[1].GetComponent<Parallax>().parallax = Globals.backParallax;
                    BG[2].GetComponent<Parallax>().parallax = Globals.frontParallax;
                    BG[3].GetComponent<Parallax>().parallax = Globals.backParallax;
                }
                //same as above
                if (currentWeather == (int)weather.SOUTHWIND)
                {
                    windRight.SetActive(false);
                    float prevVelocity = Globals.scrollSpeed;
                    Globals.scrollSpeed = -5.0f;
                    foreach (Rigidbody2D instance in Spawner.instances)
                    {
                        if (instance.CompareTag("Obstacle") || instance.CompareTag("Refreshing"))
                        {
                            instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                        }
                        else if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing"))
                        {
                            instance.velocity = new Vector2(instance.velocity.x - prevVelocity + Globals.scrollSpeed, 0);
                            Spawner.initialVelocity[instance] = instance.velocity;
                        }
                    }
                    foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes)
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                    }
                    foreach (Rigidbody2D instance in Deliver.packages)
                    {
                        instance.velocity = new Vector2(Globals.scrollSpeed, 0);
                    }
                    BG[0].GetComponent<Parallax>().parallax = Globals.frontParallax;
                    BG[1].GetComponent<Parallax>().parallax = Globals.backParallax ;
                    BG[2].GetComponent<Parallax>().parallax = Globals.frontParallax;
                    BG[3].GetComponent<Parallax>().parallax = Globals.backParallax;
                }
                currentWeather = (int)weather.CLEAR; //reset the weather back to clear
                SoundPlayer.StopPlayingWeather(); //stop playing the weather sound effect
            }

            if (currentWeather != (int)weather.CLEAR) //if the weather isn't clear then increase the time elapsed during the weather effect
            {
                lastsTime += Time.deltaTime;
            }

            time += Time.deltaTime; //increase the time step in which determines when the weather pattern has a chance to occur again
        }
    }
}
