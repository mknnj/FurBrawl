using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    
    public void OnToggle_FullScreen()
    {
        Screen.SetResolution(1024 ,500,false);
        Screen.fullScreen = fullScreenToggle.isOn;
    }

    public void OnChange_ResolutionDropdown()
    {
        Debug.Log("Chosen: "+resolutionDropdown.options[resolutionDropdown.value].text);
        switch (resolutionDropdown.options[resolutionDropdown.value].text)
        {
            case "1024x500":
                Screen.SetResolution(1024 ,500,fullScreenToggle.isOn);
                break;
            case "1920x1080":
                Screen.SetResolution(1920 ,1080,fullScreenToggle.isOn);
                break;
            case "1280x720":
                Screen.SetResolution(1280 ,720,fullScreenToggle.isOn);
                break;
            case "1280x1024":
                Screen.SetResolution(1280 ,1024,fullScreenToggle.isOn);
                break;
            case "1920×1200":
                Screen.SetResolution(1920 ,1200,fullScreenToggle.isOn);
                break;
            case "1600×1200":
                Screen.SetResolution(1600 ,1200,fullScreenToggle.isOn);
                break;
        }
    }
}
