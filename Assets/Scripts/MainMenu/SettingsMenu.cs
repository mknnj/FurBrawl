using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle myToggle;
    
    public void OnToggle_FullScreen()
    {
        Screen.SetResolution(1024 ,500,false);
        Screen.fullScreen = myToggle.isOn;
    }
}
