using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;

public class ReplayTween : MonoBehaviour //script that houses the function to reset the game loop and show the replay menu UI
{
    public Transform board; //gets reference to the board that holds the replay emnu UI after losing
    public Weather weather; //gets reference to the weather script (that contains weather information)
    public CanvasGroup background; //gets reference to the translucent black background when pausing the game
    public Movement MrPeliPostMovement; //gets a reference to the character movement script (to reset to initial values)
    public Spawner spawn; //gets a reference to the spawner (to clear all instances and reset the game)
    public ParticleSystem rain; // gets a reference to the rain particle system (to stop it when restarting the game)
    public GameObject sky; //gets a reference to the sky background (to manipulate shader values so that it reverts back to normal since it changes when raining)
    public Light2D globalLight; //gets a reference to the global lighting (when it rains the screen goes dark, so this is to revert it back to normal)
    public Light2D pointLight; //gets a reference to the point lighting originating from the character (when it rains, the only light source is the character, so it is reverted back to normal in a restart)
    public Parallax[] BG; //gets a reference to the background props to reset them back to the initial position
    public TextMeshProUGUI delivered; //gets a reference to the text HUD so that it can be reset (delivered envelopes)
    public TextMeshProUGUI numEnvelope; //gets a reference to the text HUD so that it can be reset (envelopes gathered)
    public TextMeshProUGUI weightAmt; //gets a reference to the text HUD so that it can be reset (character weight)
    public Countdown countdown; //gets a refgerence to the countdown script so that a countdown can be done
    public GameObject fadeToTitle; // gets a reference to the fade out object in case the player wants to go back to the title screen

    private void OnEnable()
    {
        background.alpha = 0; //initially, the translucent black screen is transaprent
        background.LeanAlpha(1, 1.0f); //slowly tween the alpah of the translucent screen to its full opacity (1 * 0.5 is still 0.5)

       
        board.localPosition = new Vector2(0, -Screen.height); //the billboard for the menu UI starts off camera
        board.LeanMoveLocalY(0, 1.0f).setEaseInOutBounce(); //the billboard tweens to the center of the screen with a bounce effect
    }

    public void replay() //game replay button is pressed
    {
        MrPeliPostMovement.MrPeliPost.gravityScale = 0; //reset the character gravity (upon starting the game, everything freezes so the character is not affected by gravity yet)
        MrPeliPostMovement.MrPeliPost.velocity = Vector2.zero; //reset the character velocity (upon starting the game, everything freezes so the character does not move yet)
        MrPeliPostMovement.MrPeliPost.gameObject.GetComponent<BoxCollider2D>().enabled = true; //reset the character collision back on (when the player loses the game, the box collider is turned off)
        MrPeliPostMovement.lockMovement = true; //lock the character control (upon starting the game, everything freezes so the character does not move yet)
        MrPeliPostMovement.weight = 1f; //reset the weight of the character from the previous values of the previous play session
        MrPeliPostMovement.MrPeliPost.rotation = 0f; //if the character was rotated in anyway, reset the rotation (the character may be rotated upon losing by helicopter collision)
        MrPeliPostMovement.MrPeliPost.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; //refreezes the rotation of the character (the character is allowed to rotate if the player loses by helicopter collision
        MrPeliPostMovement.confused = false; //reset the condition of the character
        MrPeliPostMovement.conTimer = 0f; // reset the timer of how long the character remains confused (only used when and if the character is confused to begin with)
        MrPeliPostMovement.MrPeliPost.gameObject.transform.position = new Vector3(-7.35f, 0.46f, 0); //reset the character's default position
        MrPeliPostMovement.MrPeliPost.GetComponent<Animator>().Play("Peli_Idle"); //reset the character's default animation
        Movement.savedVelocity = Vector2.zero; //the character's saved velocity for resuming the game is cleared and reset
        Events.numberDelivered = 0; //reset the number of delivered envelopes to 0
        Events.numberOfEnv = 0; // reset the number of gathered envelopes to 0
        Events.lost = false; // reset the flag that denotes that the player has lost the game
        Spawner.gameStarting = true; // the game is starting so the game does not go into the end sequence while the game is still inactive
        Spawner.ended = false; // reset the flag that denotes that the game has ended already
        Spawner.interval = 10f; //the time interval between object and obstacle spawning is reset
        spawn.totalTime = 0f; //reset the total elapsed time of the game session (affects time interval between object and obstacle spawning
        spawn.time = 0f; //reset the time step of the game (if time >= interval and obstacle spawns)
        //resets the background props to their original location
        BG[0].gameObject.transform.position = new Vector3(0.452f, 0.421f, 0);
        BG[1].gameObject.transform.position = new Vector3(0.46f, 0.39f, 0);
        BG[2].gameObject.transform.position = new Vector3(23.83f, 0.44f, 0);
        BG[3].gameObject.transform.position = new Vector3(23.89f, 0.44f, 0);
        //clear out all the objects from the previous game session of their respective lists and destroy all objects
        foreach(Rigidbody2D instance in Deliver.packages)
        {
            Destroy(instance.gameObject);
        }
        Deliver.packages.Clear();
        foreach(Rigidbody2D instance in Spawner.instances)
        {
            Destroy(instance.gameObject);
        }
        Spawner.instances.Clear();
        Spawner.initialVelocity.Clear();
        foreach (Rigidbody2D instance in EnvelopeSpawner.envelopes)
        {
            Destroy(instance.gameObject);
        }
        EnvelopeSpawner.envelopes.Clear();
        foreach (Rigidbody2D instance in ChildSpawner.children)
        {
            Destroy(instance.gameObject);
        }
        ChildSpawner.children.Clear();
        foreach (Rigidbody2D instance in ChildAI.rocks)
        {
            Destroy(instance.gameObject);
        }
        ChildAI.rocks.Clear();
        Weather.currentWeather = (int) Weather.weather.CLEAR; //resets the weather so that it is clear again.
        weather.transitioning = false; //if the weather was transitioning from clear to rain or rain to clear when the game is replayed, then it is no longer transitioning
        weather.globalIntensity = 1; //resets the global light intensity so light is normal again.
        weather.alpha = 0; //this alpha value will tint the sprites a darker blue the higher the value is. So this resets it so that sprites are no longer tinted (only occurs during rain)
        weather.pointIntensity = 0; //resets the point light intensity to 0, so that it is invisible
        weather.time = 0; //resets the time step for determining when to cause a weather effect (if time >= interval then weather effect happens)
        weather.lastsTime = 0; //resets the time step for determining how much longer the weather effect will last (if lastTime >= lasts then weather stops)
        rain.Stop(); //if it was raining when game is replayed then stop the particle effect.
        //wind sprites from all cardinal directions becomes invisible
        weather.windDown.SetActive(false); 
        weather.windUp.SetActive(false);
        weather.windLeft.SetActive(false);
        weather.windRight.SetActive(false);
        MrPeliPostMovement.propelVelocity = Globals.propulsion; //the wind weather effect can effect how high the character flies in a tap. This resets it to the initial value.
        Globals.scrollSpeed = -5.0f; //the wind weather effect can effect how fast the screen scrolls. This resets it to the initial value.
        //resets the blue tint on the background props so that it is no longer tinted blue (_Alpha determines how tinted it is the higher it is. This sets the value to 0)
        BG[0].gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0);
        BG[1].gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0);
        BG[2].gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0);
        BG[3].gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0);
        sky.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0);
        globalLight.intensity = 1; //resets the global light intensity so light is normal again. This directly affects the light source.
        pointLight.intensity = 0; //resets the point light intensity to 0, so that it is invisible. This directly affects the light source.
        //resets the HUD text so that it reflects reset values
        numEnvelope.text = "x " + Events.numberOfEnv;
        weightAmt.text = "x " + MrPeliPostMovement.weight.ToString("F1");
        delivered.text = "x " + Events.numberDelivered;
        //a flag is set so that a countdown occurs
        countdown.gameObject.SetActive(true);
        countdown.isStarting = true;
        SoundPlayer.StopPlayingWeather(); //if a weather sound effect was playing, then stop it.
        SoundPlayer.PlayMusic(); //replay the music when game is replayed (it is turned off when the game is lost)
        this.gameObject.SetActive(false); //make the replay menu disappear
    }

    public void exit() //if the player presses the exit button, fade the screen to black and return to the title screen.
    {
        fadeToTitle.SetActive(true);
    }
}
