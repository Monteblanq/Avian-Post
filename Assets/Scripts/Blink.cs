using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour //Creates a blinking effect when confusion is about to end
{
    public SpriteRenderer MrPeliPost; //gets the reference to the sprite renderer of the character controlled by the player
    public SpriteRenderer Stars; //gets the reference to the sprite renderer of the stars that spin above the character's head
    public IEnumerator blinking()
    {
        Color tmp = MrPeliPost.GetComponent<SpriteRenderer>().color; //a temporary variable to store the colour information of the character (mainly for the alpha)
        Color tmp2 = Stars.GetComponent<SpriteRenderer>().color; //a temporary variable to store the colour information of the spinning stars (mainly for the alpha)
        while (true) //run until the couroutine is stopped
        {
            
            tmp.a = 0f; //set the alpha of the character colour info to 0 (transparent)
            tmp2.a = 0f; // set the alpha of the spinning stars colour info to 0 (transparent)
            MrPeliPost.GetComponent<SpriteRenderer>().color = tmp; //set the colour of the character to the colour that is set earlier (alpha is changed, mainly)
            Stars.GetComponent<SpriteRenderer>().color = tmp2; //set the colour of the spinning stars to the colour that is set earlier (alpha is changed, mainly)

            yield return new WaitForSeconds(0.1f); //go to the next frame and wait for 0.1 seconds before continuing the coroutine

            tmp.a = 1f; //set the alpha of the character colour info to 1 (opaque)
            tmp2.a = 1f; // set the alpha of the spinning stars colour info to 1 (opaque)
            MrPeliPost.GetComponent<SpriteRenderer>().color = tmp; //set the colour of the character to the colour that is set earlier (alpha is changed, mainly)
            Stars.GetComponent<SpriteRenderer>().color = tmp2; //set the colour of the spinning stars to the colour that is set earlier (alpha is changed, mainly)

            yield return new WaitForSeconds(0.1f); //go to the next frame and wait for 0.1 seconds before continuing the coroutine
        }
    }
}
