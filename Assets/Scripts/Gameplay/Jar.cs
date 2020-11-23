using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<Cat>().CanMove()) 
        {
            rb.bodyType = RigidbodyType2D.Static;
            animator.SetTrigger("broken");
            StartCoroutine(WaitThenDestroy(gameObject));
            other.gameObject.GetComponent<Cat>().Stun();
            Debug.Log("Player " + other.GetInstanceID() + " got hit by a falling jar");
        } else if (other.CompareTag("Border"))
        {
            StartCoroutine(WaitThenDestroy(gameObject));
            
        }
    }
    private IEnumerator WaitThenDestroy(GameObject g)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(g);
        yield return null;
    }
}
