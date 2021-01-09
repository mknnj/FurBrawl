using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    [SerializeField] private float fallMuliplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y < -0.1)
        {
            Debug.Log(rb.velocity.y + "fall");
            rb.gravityScale = fallMuliplier;
        }
        else if (rb.velocity.y > 0.1 && !Input.GetButton("Jump"))
        {
            Debug.Log(rb.velocity.y + "lowjump");
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
}
