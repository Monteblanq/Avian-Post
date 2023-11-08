using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioFade //A manual fading script for audio
{
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, bool turnOff) //audioSource: audio to fade,
                                                                                                                   //duration: duration of fade,
                                                                                                                   //targetVolume: end volume after fade,
                                                                                                                   //turnOff: true: turns off after target volume is reached
    {
        float currentTime = 0; // timestep of fade
        float start = audioSource.volume; //get current audio volume to fade from

        while (currentTime < duration) //keeps running if duration hasn't ended
        {
            currentTime += Time.deltaTime; //increase timestep
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration); //gradual adjustment of the volume based on timestep
            yield return null; //skip to next frame
        }
        if(turnOff) //check for flag to turn of sound after target volume is reached
        {
            audioSource.Stop(); //stops the sound
        }
        yield break; //stop the coroutine
    }
}
