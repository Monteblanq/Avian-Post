using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class WaitSecondsReplay : MonoBehaviour //a script that houses the coroutine to wait a few seconds before displaying the after game menu to replay or leave the game
{
    public GameObject replayMenuBox; //reference to the post-game menu for replay and exit game
    public Text highScoreText; //reference to the highscore text which will be displayed on the menu
    public IEnumerator replayMenu() //a couroutine that sets the menu to active so it tweens in
    {
        yield return new WaitForSeconds(3.0f); //continue for 3 seconds without doing anything
        if(Globals.highScore < Events.numberDelivered) //if the current highscore is lesser than the number of envelopes delivered in this game session, the update the high score
        {
            Globals.highScore = Events.numberDelivered; //update the high score
            string path = Application.persistentDataPath + "/highScore.txt"; //creates and opens the file to store the highscore
            StreamWriter writer = new StreamWriter(path, false); //creates the file if it doesn't exist and opens it
            writer.WriteLine(Events.numberDelivered); //overwrite the file so it only contains one line (the highscore)
            writer.Dispose(); //close the connection
        }
        highScoreText.text = "High Score: " + Globals.highScore; //set the text to reflect the high score
        replayMenuBox.SetActive(true); //set the menu to active so it tweens in
        yield break; //stops the coroutine

    }
}