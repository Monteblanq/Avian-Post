using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tweenAlpha : MonoBehaviour  //a script that causes the instructions of the tutorial to fade in
{
    public Button tutorialNextButton; //gets a reference to the next button of the tutorial
    public Button tutorialCompleteButton; //gets a reference to the complete button of the tutorial
    public Button skipButton; //gets a reference to the skup button of the tutorial

    public void OnEnable() //when this object becomes active
    {
        //starts the object with 0 opacity
        Color tmp = this.gameObject.GetComponent<SpriteRenderer>().color;
        tmp.a = 0f;
        this.gameObject.GetComponent<SpriteRenderer>().color = tmp;
        this.gameObject.LeanAlpha(1, 1f).setOnComplete(onComplete); //tweens the opacity so it is fully opaque and displays the instruction, the runs the onComplete function
        
    }

    void onComplete() //after the object has completely faded in, enable all the buttons
    {
        skipButton.enabled = true;
        tutorialNextButton.enabled = true;
        tutorialCompleteButton.enabled = true;
    }
}
