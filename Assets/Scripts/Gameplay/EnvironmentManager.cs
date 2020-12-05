using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




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
    public GameObject[] catPrefabs;
    public GameObject milkPrefab;
    public GameObject jarPrefab;
    public GameObject platformPrefab;

    [SerializeField]  private int maxMilkSpawned = 3;
    [SerializeField]  private float timeBetweenMilk = 2f;
    [SerializeField]  private float timeBetweenJars = 5f;
    [SerializeField]  private float timeBetweenPlatforms = 10f;
    [SerializeField]  private float counter;
    [SerializeField] [Range(0, 5f)] private float milkHeight = 0.46f;
    [SerializeField] [Range(0, 20f)] private float jarHeight = 10f;  //TODO this should be based on background y

    [SerializeField] private int milkCounter = 0;

    public PlayerLifeUI[] playerLifeUis; //0 is topLeft, 1 is topRight, 2 is bottomLeft, 3 is bottomRight
    private List<GameObject> _balconies;
    private List<GameObject> _freeBalconies;
    
    private List<Vector3> _freePlatforms;

    private int _numberPlayerConnected;
    private List<Player> _players;

    [SerializeField] private Text _victoryText;
    
    //private List<Cat> _cats;

    void Start()
    {
        _victoryText.gameObject.SetActive(false);
        int id = CatSpawn();
        Debug.Log(id+ " logged");
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
        //    Debug.LogWarning("MASTER SWITCHED");
        //    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        //}

        if (PhotonNetwork.IsMasterClient)
        {
            //YASEEN: If I'm the owner of the scene ? 
            StartCoroutine(spawnMilk());
            StartCoroutine(spawnJars());
            StartCoroutine(spawnPlatforms());
            Debug.LogWarning("MASTER CONNECTED");
            FindObjectOfType<AudioManager>().Play("scream");
        }
        else
            Debug.LogWarning("NOT MASTER");
        
        _balconies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Balcony"));
        _freeBalconies = _balconies;

        _freePlatforms = new List<Vector3>();
        //_cats = new List<Cat>();
        _players =  PhotonNetwork.PlayerList.ToList();
        _numberPlayerConnected=_players.Count;
        
    }

    private int CatSpawn()
    {
        //TODO Spawn the prefab corresponding to LocalPlayer.CustomProperty["SkinID"]
        var spawnPosition = Utility.getRandomSpawnLocation();
        GameObject cat = PhotonNetwork.Instantiate(catPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["SkinID"]].name, spawnPosition, catPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["SkinID"]].transform.rotation); //spawn a cat :)
        PhotonView photon_view = cat.GetComponent<PhotonView>();
        //cat.GetComponent<Cat>().envi = this;
        return photon_view.ViewID;
        //_cats.Add(cat.GetComponent<Cat>());  TODO [Sorre97] THIS DOESN'T WORK FOR SOME REASON!
        //Instantiate(catPrefab, spawnPosition, catPrefab.transform.rotation); //spawn a cat :)
    }

    // Update is called once per frame


    private IEnumerator spawnPlatforms()
    {
        yield return new WaitForSeconds(timeBetweenPlatforms);
        if (_freePlatforms.Count > 0)
        {
            System.Random rnd = new System.Random();
            int rnd_num = rnd.Next(0, _freePlatforms.Count);
            Vector3 platform = _freePlatforms[rnd_num];
            _freePlatforms.Remove(platform);
            PhotonNetwork.Instantiate(platformPrefab.name, platform, platformPrefab.transform.rotation);
            
        }
        //var location_1 = new Vector3(0.009512998f, -2.128334f, -8.9f);
        //var location_2 = new Vector3(4.32f, 2.83f, -8.9f);

        //PhotonNetwork.Instantiate(platformPrefab.name, location_1, platformPrefab.transform.rotation);
        //PhotonNetwork.Instantiate(platformPrefab.name, location_2, platformPrefab.transform.rotation);
        yield return null;
        StartCoroutine(spawnPlatforms());
    }

    public void destroyedPlatform(float xAxis, float yAxis)
    {
        _freePlatforms.Add(new Vector3(xAxis, yAxis, -8.9f));
    }

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

    public void VictoryCheck(Player player)
    {
        if (_numberPlayerConnected <= 1) return;
        int c = _players.Count;
        if (c>1)
            _players.Remove(player);
        if ( c-1< 2)
        {
            Debug.Log(_players[0].NickName+ " Wins");
            _victoryText.text = (_players[0].NickName + "\nWins");
            _victoryText.gameObject.SetActive(true);
            StartCoroutine(exitRoom());
        }

    }

    IEnumerator exitRoom()
    {
        yield return new WaitForSeconds(10);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    
}
