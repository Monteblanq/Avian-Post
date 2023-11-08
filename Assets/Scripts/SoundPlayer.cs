using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour //A script that plays sound
{
    public static SoundPlayer player; //gets the reference of the sound player
    public static AudioClip down, flap, get, hit, refresh, boom, confuse, drop, land, wind, rain, okay, bgMusic, bgMusicTitle; //audio clips of different effects and music
    static AudioSource effectSource; //source to play sound effects
    static AudioSource confusedEffectSource; //source to play a sound effect when player is confused
    static AudioSource weatherSource; //source to play weather sound effects
    static AudioSource musicSource; //source to play music
    //There must be 4 audio sources so that they play sound concurrently.


    private void Awake() //gets the reference of the object (Sound Player) upon activation
    {
        player = this;
        
    }
    void Start()
    {
        //add AudioSources to this object
        effectSource = gameObject.AddComponent<AudioSource>();
        confusedEffectSource = gameObject.AddComponent<AudioSource>();
        weatherSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        //Load the AudioClips into memory
        down = (AudioClip)Resources.Load("Down");
        flap = (AudioClip)Resources.Load("Flap");
        get = (AudioClip)Resources.Load("Get");
        hit = (AudioClip)Resources.Load("Hit");
        refresh = (AudioClip)Resources.Load("Refresh");
        boom = (AudioClip)Resources.Load("Boom");
        confuse = (AudioClip)Resources.Load("Confuse2");
        drop = (AudioClip)Resources.Load("Drop");
        land = (AudioClip)Resources.Load("Land");
        wind = (AudioClip)Resources.Load("Wind");
        rain = (AudioClip)Resources.Load("Rain2");
        okay = (AudioClip)Resources.Load("Okay");
        bgMusic = (AudioClip)Resources.Load("BGMusic");
        bgMusicTitle = (AudioClip)Resources.Load("BGMusicTitle");

    }

    // Update is called once per frame

    public static void PlayMusic() //plays background music of the main game
    {
        musicSource.loop = true; //loops the song
        musicSource.volume = 0.5f; //sets the volume to half
        musicSource.clip = bgMusic; //sets the song to be played
        musicSource.Play(); //plays the song
    }

    public static void StopMusic() //stops the background music of the game
    {
        musicSource.loop = false; //makes the song stop looping
        musicSource.Stop(); //stops the song
    }

    public static bool MusicIsPlaying() //checks if the song is still playing
    {
        return musicSource.isPlaying; //returns the state of the player
    }

    public static void PlayTitleMusic() //plays the background music of the title 
    {
        musicSource.loop = true; //loops the song
        musicSource.volume = 0.5f; //sets the volume to half
        musicSource.clip = bgMusicTitle; //sets the song to be played
        musicSource.Play(); //plays the song
    }

    public static void StopTitleMusic()
    {
        musicSource.loop = false; //makes the song stop looping
        player.StartCoroutine(AudioFade.StartFade(musicSource, 0.5f, 0f, true)); //starts a simultaneous function to run to fade the music out

    }

    public static void PlaySound(string clip) //plays a sound effect
                                              //clip: name of sound to be played
    {
        switch (clip) //sets the audio source to the appropriate sound effect and play it once
        {
            case "down":
                effectSource.PlayOneShot(down);
                break;
            case "flap":
                effectSource.PlayOneShot(flap);
                break;
            case "get":
                effectSource.PlayOneShot(get);
                break;
            case "hit":
                effectSource.PlayOneShot(hit);
                break;
            case "refresh":
                effectSource.PlayOneShot(refresh);
                break;
            case "boom":
                effectSource.PlayOneShot(boom);
                break;
            case "confuse":
                effectSource.PlayOneShot(confuse);
                break;
            case "drop":
                effectSource.PlayOneShot(drop);
                break;
            case "land":
                effectSource.PlayOneShot(land);
                break;
            case "okay":
                effectSource.PlayOneShot(okay);
                break;
        }
    }
    public static void PlayOnRepeatConfused(string clip) //plays a sound effect for the confused status effect on loop
                                                         //clip: name of sound to be played on loop
    {
        switch (clip) //sets the audio source to the clip and plays it on loop
        {
            case "flap":
                confusedEffectSource.loop = true; //loop the sound
                confusedEffectSource.clip = flap; //set the sound
                confusedEffectSource.Play(); //plays the sound on loop
                break;
        }
    }

    public static void StopPlayingConfused() //when the player stops being confused, stop the confused sound effect
    {
        confusedEffectSource.loop = false; //sets the sound to stop looping
        confusedEffectSource.Stop(); //stops the sound
    }

    public static void PlayOnRepeatWeather(string clip) //plays a sound effect for weather
                                                        //clip: name of sound to be played
    {
        switch (clip) //sets the audio source to the clip and plays it on loop
        {
            case "wind":
                weatherSource.loop = true;
                weatherSource.clip = wind;
                weatherSource.Play();
                break;
            case "rain":
                weatherSource.loop = true;
                weatherSource.clip = rain;
                //in the case of rain, fade in the sound effect
                weatherSource.volume = 0; //sound starts mute
                player.StartCoroutine(AudioFade.StartFade(weatherSource, 0.5f, 0.5f, false)); //sound slowly fades in
                weatherSource.Play();
                break;
        }
    }
    public static void StopPlayingWeather() //stops playing the weather sound effect
    {
        weatherSource.loop = false; //set the sound to stop looping
        weatherSource.Stop(); //stops the sound
    }
}
