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
using Random = UnityEngine.Random;


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
public class EnvironmentManager : MonoBehaviourPunCallbacks
{
    public GameObject[] catPrefabs;
    public Material[] catMaterials;
    public GameObject[] milkPrefabsList;
    public GameObject[] goldenMilkPrefabsList;
    public GameObject[] jarPrefabsList;
    public GameObject[] platformPrefabsList;
    [SerializeField] private GameObject[] mapPrefabList;
    [SerializeField] private GameObject lastLifeImage;

    [SerializeField]  private int maxMilkSpawned = 3;
    [SerializeField]  private float timeBetweenMilk = 2f;
    [SerializeField]  private float timeBetweenJars = 5f;
    [SerializeField]  private float timeBetweenPlatforms = 10f;
    [SerializeField]  private float counter;
    [SerializeField] [Range(0, 5f)] private float milkHeight = 0.46f;
    [SerializeField] [Range(0, 20f)] private float jarHeight = 10f;  //TODO this should be based on background y
    [SerializeField] [Range(0, 1f)] private float goldenMilkProbability = 0.1f;

    [SerializeField] private int milkCounter = 0;

    public PlayerLifeUI[] playerLifeUis; //0 is topLeft, 1 is topRight, 2 is bottomLeft, 3 is bottomRight
    private List<GameObject> _balconies;
    private List<GameObject> _freeBalconies;
    
    private List<Vector3> _freePlatforms;

    private int _numberPlayerConnected;
    private List<Player> _players;
    private bool _respawn = true;
    private List<Player> _lastStandPlayers;
    
    [SerializeField] private Text _victoryText;

    [SerializeField] private Image[] startingCountdown;

    [SerializeField] private float timeCountdown = 0.5f;
    //private List<Cat> _cats;

    void Start()
    {
        Cursor.visible = false;
        lastLifeImage.SetActive(false);
        Instantiate(mapPrefabList[CrossSceneMapInfo.MapID]);
        _victoryText.gameObject.SetActive(false);
        PhotonView photon = CatSpawn();
        int id = photon.ViewID;
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
            photon.RPC("Countdown",RpcTarget.AllViaServer);
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

    public bool canRespawn()
    {
        return _respawn;
    }
    
    private PhotonView CatSpawn()
    {
        var spawnPosition = Utility.getRandomSpawnLocation();
        int skinID = (int) PhotonNetwork.LocalPlayer.CustomProperties["SkinID"];
        int baseSkinID = CatSkins.catSkinsList[skinID].baseSkinID;
        GameObject cat = PhotonNetwork.Instantiate(catPrefabs[baseSkinID].name, spawnPosition, catPrefabs[baseSkinID].transform.rotation); //spawn a cat :)
        PhotonView photon_view = cat.GetComponent<PhotonView>();
        photon_view.RPC("setCatSkinRPC",RpcTarget.AllViaServer, skinID);
        cat.GetComponent<Cat>().SetUINameColor(Color.white);
        //cat.GetComponent<Cat>().envi = this;
        return photon_view;
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
            PhotonNetwork.Instantiate(platformPrefabsList[CrossSceneMapInfo.MapID].name, platform, platformPrefabsList[CrossSceneMapInfo.MapID].transform.rotation);
            
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
            PhotonNetwork.Instantiate(jarPrefabsList[CrossSceneMapInfo.MapID].name, spawnPosition, jarPrefabsList[CrossSceneMapInfo.MapID].transform.rotation);
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

            if (Random.Range(0f, 1) > goldenMilkProbability)
            {
                GameObject milk = PhotonNetwork.Instantiate(milkPrefabsList[CrossSceneMapInfo.MapID].name,
                    spawnPosition, milkPrefabsList[CrossSceneMapInfo.MapID].transform.rotation);
                milk.GetComponent<Milk>().setBalcony(balcony, false);
            }
            else
            {
                GameObject milk = PhotonNetwork.Instantiate(goldenMilkPrefabsList[CrossSceneMapInfo.MapID].name, spawnPosition, milkPrefabsList[CrossSceneMapInfo.MapID].transform.rotation);
                milk.GetComponent<Milk>().setBalcony(balcony, true);
            }
            milkCounter++;
                
            
            //Debug.Log("MILK COUNT - " + value);
        }
        yield return new WaitForSeconds(timeBetweenMilk);
        StartCoroutine(spawnMilk());
    }

    public void milkDrinked(GameObject balcony)
    {
        milkCounter--;
        _freeBalconies.Add(balcony);

    }

    public void SetLastLife()
    {
        StartCoroutine(LastLifeCoroutine());
    }

    private IEnumerator LastLifeCoroutine()
    {
        lastLifeImage.SetActive(true);
        yield return new WaitForSeconds(1f);
        lastLifeImage.SetActive(false);
    }

    public void VictoryCheck(Player player)
    {
        if (_numberPlayerConnected <= 1) return;
        int c = _players.Count;
        if (c>1)
            _players.Remove(player);
        if ( c-1< 2)
        {
            Debug.Log("[Victory] Victory called from normal death");
            victoryScreen(_players[0]);
        }
    }

    private void victoryScreen(Player winner)
    {
        Debug.Log(winner.NickName + " Wins");
        _victoryText.text = (winner.NickName + "\nWins");
        _victoryText.gameObject.SetActive(true);
            
        CrossSceneVictoryInfo.SetWinner(winner);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(0);
        }
        //Cursor.visible = true;
        //StartCoroutine(exitRoom());
    }
    
    IEnumerator exitRoom()
    {
        yield return new WaitForSeconds(10);
        //Cursor.visible = true;
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    

    public IEnumerator CountdownCoroutine()
    {
        FindObjectOfType<AudioManager>().Play("countdown_sound");
        GameObject[] cats = GameObject.FindGameObjectsWithTag("Player");
        foreach (var cat in cats)
            cat.GetComponent<Cat>().SetCanMove(false);
        
        startingCountdown[0].gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(timeCountdown);
        
        startingCountdown[0].gameObject.SetActive(false);
        startingCountdown[1].gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(timeCountdown);
        
        startingCountdown[1].gameObject.SetActive(false);
        startingCountdown[2].gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(timeCountdown);
        
        startingCountdown[2].gameObject.SetActive(false);
        foreach (var cat in cats)
            cat.GetComponent<Cat>().SetCanMove(true);
        gameObject.GetComponent<Timer>().customStart();
    }

    public void endGame()
    {
        int max = -1;
        List<Player> evenWinners = new List<Player>();
        Player winner = null;
        int winners = 0;

        for (int counter = 0; counter < _numberPlayerConnected; counter++)
        {
            PlayerLifeUI player = playerLifeUis[counter];
            Debug.Log("[endGame] Player " + player.getOwner().NickName + " has " + player.getLives() + " lives left");
            if (player.getLives() > max)
            {
                Debug.Log("[endGame] new Winner is " + player.getOwner().NickName);
                max = player.getLives();
                winner = player.getOwner();
                evenWinners.Clear();
                evenWinners.Add(player.getOwner());
                winners = 1;
            }
            else if (player.getLives() == max)
            {
                winners++;
                evenWinners.Add(player.getOwner());
            }
        }

        if (winners == 1)
            victoryScreen(winner);
        else
        {
            gameObject.GetComponent<Timer>().setLastStand();
            _respawn = false;
            _lastStandPlayers = evenWinners;
            Debug.Log("[LastStand] Even winners are:");
            foreach (Player player in _lastStandPlayers)
            {
                Debug.Log("[LastStand] - " + player.NickName);
            }
        }
    }

    public void lastStand(Player player)
    {
        if (!_respawn)
        {
            if (_lastStandPlayers.Contains(player))
            {
                Debug.Log("[LastStand] Critical Player fell: " + player.NickName);
                _lastStandPlayers.Remove(player);
                if (_lastStandPlayers.Count == 1)
                {
                    Debug.Log("[Victory] Victory called from last stand death");
                    victoryScreen(_lastStandPlayers[0]);
                }
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //Cursor.visible = true;
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    
    
}
