using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuTween : MonoBehaviour //script that handles the tweening of the pause menu and its related functions
{
    public Transform board; // the reference of the transform (determines location) of the board UI
    public CanvasGroup background; // the reference for the translucent black screen
    public Countdown countdown; // the reference for the countdown script (to countdown and resume the game)
    public GameObject fadeToTitle; //the reference for the fade out black screen when fading out to the title screen
    private void OnEnable()//when the object becomes active
    {
        background.alpha = 0; //the translucent screen is originally transparent
        background.LeanAlpha(1, 1.0f); // gradually turns the transparent screen to translucent


        board.localPosition = new Vector2(0, -Screen.height*2); //set the default location for the board (off camera)
        board.LeanMoveLocalY(0, 1.0f).setEaseInOutBounce(); //tween the board to the center of the screen
    }

    public void resume() //function for the resume button
    {
        board.LeanMoveLocalY(-Screen.height*2, 0.5f).setEaseLinear().setOnComplete(resumeTweenComplete); //tween away the board and then runs the function resumeTweenComplete
    }

    void resumeTweenComplete() 
    {
        countdown.gameObject.SetActive(true); //set the countdown to start
        this.gameObject.SetActive(false); //disables the pause menu UI
        countdown.isStarting = true; //starts the countdown
    }

    public void exit() //function for the exit button
    {
        fadeToTitle.SetActive(true); //turn on the fade to black when going back to the title screen which causes a chain of functions to execute
    }
}
