using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToTitle : MonoBehaviour //script that handles transitioning back to title screen
{
    public CanvasGroup fade; //the reference for the black screen
    private void OnEnable()
    {
        fade.LeanAlpha(1, 1f).setOnComplete(fadeComplete); //gradually tween the alpha of the black screen to opaque, after which the fadeComplete function runs
    }

    void fadeComplete() //function that runs after the screen goes dark
    {
        //reset all game conditions to default
        Movement.savedVelocity = Vector2.zero; //the character's saved velocity is reset to default (Vector.zero)
        Events.numberDelivered = 0; //empties the number of delivered envelopes
        Events.numberOfEnv = 0; //empties the number of gathered envelopes
        Events.lost = false; //reset the flag that determines if the player has lost
        Spawner.gameStarting = true; //reset the game start flag when the player goes back to the game
        Spawner.ended = false; //reset the flag that determines whether the game has completely ended
        Spawner.interval = 10f; // reset the spawn interval to default
        //destroy all objects and clears all data and lists
        foreach (Rigidbody2D instance in Deliver.packages)
        {
            Destroy(instance.gameObject);
        }
        Deliver.packages.Clear();
        foreach (Rigidbody2D instance in Spawner.instances)
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
        Weather.currentWeather = (int)Weather.weather.CLEAR;//resets the weather
        Globals.scrollSpeed = -5.0f; //resets the scrolling speed (may have been altered due to weather)
        SoundPlayer.StopPlayingWeather(); //stop the weather sound effect
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single); //load the title screen scene
    }
}
