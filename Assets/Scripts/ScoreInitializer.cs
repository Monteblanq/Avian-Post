using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreInitializer : MonoBehaviour //initialises the highscore record by reading from a text file
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/highScore.txt"; //the URL for the highscore file
        if (File.Exists(path)) //if the file is found
        {
            StreamReader reader = new StreamReader(path); //prepare to read the file
            Globals.highScore = int.Parse(reader.ReadLine()); //read the first line of the text file (there is only one line) and convert it to an integer, and initialise the recorded high score from previous game sessions
            reader.Dispose(); //close the connection to the file
        }
        else
        {
            Globals.highScore = 0; //if file is not found then there is no highscore, so set it to 0.
        }
    }
}
