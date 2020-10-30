using System;
using System.Collections;
using System.Collections.Generic;
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
    public Vector3 direction;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        direction = _transform.right;
        Debug.Log("Furball Launched");
        
    }

    private void FixedUpdate()
    {
        _transform.position = _transform.position + _speed * Time.fixedDeltaTime * direction;
        /*if (_transform.position.x > _rightLimit || _transform.position.x <_leftLimit)
        {
            Destroy(gameObject);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name+ " furball collided");
        Destroy(gameObject);
    }

    public void SetData(Player owner, float lag)
    {
        Launcher = owner;
        _lag = lag;
    }
}
