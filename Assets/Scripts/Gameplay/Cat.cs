using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Runtime.InteropServices.ComTypes;

[RequireComponent(typeof(UserInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cat : MonoBehaviourPun
{
    public PhotonView pv;
    [SerializeField] private float _speed = 5;
    [Range(1,4)][SerializeField] private int _furLevel=4; //will act like a weight, too (probably?)
    [SerializeField] [Range(1, 9)] private int _hearts;
    [SerializeField] private bool _canBeHit = true;
    [SerializeField] private float invincibility;
    [SerializeField] private float _jumpIntensity = 5f;

    [SerializeField] private Transform _throwPoint;
    [SerializeField] private float _throwWaitTime = 0.2f;
    [SerializeField] private bool _isAttacking=false;
    [SerializeField]private GameObject _furBallPrefab;
    [SerializeField] private float _pushImpact=5f;
    [SerializeField] private bool _hitted=false;
    [SerializeField] private float _hittedWaitTime = 0.2f;
    
    private BoxCollider2D _boxCollider;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;
    public FeetCollider _feetCollider;

    
    

    private void Awake()
    {
        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _feetCollider = GetComponentInChildren<FeetCollider>();
        


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
        }
        /*else
        {
            smoothMovement(); // for other players, then get their new locations and smooth the movements 
        }*/
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (!_hitted)
            {
                Move(); // if we're the player process user input and update location
                Jump(); // NOT SYNCED YET
            }
        }
    }

    /*private void smoothMovement()
    {
        //gets the new position of OTHER cats (according to when is this function called), and smoothes the transition between their past position and new one
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);

    }*/

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
    }

    
    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //manages read/write streams to the server
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); // sends our current position to everbody else
        } else if (stream.IsReading)
        {
            smoothMove = (Vector3) stream.ReceiveNext();
        }
    }*/

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
        if (other.CompareTag("FurBall"))
        {
            _hitted = true;
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
        fb.GetComponent<FurBall>().SetData(photonView.Owner,Mathf.Abs(lag));
        
    }
} 
