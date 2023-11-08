using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour //script for background props and determining their respective scroll speeds, creating a parallax effect
{
    public GameObject backGround1Back; // gets the reference for the background prop backup off the camera to the right
    public float parallax = 0f; //initially when the game starts the background props don't move
    private float backStartPos; // stores the position of the background prop off the camera to the right
    private Vector2 screenBoundaries; //stores the screen boundaries
    // Start is called before the first frame update
    void Start()
    {
        backStartPos = backGround1Back.transform.position.x; // gets the position of the background prop that is off camera to the right
        screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); // determines the screen boundaries
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -screenBoundaries.x * 2 - 2) //if this prop goes past a certain threshold on the left off camera
        {
            transform.position = new Vector3(backStartPos, transform.position.y, transform.position.z); //snap it to the right off camera, creating an endless scrolling background
        }
        transform.position = new Vector3(transform.position.x - parallax * Time.deltaTime, transform.position.y, transform.position.z); // move the background according to a parallax effect
    }
}
