﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SocialPlatforms;



/*
 NOTE:
Currently, the code assumes the pre-existance of balconies.
it spawns the cat above a random balcony. 
(Doesn't check if there's a cat already there)
 
 */
public class EnvironmentManager : MonoBehaviourPun
{
    public GameObject catPrefab;
    public GameObject milkPrefab;

    [SerializeField]  private int _maxMilkSpawned = 20;
    [SerializeField]  private float _timeBetweenMilkSpawns = 2f;
    [SerializeField] private float firstMilkDelay = 5f;
    [SerializeField] private float counter;

    void Start()
    {
        CatSpawn();
        if (PhotonNetwork.IsMasterClient) //YASEEN: If I'm the owner of the scene ? 
            StartCoroutine(spawnMilk(_maxMilkSpawned, firstMilkDelay));
        else
            Debug.Log("NOT MASTER");
    }

    private void CatSpawn()
    {

        var spawnPosition = Utility.getRandomSpawnLocation();
        GameObject cat = PhotonNetwork.Instantiate(catPrefab.name, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
        //Instantiate(catPrefab, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame

    private IEnumerator spawnMilk(int maxNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        WaitForSeconds wait = new WaitForSeconds(_timeBetweenMilkSpawns);
        foreach (int value in System.Linq.Enumerable.Range(1, maxNumber))
        {
            counter = value;
            FindObjectOfType<AudioManager>().Play("milkDrop");
            var spawnPosition = Utility.getRandomSpawnLocation() + Utility.GetRandomVector3(-1.5f,1.5f);
            PhotonNetwork.Instantiate(milkPrefab.name, spawnPosition, milkPrefab.transform.rotation); //spawn milk :)
            //Debug.Log("MILK COUNT - " + value);
            yield return wait;
        }
    }

}