using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour //a script that does the fade in from black at the start of the game and does initial prep
{
    // Start is called before the first frame update
    public CanvasGroup blackBG; //reference of the black screen
    public GameObject tutorial; //reference of the UI group that shows the tutorial

    void Start()
    {
        blackBG.LeanAlpha(0, 1f).setOnComplete(completeFadeIn); //gradually tween the alpha of the black screen to 0 in one second, after which the completeFadeIn function is called after it is done
    }

    void completeFadeIn() //function that runs after the black screen clears up
    {
        tutorial.SetActive(true); //set the UI group for the tutorial to be active, which activates initialisation prep
        this.gameObject.SetActive(false); //the black screen disappears
    }
}
