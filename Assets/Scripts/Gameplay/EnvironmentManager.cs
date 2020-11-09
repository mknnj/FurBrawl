using System;
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

    [SerializeField]  private int maxMilkSpawned = 3;
    [SerializeField]  private float _timeBetweenMilkSpawns = 2f;
    [SerializeField] private float counter;
    [SerializeField] [Range(0, 5f)] private float milkHeight = 1;

    [SerializeField] private int _milkCounter = 0;
    private List<GameObject> _balconies;
    private List<GameObject> _freeBalconies;
    
    void Start()
    {
        CatSpawn();
        if (PhotonNetwork.IsMasterClient) //YASEEN: If I'm the owner of the scene ? 
            StartCoroutine(spawnMilk());
        else
            Debug.Log("NOT MASTER");
        
        _balconies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Balcony"));
        _freeBalconies = _balconies;
    }

    private void CatSpawn()
    {

        var spawnPosition = Utility.getRandomSpawnLocation();
        GameObject cat = PhotonNetwork.Instantiate(catPrefab.name, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
        //Instantiate(catPrefab, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame

    private IEnumerator spawnMilk()
    {
        yield return new WaitForSeconds(_timeBetweenMilkSpawns);
        //[Sorre97]: New implementation for Milk Spawning, instead of spawning 20 milks and stop, now it spawns milk if there is less than maxMilkSpawned
        if (_milkCounter < maxMilkSpawned && _freeBalconies.Count != 0)
        {
            FindObjectOfType<AudioManager>().Play("milkDrop");
            System.Random rnd = new System.Random();
            int rnd_num = rnd.Next(0, _freeBalconies.Count);
            var balcony = _freeBalconies[rnd_num];
            var spawnPosition = Utility.getRandomMilkSpawn(milkHeight, balcony);
            _freeBalconies.Remove(balcony);
            GameObject milk = PhotonNetwork.Instantiate(milkPrefab.name, spawnPosition, milkPrefab.transform.rotation);
            _milkCounter++;
            milk.GetComponent<Milk>().setBalcony(balcony);
            //Debug.Log("MILK COUNT - " + value);
        }
        yield return new WaitForSeconds(_timeBetweenMilkSpawns);
        StartCoroutine(spawnMilk());

        /* [Sorre97]: Old implementation of milk spawning, i didn't delete it
         
        foreach (int value in System.Linq.Enumerable.Range(1, maxNumber))
        {
            counter = value;
            FindObjectOfType<AudioManager>().Play("milkDrop");
            System.Random rnd = new System.Random();
            int rnd_num = rnd.Next(0, _freeBalconies.Count);
            var balcony = _freeBalconies[rnd_num];
            var spawnPosition = Utility.getRandomMilkSpawn(milkHeight, balcony);
            _freeBalconies.Remove(balcony);
            GameObject milk = PhotonNetwork.Instantiate(milkPrefab.name, spawnPosition, milkPrefab.transform.rotation);
            _milkCounter++;
            milk.GetComponent<Milk>().setBalcony(balcony);
            //Debug.Log("MILK COUNT - " + value);
            yield return wait;
        } */
    }

    public void milkDrinked(GameObject balcony)
    {
        _milkCounter--;
        _freeBalconies.Add(balcony);

    }
}
