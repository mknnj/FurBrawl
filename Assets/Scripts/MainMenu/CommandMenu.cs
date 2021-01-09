using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandMenu : MonoBehaviour
{
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button furBallButton;
    [SerializeField] private Button meleeButton;
    
    Event keyEvent;
    TextMeshProUGUI buttonText;
    KeyCode newKey;
    bool waitingForKey;
    
    void Start ()

    {
        waitingForKey = false;
        meleeButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.Melee.ToString();
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.Jump.ToString();
        furBallButton.GetComponentInChildren<TextMeshProUGUI>().text = KeyBindings.FurBall.ToString();
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
