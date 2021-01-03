using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUIRight : PlayerLifeUI
{
    [SerializeField] protected int childToRemove = 0;
    public override void RemoveLife()
    {
        if (childToRemove > 8)
            return;

        lifeZone.GetChild(childToRemove).gameObject.SetActive(false);
        childToRemove++;
    }
    
    public override int getLives()
    {
        return 8 - childToRemove;
    }
}
