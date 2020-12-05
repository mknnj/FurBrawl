using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerLifeUI : MonoBehaviour
{
    [SerializeField] protected Text playerName;
    [SerializeField] protected Image playerIcon;
    [SerializeField] protected Transform lifeZone;
    
    /// <summary>
    /// Remove one life icon from UI
    /// </summary>
    public abstract void RemoveLife();
    
    /// <summary>
    /// Set Ui text for player name
    /// </summary>
    /// <param name="name"></param>
    public virtual void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public virtual void SetAvatar(Sprite avatar)
    {
        playerIcon.sprite = avatar;
    }
}
