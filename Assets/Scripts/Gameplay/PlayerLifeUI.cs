using System.Collections;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerLifeUI : MonoBehaviour
{
    [SerializeField] protected Text playerName;
    [SerializeField] protected Image playerIcon;
    [SerializeField] protected Transform lifeZone;
    [SerializeField] private Material[] catMaterials;
    [SerializeField] private Sprite[] catAvatars;
    //[SerializeField] private Image lastLifeWarnings;
    //[SerializeField] private int time = 5;

    private Player player;
    
    /// <summary>
    /// Remove one life icon from UI
    /// </summary>
    public abstract void RemoveLife();
    
    /// <summary>
    /// Set Ui text for player name
    /// </summary>
    /// <param name="name"></param>
    public virtual void SetPlayerName(Player player, int number) {
        this.player = player;
        playerName.text = "P"+ (number) + " - " + player.NickName;
    }

    public Player getOwner()
    {
        return player;
    }

    public virtual int getLives()
    {
        return 0;
    }
    public virtual void SetAvatar(int avatarID)
    {
        CatSkin targetCatSkin = CatSkins.catSkinsList[avatarID];
        playerIcon.sprite = catAvatars[targetCatSkin.baseSkinID];
        Material mat = new Material(catMaterials[targetCatSkin.baseSkinID]);
        mat.SetColor("_SkinABC", new Color(targetCatSkin.skinColor.r/255,targetCatSkin.skinColor.g/255, targetCatSkin.skinColor.b/255 ));
        mat.SetColor("_DotsABC", new Color(targetCatSkin.dotsColor.r/255,targetCatSkin.dotsColor.g/255, targetCatSkin.dotsColor.b/255 ));
        mat.SetColor("_DetailsABC", new Color(targetCatSkin.detailsColor.r/255,targetCatSkin.detailsColor.g/255, targetCatSkin.detailsColor.b/255 ));
        playerIcon.material = mat;
    }

    // public virtual void LastLife()
    // {
    //     lastLifeWarnings.gameObject.SetActive(true);
    //     Debug.Log("Activate warning");
    //     StartCoroutine(LastLifeWaitTime());
    // }
    //
    // private IEnumerator LastLifeWaitTime()
    // {
    //     yield return  new  WaitForSeconds(time);
    //     Debug.Log("Deactivate warning");
    //     lastLifeWarnings.gameObject.SetActive(false);
    // }
}
