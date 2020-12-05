using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void OnToggle_FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
