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
 NOTE 2: [Sorre97]
Currently, the code for checks on borders and random spawning are
hardcoded, we should pass "background" to this and use it to create
the borders
 */
public class EnvironmentManager : MonoBehaviourPun
{
    public GameObject catPrefab;
    public GameObject milkPrefab;
    public GameObject jarPrefab;

    [SerializeField]  private int maxMilkSpawned = 3;
    [SerializeField]  private float timeBetweenMilk = 2f;
    [SerializeField]  private float timeBetweenJars = 5f;
    [SerializeField]  private float counter;
    [SerializeField] [Range(0, 5f)] private float milkHeight = 1.28f;
    [SerializeField] [Range(0, 20f)] private float jarHeight = 10f;  //TODO this should be based on background y

    [SerializeField] private int milkCounter = 0;
    private List<GameObject> _balconies;
    private List<GameObject> _freeBalconies;
    //private List<Cat> _cats;
    
    void Start()
    {
        CatSpawn();
        if (PhotonNetwork.IsMasterClient)
        {
            //YASEEN: If I'm the owner of the scene ? 
            StartCoroutine(spawnMilk());
            StartCoroutine(spawnJars());
        }
        else
            Debug.Log("NOT MASTER");
        
        _balconies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Balcony"));
        _freeBalconies = _balconies;
        //_cats = new List<Cat>();
    }

    private void CatSpawn()
    {

        var spawnPosition = Utility.getRandomSpawnLocation();
        GameObject cat = PhotonNetwork.Instantiate(catPrefab.name, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
        //_cats.Add(cat.GetComponent<Cat>());  TODO [Sorre97] THIS DOESN'T WORK FOR SOME REASON!
        //Instantiate(catPrefab, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame

    private IEnumerator spawnJars()
    {
        yield return new WaitForSeconds(timeBetweenJars);
        GameObject[] cats = GameObject.FindGameObjectsWithTag("Player");
        if (cats.Length != 0)
        {
            System.Random rnd = new System.Random();
            int rnd_num = rnd.Next(0, cats.Length);
            var spawnPosition = Utility.getRandomJarSpawn(cats[rnd_num].GetComponent<Transform>().position, jarHeight);
            PhotonNetwork.Instantiate(jarPrefab.name, spawnPosition, jarPrefab.transform.rotation);
            yield return new WaitForSeconds(timeBetweenJars);  //TODO [Sorre97] Height should be retrieved by the background y
        }
        StartCoroutine(spawnJars());
        yield return null;
    }
    
    private IEnumerator spawnMilk()
    {
        yield return new WaitForSeconds(timeBetweenMilk);
        //[Sorre97]: New implementation for Milk Spawning, instead of spawning 20 milks and stop, now it spawns milk if there is less than maxMilkSpawned
        if (milkCounter < maxMilkSpawned && _freeBalconies.Count != 0)
        {
            FindObjectOfType<AudioManager>().Play("milkDrop");
            System.Random rnd = new System.Random();
            int rnd_num = rnd.Next(0, _freeBalconies.Count);
            var balcony = _freeBalconies[rnd_num];
            var spawnPosition = Utility.getRandomMilkSpawn(milkHeight, balcony);
            _freeBalconies.Remove(balcony);
            GameObject milk = PhotonNetwork.Instantiate(milkPrefab.name, spawnPosition, milkPrefab.transform.rotation);
            milkCounter++;
            milk.GetComponent<Milk>().setBalcony(balcony);
            //Debug.Log("MILK COUNT - " + value);
        }
        yield return new WaitForSeconds(timeBetweenMilk);
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
        milkCounter--;
        _freeBalconies.Add(balcony);

    }
}
