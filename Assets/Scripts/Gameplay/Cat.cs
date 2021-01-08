using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(UserInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cat : MonoBehaviourPun
{
    public PhotonView pv;
    [SerializeField] [Range(0, 3)] float PITCH=1;
    [SerializeField] private float _speed = 5;
    [SerializeField] [Range(1,5)] private int maxFurLevel=4; //will act like a weight, too (probably?)
    [SerializeField] [Range(1, 5)] private int furLevel;
    [SerializeField] [Range(0,8)] private int maxFurSubLevel=8; //a fur level is composed by many sub-levels as discussed
    [SerializeField] [Range(0,8)] private int furSubLevel; //note that furlevel goes from 1 to max, while fursublevel from 0 to max
    [SerializeField] [Range(1, 9)] private int _hearts;
    [SerializeField] [Range(0, 3)] private float idleTime = 2; //Sorre97: time for player drinking the milk, no input is accepted during it
    [Tooltip("Stun time for the cat hitted by a Jar or jumped on by another cat")]
    [SerializeField] [Range(0,3f)] private float stunTime = 5f;
    [SerializeField] private bool _canBeHit = true;
    [SerializeField] private float invincibility;
    [SerializeField] private float _jumpIntensity = 7.5f;
    [Tooltip("Point where prefab for furball is instantiated")]
    [SerializeField] private Transform _throwPoint;
    [Tooltip("Time to wait between furball throws ")]    
    [SerializeField] private float _throwWaitTime = 0.2f;
    [SerializeField] private bool _isAttacking=false;
    [SerializeField] private GameObject _furBallPrefab;
    [Tooltip("force that it is applicated when hitted by a furball")]
    [SerializeField] private float _pushImpact=2f;
    [SerializeField] private bool _hitted=false;
    [Tooltip("Time to wait to move again after being hitted")]
    [SerializeField] private float _hittedWaitTime = 0.2f;
    [SerializeField] private bool canMove = true;
    [Tooltip("Time to wait between melee attack")]
    [SerializeField] private float _attackWaitTime=0.3f;
    [Tooltip("radius of circle to control melee attack")]
    [SerializeField] private float _radiusMelee = 1;
    [Tooltip("Maximum distance over which to cast the circle")]
    [SerializeField] private float _distanceMelee = 1;

    [Tooltip("force that it is applicated when hitted by a melee")] [SerializeField]
    private float _pushMeleeImpact = 5;

    [SerializeField] private Text identifier;

    private Player _player;
    private BoxCollider2D _boxCollider;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;
    public FeetCollider _feetCollider;
    public Animator animator;
    private SpriteRenderer _SR;
    
    
    private int _number;
    public float velX;
    public float vely;
    private PlayerLifeUI _lifeUI;
    private EnvironmentManager _envi;//reference to environment manager of room;
    [SerializeField] private AnimationTypeController _animationsController;
    
    static System.Random random = new System.Random();
    
    private void Awake()
    { 
        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _feetCollider = GetComponentInChildren<FeetCollider>();
        _SR = GetComponent<SpriteRenderer>();
        furLevel = maxFurLevel;
        furSubLevel = maxFurSubLevel;
        _envi=GameObject.FindWithTag("Manager").GetComponent<EnvironmentManager>();
        _animationsController = GetComponentInChildren<AnimationTypeController>();
        invincibility = 1f;
    }

    private void Start()
    {
        //set life ui for this cat 
        //_number = (photonView.ViewID - 1001) / 1000;
        _number = PhotonNetwork.PlayerList.ToList().IndexOf(photonView.Owner);
        _lifeUI = _envi.playerLifeUis[_number];
        _lifeUI.SetPlayerName(photonView.Owner, _number + 1 );
        if(photonView.Owner.CustomProperties.ContainsKey("SkinID"))
            _lifeUI.SetAvatar((int)photonView.Owner.CustomProperties["SkinID"]);
        _lifeUI.gameObject.SetActive(true);
        identifier.text = "P" + (_number + 1);
    }

    public int GetFurLevel()
    {
        return furLevel;
    }
    public bool CanBeHit
    {
        get => _canBeHit;
    }

    public void setOwner(Player player)
    {
        _player = player;
    }

    public Player getOwner()
    {
        return _player;
    }
    private void Update() {

        //YASEEN: Change Animation based on Furlevel (Not the cleanest way to do it)
        switch (furLevel)
        {
            case 4:
                _animationsController.Set(0);
                break;
            case 3:
                _animationsController.Set(1);
                break;
            case 2:
                _animationsController.Set(2);
                break;
            default:
                _animationsController.Set(3);
                break;
        }
        /*if(furLevel == 4)
        {
            _animationsController.Set(0);
        }
        else if (furLevel == 3)
        {
            _animationsController.Set(1);
        }
        else
        {
            _animationsController.Set(2);
        } */

        if (photonView.IsMine)
        {
            if (canMove && !_isAttacking && userInput.throwInput && (furLevel >1 ) && !_hitted)
            {
                _isAttacking = true;
                photonView.RPC("ThrowRPC", RpcTarget.AllViaServer, _throwPoint.position,_throwPoint.rotation);
                animator.SetTrigger("shooting");
                StartCoroutine(ThrowWait());
            }
            if (canMove && !_isAttacking && userInput.meleeInput && !_hitted)
            {
                _isAttacking = true;
                photonView.RPC("MeleeRPC", RpcTarget.AllViaServer, _throwPoint.position,_throwPoint.rotation,photonView.Owner);
                animator.SetTrigger("attacking");
                StartCoroutine(AttackWait());
                //FindObjectOfType<AudioManager>().Play("cat_angry");
            }
            if (canMove && !_hitted)
            {
                Jump(); 
            }
        }
        /*else
        {
            smoothMovement(); // for other players, then get their new locations and smooth the movements 
        }*/
        //if (random.NextDouble() > 0.9999f)
        //    FindObjectOfType<AudioManager>().Play("cat_purr");




    }
    private IEnumerator AttackWait()
    {   
        Debug.Log(photonView.Owner+" melee");
        yield return new WaitForSeconds(_attackWaitTime);
        _isAttacking = false;
    }


    private void FixedUpdate()
    {
        if (photonView.IsMine && !_hitted && canMove && !_isAttacking)
        {
            Move(); // if we're the player process user input and update location
        }
        
        if (rb.velocity.x < 0)
        {
            identifier.transform.localScale = new Vector2(Math.Abs(identifier.transform.localScale.x), identifier.transform.localScale.y);
        } else if (rb.velocity.x > 0)
        {
            identifier.transform.localScale = new Vector2(-Math.Abs(identifier.transform.localScale.x), identifier.transform.localScale.y);
        }
        
    }



    private void Jump() //simple
    {
        if (_feetCollider.IsOnGround && userInput.jumpInput && rb.velocity.y <= 0.01f && !_hitted && canMove && !_isAttacking)
        {
            rb.AddForce(Vector2.up * (_jumpIntensity - 0.5f*furLevel), ForceMode2D.Impulse);
        }
        if( rb.velocity.y != 0)
        {
            animator.SetBool("jump", true);
        }
        else
        {
            animator.SetBool("jump", false);
        }
        //if(rb.velocity.y < 0)
        //{
        //    rb.AddForce(Vector3.up * (-0.2f*furLevel), ForceMode2D.Impulse);
        //}
    }

    //Check if the player is grounded, otherwise he won't be able to jump
    private void Move() //simple
    {
        float move = userInput.movementInput.x;
        //transform.position += move * _speed * Time.deltaTime;
        rb.velocity=new Vector2(move*_speed,rb.velocity.y);

        //YASEEN: Flip spirit when moving to the other direction
        if (rb.velocity.x < 0)
        {
            rb.transform.localScale = new Vector2(Math.Abs(rb.transform.localScale.x), transform.localScale.y);
            identifier.transform.localScale = new Vector2(Math.Abs(identifier.transform.localScale.x), identifier.transform.localScale.y);
            //_SR.flipX = true;

        } else if (rb.velocity.x > 0)
        {
            rb.transform.localScale = new Vector2(-Math.Abs(rb.transform.localScale.x), transform.localScale.y);
            identifier.transform.localScale = new Vector2(-Math.Abs(identifier.transform.localScale.x), identifier.transform.localScale.y);
            //_SR.flipX = false;
        }

        //YASEEN: Set velocity IN ANIMATOR 
        animator.SetFloat("velocity", Mathf.Abs(rb.velocity.x));
        velX = rb.velocity.x;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void SetCanMove(bool v)
    {
        canMove = v;
    }
    
    
    
    //************************************ BEGIN SECTION for melee attack **********************************************
     
    
    [PunRPC]
    public void MeleeRPC(Vector3 pos, Quaternion q,Photon.Realtime.Player attacker)
    {
        Debug.Log(attacker+" melee");
        Collider2D[] hits;

        bool oppositeDirection = rb.transform.localScale.x > 0;
        Vector2 direction=new Vector2(1,0);
        if (oppositeDirection)
            direction *= -1;
        hits=Physics2D.OverlapCircleAll(pos, _radiusMelee);
        foreach (var hit in hits)
        {
            Cat hitted = hit.GetComponent<Cat>();
            if (hitted && hitted.CanBeHit && !attacker.Equals(hitted.photonView.Owner))
            {
                hitted.MeleeAttacked(direction);
            }
        }
    }


    // Given the furlevel 1 (slim), gives pitch = 2
    // Given the furlevel 4 (fat), gives pitch = 1
    private float getPitch(int furLevel)
    {
        return (-1 / 3f) * furLevel + (7f / 3);
    }
    /// <summary>
    /// Method called when hitted by a melee attack
    /// </summary>
    /// <param name="direction"></param>
    public void MeleeAttacked(Vector2 direction)
    {
        _hitted = true;
        Debug.Log(photonView.Owner+" hitted by a melee");
        rb.Sleep();
        FindObjectOfType<AudioManager>().Play("cat_angry", getPitch(furLevel));
        RemoveFur(4);
        if (furLevel==1 && furSubLevel==0)
        {
            rb.AddForce(direction*_pushMeleeImpact,ForceMode2D.Impulse); 
        }
        StartCoroutine(HitKnockoutTime());
    }

    //********************************* BEGIN SECTION for furball throw ************************************************
    private IEnumerator ThrowWait()
    {
        RemoveFur(4);
        yield return new WaitForSeconds(_throwWaitTime);
        _isAttacking = false;
    }
    
    
    [PunRPC]
    public void ThrowRPC(Vector3 pos, Quaternion q,PhotonMessageInfo info)
    {
        //Debug.Log("Furball RPC");
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        GameObject fb = Instantiate(_furBallPrefab, pos, q);
        fb.GetComponent<SpriteRenderer>().material = GetComponent<SpriteRenderer>().material;

        //YASEEN: 'oppositeDirection' Passes the info of the direction to the lil cute Furball <3 
        bool oppositeDirection =rb.transform.localScale.x > 0;

        fb.GetComponent<FurBall>().SetData(photonView.Owner,Mathf.Abs(lag),oppositeDirection);

    }

    //*************************************** BEGIN SECTION for Fur management *****************************************
    
    public void Drink(GameObject balcony)
    {
        //[Sorre97]: There is SURELY a better way to find the environment manager
        GameObject.FindGameObjectWithTag("Manager").GetComponent<EnvironmentManager>().milkDrinked(balcony);
        FindObjectOfType<AudioManager>().Play("cat_drink_milk", getPitch(furLevel));
        canMove = false;
        StartCoroutine(IdleCoroutine(idleTime));
        furSubLevel = maxFurSubLevel; //a fur level corresponds to the maximum of the sublevels
        if (furLevel == maxFurLevel) {
            
            //Sorre97: if cat has already the max Fur there is something to do?
        } else {
            furLevel++;
        }
        photonView.RPC("FurSyncRPC", RpcTarget.AllViaServer, furLevel, furSubLevel);
    }
    
    private IEnumerator IdleCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        canMove = true;
        yield return null;
    }
    
    public void RemoveFur(int qty) //method to be called by other's melee attack to modify this cat's fur level
    {
        if (furSubLevel > qty) // if the cat will conserve it's fur level
        {
            furSubLevel -= qty;
        }
        else if (furLevel > 1) // when we try to remove more fur than that in the fursublevel, we need to decrease the fur level
        {
            int excess = qty - furSubLevel;
            furSubLevel = maxFurSubLevel;
            furLevel--;
            RemoveFur(excess);
        }
        else if (furLevel == 1) // when the cat is already at minimum fur level and we try to remove more than it has
        {
            furSubLevel = 0;
        }
        photonView.RPC("FurSyncRPC", RpcTarget.AllViaServer, furLevel, furSubLevel);
    }

    [PunRPC] //this rpc is called on each client, if a client notices that it is the one being stunned, it stuns its character
    public void FurSyncRPC(int newFurLevel, int newFurSubLevel)
    {
        furLevel = newFurLevel;
        furSubLevel = newFurSubLevel;
    }

    //************************************* BEGIN SECTION for stun logic ***********************************************
    public void Stun() {
        float duration = stunTime;
        canMove = false; // a stunned cat can't move, should not attack either
        //Debug.Log("I am stunned");
        rb.Sleep();
        StartCoroutine(StunFrame(duration));
        animator.SetBool("stunned", true);
    }
    
    private IEnumerator StunFrame(float duration)
    {
        rb.WakeUp();
        yield return new WaitForSeconds(duration);
        canMove = true;
        animator.SetBool("stunned", false);
        yield return null;
    }


    public void FallOnHead(int otherFurLevel)
    {
        photonView.RPC("JumpOverMyHeadRPC", RpcTarget.Others,  otherFurLevel);
    }
    
    [PunRPC] //this rpc is called on each client, if a client notices that it is the one being stunned, it stuns its character
    public void JumpOverMyHeadRPC(int otherFurLevel)
    {
        Debug.Log("Other fur level: " + otherFurLevel + " my fur level: " + furLevel);
        if(otherFurLevel >= furLevel) Stun();
    }

    //*********************************** BEGIN SECTION for death & lives loss logic ***********************************

    public void Die(String cause) { 
        _hearts -= 1;
        bool canRespawn = FindObjectOfType<EnvironmentManager>().canRespawn();
        if(photonView.IsMine)
            photonView.RPC("RemoveLifeUIRPC",RpcTarget.AllViaServer,_number,_hearts==1);
        if (_hearts == 0 || !canRespawn) {
            print("player <...> died due to: " + cause);
            //lifeUI.gameObject.SetActive(false);
            //gameObject.SetActive(false);
            photonView.RPC("DieRPC",RpcTarget.AllViaServer,photonView.Owner);
            if (!canRespawn)
            {
                FindObjectOfType<EnvironmentManager>().lastStand(photonView.Owner);
            }
        } else {
            Vector3 respawnPoint = Utility.getRandomSpawnLocation();
            switch (cause)
            {
                case "Fall":
                    print("player <...> fell from an high place, " + _hearts + " lives left");

                    //YASEEN: Play sound
                    FindObjectOfType<AudioManager>().Play("scream", getPitch(furLevel));

                    break;
            }

            
            furLevel = maxFurLevel;
            furSubLevel = maxFurSubLevel;
            photonView.RPC("FurSyncRPC", RpcTarget.AllViaServer, furLevel, furSubLevel);
            transform.position = respawnPoint;
            rb.Sleep();
            rb.WakeUp();
            canMove = false;
            _canBeHit = false;
            //StartCoroutine(InvincibilityFrame(invincibility));
        }
    }
    private IEnumerator InvincibilityFrame(float time)
    {
        rb.WakeUp();
        yield return new WaitForSeconds(time);
        canMove = true;
        _canBeHit = true;
        yield return null;
    }

    public void EndInvincibility()
    {
        if (!canMove && !_canBeHit)
        {
            canMove = true;
            _canBeHit = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("trigger with " + other.name);
        
        if (other.CompareTag("FurBall") && _canBeHit && !other.GetComponent<FurBall>().Launcher.Equals(photonView.Owner))
        {
            _hitted = true;
            
            //YASEEN: Play sound
            FindObjectOfType<AudioManager>().Play("scream", getPitch(furLevel));
            //Debug.Log(photonView.Owner+" hitted by a furball");
            //float power = Mathf.Pow((_pushImpact - furLevel), 1.65f);
            float power = _pushImpact * (1 / (float)furLevel)*1.5f;

            rb.Sleep();
            rb.AddForce(other.GetComponent<FurBall>().direction * power, ForceMode2D.Impulse);
            //Debug.Log("force applicate");
            StartCoroutine(HitKnockoutTime());
        }
    }
    private IEnumerator HitKnockoutTime()
    {
        yield return  new WaitForSeconds(_hittedWaitTime);
        _hitted = false;
    }

    [PunRPC]
    public void RemoveLifeUIRPC(int index, bool last)
    {
        _envi.playerLifeUis[index].RemoveLife();
        if(last) _envi.playerLifeUis[index].LastLife();
    }

    [PunRPC]
    public void DieRPC(Player p)
    {
        gameObject.SetActive(false);
        _envi.VictoryCheck(p);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(_throwPoint.position,_radiusMelee);
    }

    [PunRPC]
    public void setCatSkinRPC(int skinID)
    {
        GetComponent<SpriteRenderer>().material = new Material(GetComponent<SpriteRenderer>().material);
        GetComponent<SpriteRenderer>().material.SetColor("_SkinABC", new Color(CatSkins.catSkinsList[skinID].skinColor.r/255,CatSkins.catSkinsList[skinID].skinColor.g/255, CatSkins.catSkinsList[skinID].skinColor.b/255 ));
        GetComponent<SpriteRenderer>().material.SetColor("_DotsABC", new Color(CatSkins.catSkinsList[skinID].dotsColor.r/255,CatSkins.catSkinsList[skinID].dotsColor.g/255, CatSkins.catSkinsList[skinID].dotsColor.b/255 ));
        GetComponent<SpriteRenderer>().material.SetColor("_DetailsABC", new Color(CatSkins.catSkinsList[skinID].detailsColor.r/255,CatSkins.catSkinsList[skinID].detailsColor.g/255, CatSkins.catSkinsList[skinID].detailsColor.b/255 ));
    }
    
    [PunRPC]
    public void Countdown()
    {
        StartCoroutine(_envi.CountdownCoroutine(this));
    }
} 
