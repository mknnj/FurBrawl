using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollider : MonoBehaviour
{
    [SerializeField]
    private bool _isOnGround = false;

    public bool IsOnGround => _isOnGround;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Feet collided with: "+other.tag);
        if (other.CompareTag("PlayerHead") || other.CompareTag("Balcony") || other.CompareTag("Platform"))
        {
            _isOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHead") || other.CompareTag("Balcony"))
        {
            _isOnGround = false;
        }
    }
}
