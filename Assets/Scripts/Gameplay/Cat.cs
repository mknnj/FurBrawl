using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(UserInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cat : MonoBehaviourPun
{
    public PhotonView pv;
    [SerializeField] private float _speed = 5;
    [SerializeField] [Range(1,5)] private int maxFurLevel=4; //will act like a weight, too (probably?)
    [SerializeField] [Range(1, 5)] private int furLevel;
    [SerializeField] [Range(0,8)] private int maxFurSubLevel=8; //a fur level is composed by many sub-levels as discussed
    [SerializeField] [Range(0,8)] private int furSubLevel; //note that furlevel goes from 1 to max, while fursublevel from 0 to max
    [SerializeField] [Range(1, 9)] private int _hearts;
    [SerializeField] [Range(0, 3)] private float idleTime = 2; //Sorre97: time for player drinking the milk, no input is accepted during it
    [Tooltip("Stun time for the cat hitted by a Jar or jumped on by another cat")]
    [SerializeField] [Range(0,3f)] private float stunTime = 1f;
    [SerializeField] private bool _canBeHit = true;
    public bool CanBeHit
    {
        get => _canBeHit;
    }
    [SerializeField] private float invincibility;
    [SerializeField] private float _jumpIntensity = 5f;
    [Tooltip("Point where prefab for furball is instantiated")]
    [SerializeField] private Transform _throwPoint;
    [Tooltip("Time to wait between furball throws ")]    
    [SerializeField] private float _throwWaitTime = 0.2f;
    [SerializeField] private bool _isAttacking=false;
    [SerializeField] private GameObject _furBallPrefab;
    [Tooltip("force that it is applicated when hitted by a furball")]
    [SerializeField] private float _pushImpact=5f;
    [SerializeField] private bool _hitted=false;
    [Tooltip("Time to wait to move again after being hitted")]
    [SerializeField] private float _hittedWaitTime = 0.2f;
    [SerializeField] private bool canMove = true;
    [Tooltip("Time to wait between melee attack")]
    [SerializeField] private float _attackWaitTime;
    [Tooltip("radius of circle to control melee attack")]
    [SerializeField] private float _radiusMelee = 1;
    [Tooltip("Maximum distance over which to cast the circle")]
    [SerializeField] private float _distanceMelee = 1;

    [Tooltip("force that it is applicated when hitted by a melee")] [SerializeField]
    private float _pushMeleeImpact = 5;
    
    private BoxCollider2D _boxCollider;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;
    public FeetCollider _feetCollider;
    public Animator animator;
    private SpriteRenderer _SR;

    public float velX;
    public float vely;

    private void Awake()
    { 
        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _feetCollider = GetComponentInChildren<FeetCollider>();
        _SR = GetComponent<SpriteRenderer>();
        furLevel = maxFurLevel;
        furSubLevel = maxFurSubLevel;
    }

    public int GetFurLevel()
    {
        return furLevel;
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            if (canMove && !_isAttacking && userInput.throwInput && (furLevel >1 || furSubLevel>0))
            {
                _isAttacking = true;
                photonView.RPC("ThrowRPC", RpcTarget.AllViaServer, _throwPoint.position,_throwPoint.rotation);
                animator.SetTrigger("shooting");
                StartCoroutine(ThrowWait());
            }
            if (canMove && !_isAttacking && userInput.meleeInput)
            {
                _isAttacking = true;
                photonView.RPC("MeleeRPC", RpcTarget.AllViaServer, _throwPoint.position,_throwPoint.rotation,photonView.Owner);
                StartCoroutine(AttackWait());
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
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine && !_hitted && canMove)
        {
            Move(); // if we're the player process user input and update location
        }
    }



    private void Jump() //simple
    {
        if (_feetCollider.IsOnGround && userInput.jumpInput)
        {
            rb.AddForce(Vector3.up * (_jumpIntensity - maxFurLevel), ForceMode2D.Impulse);
            
        }
        if( rb.velocity.y != 0)
        {
            animator.SetBool("jump", true);
        }
        else
        {
            animator.SetBool("jump", false);
        }
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
            //_SR.flipX = true;

        } else if (rb.velocity.x > 0)
        {
            rb.transform.localScale = new Vector2(-Math.Abs(rb.transform.localScale.x), transform.localScale.y);
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
    
    /// <summary>
    /// Method called when hitted by a melee attack
    /// </summary>
    /// <param name="direction"></param>
    public void MeleeAttacked(Vector2 direction)
    {
        _hitted = true;
        Debug.Log(photonView.Owner+" hitted by a melee");
        rb.Sleep();
        RemoveFur(4);
        if (furLevel==1 && furSubLevel==0)
        {
            rb.AddForce(direction*_pushMeleeImpact,ForceMode2D.Impulse); 
        }
        StartCoroutine(HitKnockoutTime());
    }
    
    public void Drink(GameObject balcony)
    {
        //[Sorre97]: There is SURELY a better way to find the environment manager
        GameObject.FindGameObjectWithTag("Manager").GetComponent<EnvironmentManager>().milkDrinked(balcony);
        
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
    
    private IEnumerator IdleCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        canMove = true;
        yield return null;
    }
    
    public void Die(String cause) { 
        _hearts -= 1; 
        if (_hearts == 0) {
            print("player <...> died due to: " + cause);
            gameObject.SetActive(false);
        } else {
            Vector3 respawnPoint = Utility.getRandomSpawnLocation();
            switch (cause)
            {
                case "Fall":
                    print("player <...> fell from an high place, " + _hearts + " lives left");

                    //YASEEN: Play sound
                    FindObjectOfType<AudioManager>().Play("scream");

                    break;
            }

            _canBeHit = false;
            furLevel = maxFurLevel;
            furSubLevel = maxFurSubLevel;
            photonView.RPC("FurSyncRPC", RpcTarget.AllViaServer, furLevel, furSubLevel);
            transform.position = respawnPoint;
            StartCoroutine(InvincibilityFrame(invincibility));
        }
    }

    public void Stun()
    {
        float duration = stunTime;
        canMove = false; // a stunned cat can't move, should not attack either
        //Debug.Log("I am stunned");
        StartCoroutine(StunFrame(duration));
        animator.SetBool("stunned", true);
    }

    public void FallOnHead(int otherFurLevel)
    {
        photonView.RPC("JumpOverMyHeadRPC", RpcTarget.Others,  otherFurLevel);
    }
    
    private IEnumerator StunFrame(float duration)
    {
        yield return new WaitForSeconds(duration);
        canMove = true;
        animator.SetBool("stunned", false);
        yield return null;
    }
    
    
    private IEnumerator InvincibilityFrame(float time)
    {
        yield return new WaitForSeconds(time);
        
        _canBeHit = true;
        yield return null;
    }

    private IEnumerator ThrowWait()
    {
        RemoveFur(4);
        yield return new WaitForSeconds(_throwWaitTime);
        _isAttacking = false;
    }

    private IEnumerator AttackWait()
    {   
        Debug.Log(photonView.Owner+" melee");
        yield return new WaitForSeconds(_attackWaitTime);
        _isAttacking = false;
    }

    private IEnumerator HitKnockoutTime()
    {
        yield return  new WaitForSeconds(_hittedWaitTime);
        _hitted = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("trigger with " + other.name);
        
        if (other.CompareTag("FurBall") && _canBeHit)
        {
            _hitted = true;
            
            //YASEEN: Play sound
            FindObjectOfType<AudioManager>().Play("scream");
            //Debug.Log(photonView.Owner+" hitted by a furball");
            rb.Sleep();
            rb.AddForce(other.GetComponent<FurBall>().direction * _pushImpact, ForceMode2D.Impulse);
            //Debug.Log("force applicate");
            StartCoroutine(HitKnockoutTime());
        }
    }

    [PunRPC]
    public void ThrowRPC(Vector3 pos, Quaternion q,PhotonMessageInfo info)
    {
        //Debug.Log("Furball RPC");
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        GameObject fb = Instantiate(_furBallPrefab, pos, q);


        //YASEEN: 'oppositeDirection' Passes the info of the direction to the lil cute Furball <3 
        bool oppositeDirection =rb.transform.localScale.x > 0;

        fb.GetComponent<FurBall>().SetData(photonView.Owner,Mathf.Abs(lag),oppositeDirection);
    }
    
    [PunRPC] //this rpc is called on each client, if a client notices that it is the one being stunned, it stuns its character
    public void JumpOverMyHeadRPC(int otherFurLevel)
    {
        Debug.Log("Other fur level: " + otherFurLevel + " my fur level: " + furLevel);
        if(otherFurLevel >= furLevel) Stun();
    }
    
    [PunRPC] //this rpc is called on each client, if a client notices that it is the one being stunned, it stuns its character
    public void FurSyncRPC(int newFurLevel, int newFurSubLevel)
    {
        furLevel = newFurLevel;
        furSubLevel = newFurSubLevel;
    }
    [PunRPC]
    public void MeleeRPC(Vector3 pos, Quaternion q,Photon.Realtime.Player attacker)
    {
        Debug.Log(attacker+" melee");
        Collider2D[] hits;

        bool oppositeDirection = rb.transform.localScale.x < 0;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(_throwPoint.position,_radiusMelee);
    }
} 
