using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTween : MonoBehaviour //script that hosts the pause function
{
    public ParticleSystem rain; //get the reference to the particle system for rain (to freeze when pausing)
    public GameObject[] winds; //get the references to all the winds in all four cardinal locations (to freeze when pausing)
    public GameObject pauseMenu; //get the reference for the pause menu UI (to activate and cause the UI to appear on the screen)
    public GameObject stars; // get the reference for the spinning stars sprite (to freeze when pausing)
    public GameObject sweat; // get the reference for the sweat drop sprite (to freeze when pausing)
    public Movement mrPeliPost; // get the reference for the character movement script (to lock movement)
    public Parallax[] BG; // get the references for the background props (to freeze scrolling when pausing)

    public void pause() // pause function
    {
        if (!Events.lost) // if the game isn't over yet, then allow pause
        {
            Movement.savedVelocity = mrPeliPost.MrPeliPost.velocity; //save the velocity before pausing the game
            mrPeliPost.MrPeliPost.gravityScale = 0.0f; // when pausing the character isn't affected by gravity
            mrPeliPost.MrPeliPost.velocity = Vector2.zero; // freeze the character (make it not move)
            Movement.gameActive = false; // the game is no longer active when pausing the game, so lock the movement
            //background scrolling stops
            BG[0].parallax = 0.0f;
            BG[1].parallax = 0.0f;
            BG[2].parallax = 0.0f;
            BG[3].parallax = 0.0f;
            if(Weather.currentWeather == (int)Weather.weather.RAIN) //if the weather is raining, then pause the rain particle effect
            {
                rain.Pause();
            }
            SoundPlayer.StopPlayingConfused(); //if the character was confused, then stop the sound effect.
            Spawner.gameStarting = true; //set a flag so that the game can resume (this flag prevents the ending sequence of the game in the Spawner object)
            Spawner.gameActive = false; // game is no longer active when pausing the game, so spawning stops
            Events.gameActive = false; // game is no longer active when pausing the game, so character movement is locked
            ChildSpawner.gameActive = false; // game is no longer active when pausing the game, so spawning stops
            EnvelopeSpawner.gameActive = false; // game is no longer active when pausing the game, so spawning stops
            mrPeliPost.MrPeliPost.gameObject.GetComponent<Animator>().enabled = false; //whatever animation that the character is playing is paused
            foreach (Rigidbody2D instance in Deliver.packages) //iterate through the list of packages in the game
            {
                instance.gravityScale = 0.0f; //package stops being affect by gravity when game is paused
                Deliver.savedVelocity.Add(instance, instance.velocity); //the velocity is saved so it can be reset when game is resumed
                instance.velocity = Vector2.zero; // the packages stop moving 
                instance.gameObject.GetComponent<Animator>().enabled = false; //whatever animation the package is playing is paused
            }
            foreach (Rigidbody2D instance in Spawner.instances) //iterate through the list of objects in the game
            {
                instance.velocity = Vector3.zero; //all objects top moving
                if (instance.CompareTag("FlyingObstacle") || instance.CompareTag("Confusing")) //if the object is a helicopter or a tornado
                {
                    instance.gameObject.GetComponent<Animator>().enabled = false; //animation is frozen
                }

            }
            foreach(Rigidbody2D instance in ChildAI.rocks) //iterate through the list of rock entities in the game
            {
                instance.gravityScale = 0.0f; //rock is no longer affect by gravity in a pause
                ChildAI.rockSavedVelocity.Add(instance, instance.velocity); // each rock's velocity before pausing is saved so that it can be reset when resuming
                instance.velocity = Vector2.zero; // the rocks no longer moves
            }
            //freeze the animations of the star, sweat and the wind sprites
            stars.GetComponent<Animator>().enabled = false;
            sweat.GetComponent<Animator>().enabled = false;
            winds[0].GetComponent<Animator>().enabled = false;
            winds[1].GetComponent<Animator>().enabled = false;
            winds[2].GetComponent<Animator>().enabled = false;
            winds[3].GetComponent<Animator>().enabled = false;

            pauseMenu.SetActive(true); //the pause menu UI tweens down after being set active
        }
    }
}
