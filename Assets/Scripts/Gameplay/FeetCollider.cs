using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollider : MonoBehaviour
{
    [SerializeField]
    private bool _isOnGround = false;

    public bool IsOnGround => _isOnGround;
    public Animator catAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.CompareTag("Balcony") || other.CompareTag("Platform"))
        {
            _isOnGround = true;
            
        }

        if (other.CompareTag("PlayerHead") ) //we need a separate check if the player on which we are standing is mid-air
        {
            if (other.GetComponentInParent<Cat>()._feetCollider._isOnGround)
            {
                _isOnGround = true;
            }
            other.GetComponentInParent<Cat>().FallOnHead(GetComponentInParent<Cat>().GetFurLevel());
            
        }


    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ( other.CompareTag("Balcony") || other.CompareTag("Platform"))
        {
            _isOnGround = true;
        }

        if (other.CompareTag("PlayerHead")) //we need a separate check if the player on which we are standing is mid-air
        {
            _isOnGround = other.GetComponentInParent<Cat>()._feetCollider._isOnGround;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHead") || other.CompareTag("Balcony") || other.CompareTag("Platform")) // if for any reason the cat's feet don't collide anymore, it is considered mid-air
        {
            _isOnGround = false;
        }
    }
}
