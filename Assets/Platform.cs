using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    private SpriteRenderer _SR;
    private bool _damage; //used to control starting and stopping the damage
    private float _damageFactor;
    //List<GameObject> currentCollisions = new List<GameObject>();
    private void Awake()
    {
        _damage = false;
        _SR = GetComponent<SpriteRenderer>();
        _damageFactor = 0;

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "FeetCollider")
        {
            //currentCollisions.Add(other.gameObject);
            _damageFactor += 1;
            _damage = true;
            Debug.Log("EMTERING");
            StartCoroutine(Damage(0));
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.name == "FeetCollider")
        {
            _damageFactor -= 1;
            if (_damageFactor <= 0)
                _damage = false;
            Debug.Log("EXITING");
        }

    }


    private IEnumerator Damage(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("DAMAGE DONE");

        WaitForSeconds wait = new WaitForSeconds(0.5f);
        float initialHealth = _SR.color.g*255;
        Debug.Log("intial helth" + initialHealth);
        foreach (int value in System.Linq.Enumerable.Range(1, (int)initialHealth))
        {
            Debug.Log("crnt helth" + initialHealth);
            initialHealth -= 10 * Mathf.Pow(3, _damageFactor);
            _SR.color = new Color((255 - initialHealth)/255, Mathf.Max(initialHealth, 0)/255, 0);
            if(initialHealth <= 0)
                Destroy(gameObject);
            if(!_damage)
                yield break;
            yield return wait;
        }
        //   Destroy(gameObject);
    }
}
