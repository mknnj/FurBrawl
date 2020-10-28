using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject catPrefab;
  
    void Start()
    {
        CatSpawn();   
    }

    private void CatSpawn()
    {
        PhotonNetwork.Instantiate(catPrefab.name, catPrefab.transform.position, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame

}
