using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Cat>().CanMove()) 
        {
            Destroy(gameObject);
            other.gameObject.GetComponent<Cat>().Stun();
            Debug.Log("Player " + other.name + " got hit by a falling jar");
        } else if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }
    
}
