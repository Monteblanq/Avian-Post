using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public CanvasGroup blackBG; //gets reference to a black screen

    private void OnEnable() //On activation of the object, fade the black screen from transparent to opaque
    {
        blackBG.alpha = 0.0f; //black screen starts transparent
        blackBG.LeanAlpha(1, 1.0f).setOnComplete(completeAlpha); //tween the alpha of the black screeen to fully opaque, then after it finishes, do the completeAlpha function
    }

    void completeAlpha() //function that runs when the alpha finishes tweening
    {
        TitleTween.finishTween = false; //reset the state of the tweening so that players can tap the screen once more after the return to the title screen
        SceneManager.LoadScene("AvianPost", LoadSceneMode.Single); //changes the scene to the game scene
    }
}
