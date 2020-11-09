using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milk : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Cat>().CanMove()) 
        {
            Destroy(gameObject);
            other.gameObject.GetComponent<Cat>().Drink();
            //Debug.Log("Player " + other.name + " drank a bottle of milk");
        }
    }
}
