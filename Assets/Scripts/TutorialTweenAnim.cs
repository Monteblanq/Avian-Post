using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTweenAnim : MonoBehaviour //the script resposible for displaying the tutorial UI as well as giving the tutorial itself
{
    public Transform board; //reference to the board that holds the tutorial menu elements
    public CanvasGroup background; //reference to the translucent black background that overlays the screen
    public Button tutorialButton; //reference to the next button
    public Button tutorialButtonComplete; //reference to the complete tutorial button
    public Button skipButton; //reference to the skip tutorial button
    public GameObject[] instructions; //objects of pages of instructions. index 0 has page 1, index 1 has page 2, and so on
    public Countdown countdown; //a reference to the countdown script to initiate a countdown after the tutorial has been given
    private int whichIntruction = 0; //denotes which page the tutorial is on

    private void OnEnable() //when this object is activated
    {
        if (!Globals.tutorialGiven) //checks if the tutorial has already been given, if it has already been given before, it won't show itself again.
        {
            tutorialButton.enabled = false; //the next button is disabled initially until the instruction is fully shown
            tutorialButtonComplete.enabled = false; //the complete button is disabled until the last instruction is fully shown
            skipButton.enabled = false; //the skip button cannot be pressed until it has fully come into the screen.
            skipButton.transform.localPosition = new Vector2(skipButton.transform.localPosition.x, -Screen.height * 2); //set the initial location of the skip button off camera at the bottom

            background.alpha = 0; //black translucent screen is initially transparent
            background.LeanAlpha(1, 1f); //the translucent black screen slowly becomes fully opaque (1 * 0.5 is still 0.5 opacity)

            board.localPosition = new Vector2(0, -Screen.height * 2); //the initial location of the board is off camera at the bottom
            board.LeanMoveLocalY(0, 1f).setEaseInOutBounce().setOnComplete(tweenSkip); //tween in the board and the buttons and instructions with a bounce, then runs the tweenSkip function


            
        }
        else //if tutorial has already been given
        {
            background.alpha = 1; //the black translucent screen immediately starts with full opacity as the countdown starts
            countdown.isStarting = true; //starts the countdown
            if(!SoundPlayer.MusicIsPlaying()) //if the song isn't already play, then play the song
            {
                SoundPlayer.PlayMusic();
            }
        }
    }

    public void next() //the next button that goes to the next instruction
    {
        tutorialButton.enabled = false; //after pressing the next button, the next button will temporarily be disabled until the next instruction completely fades in
        tutorialButtonComplete.enabled = false; //after pressing the next button, the complete tutorial button will temporarily be disabled until the final instruction completely fades in
        instructions[whichIntruction].LeanAlpha(0, 0.5f).setOnComplete(completeTween); //when the next button is pressed, the current instruction will fade out and run the completeTween function



    }

    public void complete() //the complete button that finishes the tutorial
    {
        tutorialButtonComplete.enabled = false; //after pressing the complete button, it cannot be pressed again
        instructions[whichIntruction].LeanAlpha(0, 0.5f).setOnComplete(completeTweenForComplete); //after pressing the complete button, the last instruction will fade out and run the completeTweenForComplete function
        board.LeanMoveLocalY(-Screen.height * 2, 1f).setOnComplete(completeMove); //the board tweens away off screen and then runs the function completeMove

    }

    void completeTween() //runs when the current instruction fades out, which fades in the next instruction
    {
        whichIntruction++; //goes to the next page
        instructions[whichIntruction - 1].SetActive(false); //deactivate the previous page after it fades out
        instructions[whichIntruction].SetActive(true); //activate the new page and it fades in (with its OnEnable function)
        if (whichIntruction == 5) //if this is the last page
        {
            tutorialButton.gameObject.SetActive(false); //the next button disappears...
            tutorialButtonComplete.gameObject.SetActive(true); // ..revealing the complete button underneath
        }
    }

    void completeTweenForComplete() //at the end of the tutorial, after pressing the complete button this runs
    {
        instructions[whichIntruction].SetActive(false); //set the final instruction inactive
    }

    void completeMove() //moves the board away after the tutorial
    {
        skipButton.transform.LeanMoveLocalY(-Screen.height * 2, 0.5f); //tweens the board away
        countdown.isStarting = true; //starts up the countdown
        Globals.tutorialGiven = true; //after this, the tutorial is given and won't be given again until the next reboot
        SoundPlayer.PlayMusic(); //the music starts playing
    }

    void tweenSkip() //the skip button tweens in after the board tweens in
    {
        skipButton.transform.LeanMoveLocalY(-424f, 0.5f).setEaseLinear().setOnComplete(finishTweenSkip); //the skip button tweens in and then plays the function finishTweenSkip
    }

    void finishTweenSkip() //after the skip button tweens in, the first page is shown and fades in.
    {
        instructions[whichIntruction].SetActive(true);
    }

    public void skip() //if the player presses the skip button
    {
        tutorialButton.enabled = false; //the next button can no longer be pressed
        tutorialButtonComplete.enabled = false; //the complete button can no longer be pressed
        skipButton.enabled = false; //the skup button can not be pressed again
        instructions[whichIntruction].LeanAlpha(0, 0.5f).setOnComplete(finishSkip); //the current page fades out and the runs the function finishSkip
    }

    void finishSkip() //runs to move the board away after skipping
    {
        instructions[whichIntruction].SetActive(false); //deactivates the current page
        board.LeanMoveLocalY(-Screen.height * 2, 1f).setOnComplete(completeMove); //tweens the board away and runs completeMove, which starts the countdown and starts the game.
    }
}
