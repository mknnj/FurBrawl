using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milk : MonoBehaviour
{
    private GameObject balcony;
    public bool isGolden = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Cat>().CanMove()) 
        {
            Destroy(gameObject);
            if (isGolden)
                other.gameObject.GetComponent<Cat>().DrinkGoldenMilk(balcony);
            else
                other.gameObject.GetComponent<Cat>().Drink(balcony);
            
            
            //Debug.Log("Player " + other.name + " drank a bottle of milk");
        }
    }

    public void setBalcony(GameObject balcony, bool isGolden)
    {
        this.balcony = balcony;
        this.isGolden = isGolden;
    }
}
