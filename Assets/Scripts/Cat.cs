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
    [SerializeField] private int _hearts;
    [SerializeField] private bool _canBeHit;
    private Vector3 smoothMove;
    private UserInput userInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        userInput = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody2D>();
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
} 
