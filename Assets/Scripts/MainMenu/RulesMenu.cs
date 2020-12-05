using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesMenu : MonoBehaviour
{
    private int imageIndex = 0;
    [SerializeField] private Sprite[] rulesList;
    [SerializeField] private Image displayedRule;
    [SerializeField] private ScreenManager sm;

    public void OnEnable()
    {
        displayedRule.sprite = rulesList[imageIndex];
    }

    public void OnClick_Next()
    {
        imageIndex += 1;
        if (imageIndex > rulesList.Length - 1)
            imageIndex = 0;
        displayedRule.sprite = rulesList[imageIndex];
    }
    
    public void OnClick_Prev()
    {
        imageIndex -= 1;
        if (imageIndex <0)
            imageIndex = rulesList.Length - 1;
        displayedRule.sprite = rulesList[imageIndex];
    }

    public void OnClick_Back()
    {
        sm.OpenMainMenu();
    }
}
