using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Events : MonoBehaviour //handles all the events that occur (mainly collision events) as well as storing some critical information
{
    public Movement peliMovement; //get a reference to the character movement script
    public EnvelopeSpawner eSpawn; //get a reference to the envelope spawner
    public ChildSpawner cSpawn; //get a reference to the child entity spawner
    public Spawner spawn; // get a reference to the object spawner
    public Parallax[] BG; // get a reference to the background props
    public static bool gameActive = false; // determines if the game is still active
    public static bool lost = false; // determines if the player has just lost
    public TextMeshProUGUI numEnvelope; //text HUD for number of envelopes gathered
    public TextMeshProUGUI weightAmt; // text HUD for weight of the character
    public static int numberOfEnv = 0; // the number of envelopes gathered
    public static int numberDelivered = 0; //the number of envelopes delivered
    public GameObject stars; //object to display spinning stars on the character when he is confused
    public GameObject sweat; // object to display a sweat drop feedback when getting hit by a rock

    public CameraShake shake; // reference to a camera object (specifically the script that makes it shake)
    public WaitSecondsReplay waitSecondsReplay; //reference to a script that waits for a few second after the player loses to display the restart or exit menu
 

    private void Update()
    {

        if (gameActive) //as long as the game is still active, the character's movement isn't locked
        {
            peliMovement.lockMovement = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //events for triggers (overlappable objects)
    {
        if(collision.CompareTag("Collectable")) //if the character touches an envelope
        {
            peliMovement.weight += 0.1f; //add weight
            numberOfEnv++; // increase the number of envelopes gathered
            EnvelopeSpawner.envelopes.Remove(collision.attachedRigidbody); //remove the envelope from the list of envelopes spawned
            Destroy(collision.gameObject); //destroy the envelope object (despawn)
            // update the HUD text
            numEnvelope.text = "x " + numberOfEnv;
            weightAmt.text = "x " + peliMovement.weight.ToString("F1");
            SoundPlayer.PlaySound("get"); //play the sound effect for getting a collectable
        }
        if(collision.CompareTag("Confusing")) //if the character touches a tornado
        {
            peliMovement.confused = true; //the character becomes confused
            if(peliMovement.blinkRoutine != null) //if the character was blinking (about to be lucid from confusion)
            {
                peliMovement.stopCoroutine(); //stop the blinking coroutine from the object itself (coroutines can only be stopped where it is started)
            }
            collision.gameObject.GetComponent<Animator>().Play("Tornado_Disperse"); //play the animation for the tornado dispersing
            Color tmp2 = stars.GetComponent<SpriteRenderer>().color; //get the colour info for the spinning stars sprite (mainly the alpha is needed)
            tmp2.a = 1f; //set the value of the alpha to completely opaque
            stars.GetComponent<SpriteRenderer>().color = tmp2; // set the colour of the stars with the defined colour prior (mainly the alpha)
            stars.gameObject.SetActive(true); //set it so the stars are visible
            //do the same for the character itself
            Color tmp = peliMovement.MrPeliPost.GetComponent<SpriteRenderer>().color;
            tmp.a = 1f;
            peliMovement.MrPeliPost.GetComponent<SpriteRenderer>().color = tmp;
            //the alpha change is needed in case the blinking was in process before the character hits the tornado, so this resets the opacity (which the blink coroutine manipulates)
            peliMovement.blinkBool = false; //the character is no longer blinking, so set the flag to flase
            peliMovement.conTimer = 0; // the confusion timer (how long they have left to be confused) is reset
            SoundPlayer.PlaySound("confuse"); //play the confused sound effect
            SoundPlayer.PlayOnRepeatConfused("flap"); //play the flapping sound effect on loop
        }
        if(collision.CompareTag("Heavying")) //if the character touches a rock
        {
            peliMovement.weight += 0.5f; //add weight significantly
            weightAmt.text = "x " + peliMovement.weight.ToString("F1"); //update the text HUD for the weight
            sweat.SetActive(true); // set it so the sweat drops are visible
            ChildAI.rocks.Remove(collision.gameObject.GetComponent<Rigidbody2D>()); //remove the rock from the list of rocks
            Destroy(collision.gameObject); //destroy the rock entity (despawn)
            SoundPlayer.PlaySound("down"); //play the sound effect for getting hit with a rock
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) //events for collidable objects (objects that can bump into you)
    {

        if (collision.collider.CompareTag("Obstacle")) // if the character bumps into a building
        {
            peliMovement.MrPeliPost.velocity = new Vector2(0, 0); //in case of colliding with something, reset the velocity of the character (it isn't supposed to move) so that the character isn't affected by the collision
            collision.rigidbody.velocity = new Vector2(Globals.scrollSpeed, 0); //in case of colliding, the collided object continues to move forward (it isn't supposed to be affected by the collision)
            Vector3 direction = (transform.position + GetComponent<Collider2D>().bounds.extents) - collision.gameObject.transform.position; //gets the direction of the character relative to the collided object
            if (direction.x < 0) //if the character collides from the left (the character hits the left side of the building and not the top or the right)
            {
                lost = true; //the player has lost the game and a flag is set
                SoundPlayer.StopPlayingConfused(); //if the character was confused, stop the sound
                SoundPlayer.StopMusic(); //stop the music
                SoundPlayer.PlaySound("hit"); //play the sound effect for hitting a building
                peliMovement.MrPeliPost.gameObject.transform.position = new Vector2(collision.collider.transform.position.x - collision.collider.bounds.extents.x - peliMovement.MrPeliPost.GetComponent<BoxCollider2D>().bounds.extents.x + 0.2f, peliMovement.MrPeliPost.gameObject.transform.position.y); //in case the character has hit the building, snap the character to the left side face of the building wall (to make the animation look natural)
                peliMovement.MrPeliPost.GetComponent<Animator>().Play("Peli_Splat"); //play the animation of the character hitting a wall
                Color tmp2 = stars.GetComponent<SpriteRenderer>().color; //get the colour info for the spinning stars sprite (mainly the alpha is needed)
                tmp2.a = 1f; //set the value of the alpha to completely opaque
                stars.GetComponent<SpriteRenderer>().color = tmp2; // set the colour of the stars with the defined colour prior (mainly the alpha)
                stars.SetActive(false); //make the stars disappear
                Color tmp = this.gameObject.GetComponent<SpriteRenderer>().color; //get the colour info for the character sprite (mainly the alpha is needed)
                if (peliMovement.blinkRoutine != null) //if the character was blinking, then make it stop blinking
                {
                    peliMovement.stopCoroutine();
                }
                tmp.a = 1f; //set the value of the alpha to completely opaque
                this.gameObject.GetComponent<SpriteRenderer>().color = tmp; // set the colour of the character with the defined colour prior (mainly the alpha)
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true; //make sure that the character sprite is visible
                peliMovement.lockMovement = true; //player can no longer control the character
                peliMovement.MrPeliPost.gravityScale = 1; //the character will fall (if they were confused this would be the opposite direction, this is to reset that)
                peliMovement.confused = false; //if the character was confused they are no longer confused
                peliMovement.conTimer = 0f; //reset the confusion timer (how long the character remains confused)
                this.gameObject.GetComponent<BoxCollider2D>().enabled = false; //the character no longer collides
                gameActive = false; //the game is no longer active
                Spawner.gameActive = false; //the game is no longer active
                ChildSpawner.gameActive = false; //the game is no longer active
                EnvelopeSpawner.gameActive = false; //the game is no longer active
                foreach (Parallax background in BG) //stop the background props from scrolling
                {
                    background.parallax = 0;
                }
                foreach(Rigidbody2D instance in Deliver.packages) //all packages falling at the time of losing will stop scrolling
                {
                    instance.velocity = new Vector2(0, instance.velocity.y);
                }
                foreach(Rigidbody2D instance in ChildAI.rocks) //all rocks thrown at the time of losing will not take into account scroll speed
                {
                    instance.velocity = new Vector2(instance.velocity.x - Globals.scrollSpeed, instance.velocity.y);
                }
                StartCoroutine(shake.Shake(.15f, .4f)); //shake the camera
                StartCoroutine(waitSecondsReplay.replayMenu()); //wait a few seconds before showing the menu to replay or exit the game


            }
        }
        else if(collision.collider.CompareTag("FlyingObstacle")) //if the character bumps into a helicopter
        {
            peliMovement.MrPeliPost.velocity = new Vector2(0, 0); //in case of colliding with something, reset the velocity of the character (it isn't supposed to move) so that the character isn't affected by the collision
            Vector2 initVelocity; //a variable to store the starting velocity of the helicopter
            Spawner.initialVelocity.TryGetValue(collision.rigidbody, out initVelocity); //by using the instance ID as a key, try to get the starting velocity of the helicopter
            collision.rigidbody.velocity = initVelocity; //in case of colliding, the collided object continues to move forward as it original does (it isn't supposed to be affected by the collision)
            Vector3 direction = (transform.position + GetComponent<Collider2D>().bounds.extents) - collision.gameObject.transform.position; //checks where the direction of the collision is relative to the collided object
            if (direction.x < 0 || direction.y > 0) //if the character collides from the left or above
            {
                lost = true; //the player has then lost the game
                peliMovement.MrPeliPost.GetComponent<Animator>().Play("Peli_Squack"); //set the animation of the character to an animation of hitting a helicopter
                peliMovement.MrPeliPost.constraints = RigidbodyConstraints2D.None; //the character is free to rotate however it pleases as it is flung away from an explosion
                SoundPlayer.StopPlayingConfused(); //if the character was confused, stop the sound
                SoundPlayer.StopMusic(); //stop the music
                SoundPlayer.PlaySound("boom"); //play the sound effect for hitting a helicopter (explosion)
                peliMovement.lockMovement = true; //player can no longer control the character
                peliMovement.confused = false; //if the character was confused they are no longer confused
                peliMovement.conTimer = 0; //reset the confusion timer (how long the character remains confused)
                peliMovement.MrPeliPost.gravityScale = 1; //the character will fall (if they were confused this would be the opposite direction, this is to reset that)
                if(peliMovement.blinkRoutine != null) //if the character was blinking, then make it stop blinking
                {
                    peliMovement.stopCoroutine();
                }
                Color tmp = this.gameObject.GetComponent<SpriteRenderer>().color;  //get the colour info for the character sprite (mainly the alpha is needed)
                tmp.a = 1f; //set the value of the alpha to completely opaque
                this.gameObject.GetComponent<SpriteRenderer>().color = tmp; // set the colour of the character with the defined colour prior(mainly the alpha)
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true; //make sure that the character sprite is visible
                Color tmp2 = stars.GetComponent<SpriteRenderer>().color; //get the colour info for the spinning stars sprite (mainly the alpha is needed)
                tmp2.a = 1f; //set the value of the alpha to completely opaque
                stars.GetComponent<SpriteRenderer>().color = tmp2; // set the colour of the stars with the defined colour prior (mainly the alpha)
                stars.SetActive(false); //make the stars disappear
                peliMovement.MrPeliPost.velocity = new Vector2(-10, 10); //fling the character away to the left
                collision.rigidbody.velocity = new Vector2(0, 0); //the collided object no longer scrolls
                this.gameObject.GetComponent<BoxCollider2D>().enabled = false; //the character no long collides with anything
                gameActive = false; //the game is no longer active
                EnvelopeSpawner.gameActive = false; //the game is no longer active
                ChildSpawner.gameActive = false; //the game is no longer active
                Spawner.gameActive = false; //the game is no longer active
                foreach (Parallax background in BG) //stop the background props from scrolling
                {
                    background.parallax = 0;
                }
                foreach (Rigidbody2D instance in Deliver.packages) //all packages falling at the time of losing will stop scrolling
                {
                    instance.velocity = new Vector2(0, instance.velocity.y);
                }
                foreach (Rigidbody2D instance in ChildAI.rocks) //all rocks thrown at the time of losing will not take into account scroll speed
                {
                    instance.velocity = new Vector2(instance.velocity.x - Globals.scrollSpeed, instance.velocity.y);
                }
                StartCoroutine(shake.Shake(.15f, .4f)); //shake the camera
                collision.gameObject.GetComponent<Animator>().Play("Heli_Explode"); //play the exploding animation for the helicopter
                StartCoroutine(waitSecondsReplay.replayMenu()); //wait a few seconds before showing the menu to replay or exit the game


            }
        }
        else if(collision.collider.CompareTag("Floor")) //if the player touches the floor
        {
            peliMovement.MrPeliPost.GetComponent<Animator>().Play("Peli_Run"); //the character will have a running animation
        }

    }
    private void OnCollisionExit2D(Collision2D collision) //if the character leaves the collision
    {
        if (collision.collider.CompareTag("Obstacle")) //if the collided object is a building
        {
            if (collision.rigidbody.velocity.x > Globals.scrollSpeed) //if the building is somehow slower than how it should be scrolling
            {
                collision.rigidbody.velocity = new Vector2(Globals.scrollSpeed, 0); //the building now scrolls as it should
            }
        }
    }
}
