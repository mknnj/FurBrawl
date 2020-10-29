using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Runtime.InteropServices.ComTypes;

[RequireComponent(typeof(UserInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cat : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    [SerializeField] private float _speed = 5;
    [SerializeField] private int _furLevel; //will act like a weight, too (probably?)
    [SerializeField] [Range(1, 9)] private int _hearts;
    [SerializeField] private bool _canBeHit;
    [SerializeField] [Range (0, 1)] private float extraHeight = 0.01f;
    private BoxCollider2D _boxCollider;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Move(); // if we're the player process user input and update location
            Jump(); // NOT SYNCED YET
        }
        else
        {
            smoothMovement(); // for other players, then get their new locations and smooth the movements 
        }
    }

    private void smoothMovement()
    {
        //gets the new position of OTHER cats (according to when is this function called), and smoothes the transition between their past position and new one
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);

    }

    private void Jump() //simple
    {
        if (userInput.jumpInput)
        {
            rb.AddForce(Vector3.up * (5 - _furLevel), ForceMode2D.Impulse);
        }

    }

    //Check if the player is grounded, otherwise he won't be able to jump
    private void Move() //simple
    {
        var move = userInput.movementInput;
        transform.position += move * _speed * Time.deltaTime;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //manages read/write streams to the server
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); // sends our current position to everbody else
        } else if (stream.IsReading)
        {
            smoothMove = (Vector3) stream.ReceiveNext();
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
                    break;
                default:
                    break;
            }
            transform.position = respawnPoint;
        }
    }
} 
