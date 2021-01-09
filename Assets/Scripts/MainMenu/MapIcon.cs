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
        GetComponent<Outline>().enabled = true;
    }
    
    public void Deselect()
    {
        GetComponent<Outline>().enabled = false;
    }

    public void OnClick_MyBtn()
    {
        parent.Clicked(mapID);
    }
    
    
}
