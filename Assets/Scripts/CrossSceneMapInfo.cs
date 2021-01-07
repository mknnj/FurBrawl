using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CrossSceneMapInfo 
{
    //Class used to pass infos about the victory of a player from the gameplay to the main menu scene 
    public static int MapID = 0;

    public static int GetMapID()
    {
        return MapID;
    }
    
    public static void SetMapID(int mapID)
    {
        MapID = mapID;
    }
}
