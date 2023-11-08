using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleTween : MonoBehaviour
{
    public TextMeshProUGUI tapAnywhere; //gets reference to the text prompt that tells players to tap anywhere
    public GameObject title; //gets reference to the title text sprite
    public static bool finishTween = false; //state of the tweening
    private void Start() //execute when script just became active
    {
        title.transform.localPosition = new Vector3(0, 5.29f, 1); //set default position of the title text sprite 
        this.gameObject.transform.localPosition = new Vector3(0, 5.29f, 1); // set the default position of the title billboard sprite
        this.gameObject.LeanMoveLocalY(-0.53f, 1f).setEaseLinear().setOnComplete(complete); //tween the billboard sprite down into the screen and run the complete function after the tweening is done

    }

    public void complete() //function runs upon completion of the tweening
    {
        title.LeanMoveLocalY(-0.49f, 1f).setEaseLinear().setOnComplete(totalComplete); //tweens the title text prompt and run the totalComplete function when the tweening is done.
    }


    public void totalComplete()
    {
        tapAnywhere.enabled = true; //set the text prompt telling the player to tap anywhere to active so that the player can see the prompt
        finishTween = true; //set the state of the tweening as finished
    }
}
