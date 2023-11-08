using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour //script solely for the purpose of an animation event
{
   void disappear() //sets the object to invisible again after an animation has been played
    {
        this.gameObject.SetActive(false);
    }
}
