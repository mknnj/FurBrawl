using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class FurBall : MonoBehaviour
{
    [Tooltip("Moving speed of the furball")]
    [SerializeField] private float _speed = 5;

    [SerializeField] private float _lag;
    
    [SerializeField] private float _rightLimit;
    [SerializeField] private float _leftLimit;
    public Player Launcher { get; private set; }
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    public Vector3 direction;

   // public bool hit = false;
    

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        direction = _transform.right;
        Debug.Log("Furball Launched");
        
    }

    private void FixedUpdate()
    {

        //YASEEN: Revese speed if player facing other direction


        //if(!hit)
           // _transform.position = _transform.position + _speed * Time.fixedDeltaTime * direction;
        if (_transform.position.x > _rightLimit || _transform.position.x <_leftLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name+ " furball collided");
        //hit = true;
        if (other.CompareTag("Player") && !Launcher.Equals(other.GetComponent<PhotonView>().Owner))
        {
            Destroy(gameObject);
        }
        
    }

    public void SetData(Player owner, float lag, bool oppositeDirection)
    {
        Launcher = owner;
        _lag = lag;

        //YASEEN: if oppositeDirection, then the direction is flipped, too :P
        if (oppositeDirection)
        {
            direction *= -1;
        }
        
       _rigidbody.velocity = direction * _speed;
       _rigidbody.position += _rigidbody.velocity * lag;
    }
}
