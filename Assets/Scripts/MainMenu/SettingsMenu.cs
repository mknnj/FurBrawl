using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button furBallButton;
    [SerializeField] private Button meleeButton;
    
    Event keyEvent;
    TextMeshProUGUI buttonText;
    KeyCode newKey;
    bool waitingForKey;
    
    public void OnToggle_FullScreen()
    {
        Screen.SetResolution(1024 ,500,false);
        Screen.fullScreen = fullScreenToggle.isOn;
    }
    
    void Start ()

    {
        waitingForKey = false;
        meleeButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.Melee.ToString();
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.Jump.ToString();
        furBallButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.FurBall.ToString();
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

    void OnGUI()

    {
        keyEvent = Event.current;
        if(keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode; 
            waitingForKey = false;
        }
    }

    
    public void StartAssignment(string keyName)
    {
        if(!waitingForKey)
            StartCoroutine(AssignKey(keyName));
    }

    public void SendText(TextMeshProUGUI text)
    {
        buttonText = text;
    }
    

    IEnumerator WaitForKey()
    {
        while(!keyEvent.isKey)
            yield return null;
    }

    

    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;
        yield return WaitForKey();
        switch(keyName)
        {
            case "furball":
            //set new keycode and update button
            KeyBindings.FurBall = newKey;
            buttonText.text = KeyBindings.FurBall.ToString();
            break;
        case "melee":
            KeyBindings.Melee = newKey; 
            buttonText.text = KeyBindings.Melee.ToString(); 
            break;
        case "jump":
            KeyBindings.Jump = newKey;
            buttonText.text = KeyBindings.Jump.ToString();
            break;
        }
        yield return null;
    }
}
