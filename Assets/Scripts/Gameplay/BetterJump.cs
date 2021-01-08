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
        if (rb.velocity.y < -Mathf.Epsilon)
        {
            rb.gravityScale = fallMuliplier;
        }
        else if (rb.velocity.y > Mathf.Epsilon && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
}
