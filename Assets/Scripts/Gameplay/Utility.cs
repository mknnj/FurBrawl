﻿using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
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

    public static Vector3 getRandomMilkSpawn(float y, GameObject balcony)
    {
        float x = Random.Range(-1.5f, 1.5f);  //TODO [Sorre97] make program aware of balconies size, not hardcoded
        var spawnPosition = balcony.transform.position + new Vector3(x, y, 0);
        return spawnPosition;
    }
    
    public static Vector3 getRandomJarSpawn(Vector3 position, float height)
    {
        float x = Random.Range(-2.5f, 2.5f); //TODO [Sorre97] hardcoded value, have to make check on borders too
        var spawnPosition = position + new Vector3(x, height, 0);
        return spawnPosition;
    }
    public static Vector3 GetRandomVector3(float min, float max)
    {
        float x = Random.Range(min, max);
        float z = Random.Range(min, max);
        float y = Random.Range(min, max);
   
        return new Vector3(x,y,z);
    }

}
