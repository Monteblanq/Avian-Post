using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TapAnywhere : MonoBehaviour //script to allow player to tap on the screen to start the game
{
    public GameObject fade; //gets reference to an object that fades the screen.
    public bool started = false; //flag that ensures the player can only tap the screen once
    // Update is called once per frame

    private void Start() //Upon the scripts execution, play the title screen music
    {
        SoundPlayer.PlayTitleMusic();
    }
    void Update()
    {
        if (Input.touchCount > 0 && !started && TitleTween.finishTween) //Only allow the player to tap the screen once the intro tweening sequence is completed
        {
            fade.SetActive(true); //activate the fade object to fade the screen to black
            SoundPlayer.PlaySound("okay"); //plays a confirmation sound
            SoundPlayer.StopTitleMusic(); //stop the title screen music
            started = true; //disallow playe to tap again
        }
    }
}
