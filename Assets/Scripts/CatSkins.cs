using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CatSkin
{
    public string name;
    public int baseSkinID;
    public Color skinColor;
    public Color dotsColor;
    public Color detailsColor;

    public CatSkin(string name, int baseSkinID, Color skinColor, Color dotsColor, Color detailsColor)
    {
        this.name = name;
        this.baseSkinID = baseSkinID;
        this.skinColor = skinColor;
        this.dotsColor = dotsColor;
        this.detailsColor = detailsColor;
    }
}

public static class CatSkins
{
    public static CatSkin[] catSkinsList = new[]
    {
        new CatSkin("White", 
            0, 
            new Color(255,255,255), 
            new Color(0,0,0), 
            new Color(0,0,0)),
        new CatSkin("Gray", 
            1, 
            new Color(118,117,118), 
            new Color(92,92,92), 
            new Color(0,0,0)),
        new CatSkin("Siamese", 
            2, 
            new Color(239,210,159), 
            new Color(93,66,55), 
            new Color(0,0,0)),
        new CatSkin("Orange", 
            1, 
            new Color(217,105,19), 
            new Color(164,78,19), 
            new Color(0,0,0)),
        new CatSkin("Cream", 
            0, 
            new Color(240,210,160), 
            new Color(0,0,0), 
            new Color(0,0,0)),
        new CatSkin("Chartreux", 
            0, 
            new Color(165,176,198), 
            new Color(0,0,0), 
            new Color(0,0,0)),
        new CatSkin("Brown", 
            1, 
            new Color(150,120,84), 
            new Color(113,91,68), 
            new Color(0,0,0)),
        new CatSkin("Gray Siamese", 
            2, 
            new Color(255,255,255), 
            new Color(142,142,142), 
            new Color(0,0,0)),
        new CatSkin("Orange Siamese", 
            2, 
            new Color(240,210,160), 
            new Color(219,105,6), 
            new Color(0,0,0)),
    };
}
