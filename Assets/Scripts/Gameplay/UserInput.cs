using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [Range(0, 2)] [SerializeField] private int _mousebutton = 0;
    public Vector3 movementInput { get; private set; }
    
    public bool jumpInput { get; private set; }
    
    public bool throwInput { get; private set; }
    public bool meleeInput { get; private set; }
    

    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyBindings.Jump);
        throwInput = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyBindings.FurBall);
        meleeInput = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyBindings.Melee);
    }
}
