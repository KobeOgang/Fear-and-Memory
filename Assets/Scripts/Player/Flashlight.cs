using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;

    public bool on;
    public bool off;


    private void Start()
    {
        off = true;
        flashlight.SetActive(false);
    }

    private void Update()
    {
        if (off && Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(true);
            off = false;
            on = true;
        }
        else if (on && Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(false);
            on = false;
            off = true;
        }
    }
}
