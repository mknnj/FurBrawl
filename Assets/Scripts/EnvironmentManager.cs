using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



/*
 NOTE:
Currently, the code assumes the pre-existance of balconies.
it spawns the cat above a random balcony. 
(Doesn't check if there's a cat already there)
 
 */
public class EnvironmentManager : MonoBehaviour
{
    public GameObject catPrefab;

    void Start()
    {
        CatSpawn();
    }

    private void CatSpawn()
    {

        var spawnPosition = Utility.getRandomSpawnLocation();
        PhotonNetwork.Instantiate(catPrefab.name, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
        //Instantiate(catPrefab, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame

}
