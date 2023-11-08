using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour //Sets the character to stay within the screen
{
    Vector2 screenBounds; //the bounds of the screen
    private float objectWidth; //the character's half width
    private float objectHeight; //the character's half height
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //Returns the world bounds of the screen. It will return x value and y value that is half the units of width and height of the screen. (Any x value less than half the screen horizontally is negative and so on, same for the y for verticality)
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x; //gets the half width of the sprite (the character)
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y; //get the half height of the sprite (the character)
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Spawner.gameActive) //As long as the game is still active
        {
            Vector3 position = transform.position; //get the current position of the character
            position.x = Mathf.Clamp(position.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth); //clamp the x value so that it is always within the screen, taking into account the width of the character sprite
            position.y = Mathf.Clamp(position.y, screenBounds.y * -2 + objectHeight, screenBounds.y - objectHeight); //clamp the y value so that it is always within the screen, taking into account the height of the character sprite
            transform.position = position; //sets the position of the character to the clamped value
        }

    }
}
