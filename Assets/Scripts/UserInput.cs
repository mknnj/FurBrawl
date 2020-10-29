using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public Vector3 movementInput { get; private set; }
    public bool jumpInput { get; private set; }

    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }
}
