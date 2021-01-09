using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour
{
    [SerializeField] private MapSelector parent;
    public int mapID;

    public void SetData(Sprite baseIcon, MapSelector p, int id)
    {
        parent = p;
        mapID = id;
        GetComponent<Image>().sprite = baseIcon;
    }

    public void Select()
    {
        GetComponent<Image>().color = new Color(GetComponent<Image>().color.r ,GetComponent<Image>().color.g, GetComponent<Image>().color.b, 1f);
        //GetComponent<Outline>().enabled = true;
    }
    
    public void Deselect()
    {
        GetComponent<Image>().color = new Color(GetComponent<Image>().color.r ,GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0.5f);
        //GetComponent<Outline>().enabled = false;
    }

    public void OnClick_MyBtn()
    {
        parent.Clicked(mapID);
    }
    
    
}
