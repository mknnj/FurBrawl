using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollider : MonoBehaviour
{
    private bool _isOnGround = false;

    public bool IsOnGround => _isOnGround;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Balcony"))
        {
            _isOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isOnGround = false;
    }
}
