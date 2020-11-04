using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(UserInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cat : MonoBehaviourPun
{
    public PhotonView pv;
    [SerializeField] private float _speed = 5;
    [Range(1,40)][SerializeField] private int _furLevel=4; //will act like a weight, too (probably?)
    [SerializeField] [Range(1, 9)] private int _hearts;
    [SerializeField] private bool _canBeHit = true;
    [SerializeField] private float invincibility;
    [SerializeField] private float _jumpIntensity = 5f;

    [SerializeField] private Transform _throwPoint;
    [SerializeField] private float _throwWaitTime = 0.2f;
    [SerializeField] private bool _isAttacking=false;
    [SerializeField] private GameObject _furBallPrefab;
    [SerializeField] private float _pushImpact=5f;
    [SerializeField] private bool _hitted=false;
    [SerializeField] private float _hittedWaitTime = 0.2f;

    
    
    private BoxCollider2D _boxCollider;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;
    public FeetCollider _feetCollider;
    private SpriteRenderer _SR;
    
    private void Awake()
    {



        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _feetCollider = GetComponentInChildren<FeetCollider>();
        _SR = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!_isAttacking && userInput.throwInput && _furLevel>1)
            {
                _isAttacking = true;
                photonView.RPC("ThrowRPC", RpcTarget.AllViaServer, _throwPoint.position,_throwPoint.rotation);
                StartCoroutine(ThrowWait());
            }
            if (!_hitted)
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
        if (photonView.IsMine && !_hitted)
        {
            Move(); // if we're the player process user input and update location
        }
    }



    private void Jump() //simple
    {
        if (_feetCollider.IsOnGround && userInput.jumpInput)
        {
            rb.AddForce(Vector3.up * (_jumpIntensity - _furLevel), ForceMode2D.Impulse);
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
            rb.transform.localScale = new Vector2(-Math.Abs(rb.transform.localScale.x), transform.localScale.y);
            //_SR.flipX = true;

        } else if (rb.velocity.x > 0)
        {
            rb.transform.localScale = new Vector2(Math.Abs(rb.transform.localScale.x), transform.localScale.y);
            //_SR.flipX = false;
        }
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
            transform.position = respawnPoint;
            StartCoroutine(InvincibilityFrame(invincibility));
        }
    }
    
    private IEnumerator InvincibilityFrame(float time)
    {
        
        yield return new WaitForSeconds(time);
        
        _canBeHit = true;
        yield return null;
    }

    private IEnumerator ThrowWait()
    {
        _furLevel--;
        yield return new WaitForSeconds(_throwWaitTime);
        _isAttacking = false;
        
    }

    private IEnumerator HitKnockoutTime()
    {
        yield return  new WaitForSeconds(_hittedWaitTime);
        _hitted = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("trigger with "+other.name);
        if (other.CompareTag("FurBall") && _canBeHit)
        {
            _hitted = true;

            //YASEEN: Play sound
            FindObjectOfType<AudioManager>().Play("scream");

            Debug.Log(photonView.Owner+" hitted by a furball");
            rb.AddForce(other.GetComponent<FurBall>().direction * _pushImpact, ForceMode2D.Impulse);
            StartCoroutine(HitKnockoutTime());
        }
    }

    [PunRPC]
    public void ThrowRPC(Vector3 pos, Quaternion q,PhotonMessageInfo info)
    {
        Debug.Log("Furball RPC");
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        GameObject fb=Instantiate(_furBallPrefab, pos, q);


        //YASEEN: 'oppositeDirection' Passes the info of the direction to the lil cute Furball <3 
        bool oppositeDirection =false;
        if (rb.transform.localScale.x < 0)
        {
            oppositeDirection = true;
        }

        fb.GetComponent<FurBall>().SetData(photonView.Owner,Mathf.Abs(lag),oppositeDirection);
        
    }
} 
