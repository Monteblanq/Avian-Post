using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals //class to store global information
{
    public static int highScore = 0; //player high score
    public static float propulsion = 5.0f; //flying velocity
    public static float scrollSpeed = -5.0f; //scroll speed centralised
    public static float frontParallax = 3.0f; // scrolling speed for most front background prop
    public static float backParallax = 1.0f; //scrolling speed for most back background prep
    public static bool tutorialGiven = false; //determines whether to play the tutorial again
}
