using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private bool killsOnTouch = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (killsOnTouch && other.CompareTag("Player"))
            other.gameObject.GetComponent<Cat>().Die("Fall");
    }
}
