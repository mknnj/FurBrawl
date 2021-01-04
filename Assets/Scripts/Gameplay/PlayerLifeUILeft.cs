using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUILeft : PlayerLifeUI
{
    [SerializeField] protected int childToRemove = 8;
    public override void RemoveLife()
    {
        if (childToRemove < 0)
            return;
        
        lifeZone.GetChild(childToRemove).gameObject.SetActive(false);
        childToRemove--;
    }

    public override int getLives()
    {
        return childToRemove;
    }
}
