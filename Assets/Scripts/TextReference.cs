using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextReference : MonoBehaviour //solely for the purpose of finding this object in runtime
{
    public static TextMeshProUGUI itself; //the reference for the specific text object so that other objects can refer to it during runtime (particularly prefabs)

    private void Start()
    {
        itself = this.gameObject.GetComponent<TextMeshProUGUI>(); //store the reference to this object
    }
    private void Awake()
    {
        itself = this.gameObject.GetComponent<TextMeshProUGUI>(); //store the reference to this object the moment it is conceived
    }
}
