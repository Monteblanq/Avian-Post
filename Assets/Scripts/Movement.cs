using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float propelVelocity = Globals.propulsion; //initialise the flying up speed
    public bool lockMovement = true; // determines whether to lock the player from controlling the character
    public static bool gameActive = false; //the game is not active by default
    public Blink blinkingEffect; //reference to the blinking script
    public Coroutine blinkRoutine; //stores a coroutine reference to be referred to later
    public Rigidbody2D MrPeliPost; //reference to the rigidbody of the character
    public static Vector2 savedVelocity = Vector2.zero; //variable to save the velocity of the character in case the game is paused
    public float weight = 1.0f; // weight is defaulted to 1 (weight determines how effective flying is)
    public bool confused = false; // determines if the character is confused (inverts controls)
    public float confusedTimer = 5f; //the time threshold for the character to stop being confused
    public bool blinkBool = false; //flag to determine if the character is currently blinking
    public float conTimer = 0f; //the time step to determine how long the character remains confused (if conTimer >= confusedTimer then the character stops being confused)
    public GameObject stars; //reference to the stars that spin above the character's head when confused
    public Animator animator; // reference to the character's animator

    // Start is called before the first frame update
    void Start()
    {
        MrPeliPost.gravityScale = 0.0f; //initially when the game starts the character is frozen in place
        MrPeliPost.velocity = Vector2.zero; // the character doesn't move
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive == true) //when the game is active
        {
            if (conTimer > 2f && blinkBool == false && lockMovement == false) //when the confusion has 3 seconds left, then cause a blinking effect
            {
                blinkRoutine = StartCoroutine(blinkingEffect.blinking());
                blinkBool = true;
            }
            if (confused) //if the character is confused
            {
                if (lockMovement == false) //..and the movement isn't locked
                {
                    MrPeliPost.gravityScale = -1; //then the gravity works backwards (character is flapping wings frantically)
                }
                if (Input.touchCount > 0) //if the player taps the screen
                {
                    for (int i = 0; i < Input.touchCount; i++) //going through every tap
                    {
                        if (Input.touches[i].phase == TouchPhase.Began && lockMovement == false) //upon touch and if movement isn't locked
                        {
                            MrPeliPost.velocity = Vector2.down * propelVelocity / weight; //the character flies down, bogged down by their weight, reducing propulsion by a certain factor
                        }
                    }
                }
                if (conTimer >= confusedTimer) //when the confusion time step reached its threshold
                {
                    conTimer = 0; //reset the confusion time step
                    confused = !confused; //the character is no longer confused
                    blinkBool = false; // the character is no longer blinking
                    if (lockMovement == false) //if the player can control the character
                    {
                        MrPeliPost.velocity = Vector2.zero; //character velocity resets back to 0
                    }
                    if (blinkRoutine != null) //stop the blinking
                    {
                        StopCoroutine(blinkRoutine);
                    }
                    Color tmp = this.gameObject.GetComponent<SpriteRenderer>().color; //get the colour information of the character (mainly its alpha)
                    tmp.a = 1f; //set the opacity to completely opaque
                    Color tmp2 = stars.GetComponent<SpriteRenderer>().color; //get the colour information of the spinning stars (mainly its alpha)
                    tmp2.a = 1f; //set the opacity to completely opaque
                    stars.GetComponent<SpriteRenderer>().color = tmp2; //set the spinning stars colour to the one previously defined (mainly change the alpha)
                    stars.SetActive(false); //make the stars disappear
                    this.gameObject.GetComponent<SpriteRenderer>().color = tmp; //set the character colour to the one previously defined (mainly change the alpha)
                    MrPeliPost.GetComponent<SpriteRenderer>().enabled = true; //make sure the character is visible
                    SoundPlayer.StopPlayingConfused(); //stop playing the confused sound effect
                }
                conTimer += 1.0f * Time.deltaTime; // increase the confusion timestep
            }
            else if (Input.touchCount > 0) //if the character is not confused and the player taps
            {
                for (int i = 0; i < Input.touchCount; i++) //going through every tap
                {
                    if (Input.touches[i].phase == TouchPhase.Began && lockMovement == false) //upon touch and if movement isn't locked
                    {
                        MrPeliPost.gravityScale = 1.0f; //character is affected by gravity normally when not confused
                        MrPeliPost.velocity = Vector2.up * propelVelocity / weight; //the character flies up, bogged down by their weight, reducing propulsion by a certain factor

                        SoundPlayer.PlaySound("flap"); // play the flying sound effect
                    }
                }
            }
            else //if the player doesn't tap anything and the character isn't confused
            {
                if (lockMovement == false) //and if the controls aren't locked
                {
                    MrPeliPost.gravityScale = 1.0f;//character is affected by gravity normally
                }
            }
            animator.SetFloat("VelocityY", MrPeliPost.velocity.y); //set the value in the animator to correlate with the y velocity of the character (the animation only plays when the y velocity is negative)
        }

    }

    public void stopCoroutine() //to stop the blinking coroutine in another script (coroutines can only be stopped where it started)
    {
        StopCoroutine(blinkRoutine);
    }
}
