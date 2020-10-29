using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

    public static Vector3 getRandomSpawnLocation()
    {
        GameObject[] balconies = GameObject.FindGameObjectsWithTag("Balcony"); 
        System.Random rnd = new System.Random();
        int rnd_num = rnd.Next(0, balconies.Length);
        var balcony = balconies[rnd_num];
        var spawnPosition = balcony.transform.position + new Vector3(0, 5, 0);
        return spawnPosition;
    }

}
