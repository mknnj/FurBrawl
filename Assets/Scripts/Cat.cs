using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Runtime.InteropServices.ComTypes;

public class Cat : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    [SerializeField] private float _speed = 5;
    [SerializeField] private int _furLevel; //will act like a weight, too (probably?)
    [SerializeField] private int _hearts;
    [SerializeField] private bool _canBeHit;
    private Vector3 smoothMove;

    private void Update()
    {
        if (photonView.IsMine)
        {
            ProcesInput(); // if we're the player process user input and update location
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

    private void ProcesInput() //simple
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0);
        transform.position += move * _speed * Time.deltaTime;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //manages read/write streams to the server
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); // sends our current position to everbody else
        } else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }
} 
