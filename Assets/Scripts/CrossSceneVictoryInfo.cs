using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public static class CrossSceneVictoryInfo 
{
    //Class used to pass infos about the victory of a player from the gameplay to the main menu scene 
    public static Player Winner = null;

    public static Player GetWinner()
    {
        return Winner;
    }
    
    public static void SetWinner(Player winner)
    {
        Winner = winner;
    }
}
