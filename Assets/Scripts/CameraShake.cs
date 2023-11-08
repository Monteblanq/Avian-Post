using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraShake : MonoBehaviour //shakes the camera for dramatic effect
{
    
    public float elapsed = 0.0f; //time step of the camera shake
    public float duration = 0.0f; //the threshold of how long the shake should be
    public IEnumerator Shake(float duration, float magnitude) //function to shake the camera
                                                              //duration: how long the shake will last
                                                              //magnitude: how dramatic the shake is
    {
        this.duration = duration; //pass the passed parameter to an instance variable so it can be accessed elsewhere
        Vector3 originalPos = transform.position; //record the orginal position of the camera.

        while(elapsed <= duration) //as long as the time step has not yet reached the duration specified, it will run this block
        {
            float x = Random.Range(-1f, 1f) * magnitude; //randomise the x position, magnified by the magnitude passed in
            float y = Random.Range(-1f, 1f) * magnitude; //randomise the y position, magnified by the magnitude passed in

            transform.position = new Vector3(x, y, originalPos.z); //set the camera position with the randomised x and y position
            elapsed += Time.deltaTime; //increase the time step
            //this will continue each frame to simulate a shaking effect since a random x and y value will be calculated and constantly change the camera position
            yield return null;//go to the next frame
        }

        transform.position = originalPos; //resets the camera the way it was
        StopAllCoroutines(); //since the camera shakes when the game ends, stops every other coroutines as well (mainly the blinking if it is running at the moment this coroutine is executed)
    }

    bool IsEnd()// get the status of the shake
    {
        return (elapsed > duration); //returns whether the shaking has ended so that another part of code can stop this coroutine
    }

}
