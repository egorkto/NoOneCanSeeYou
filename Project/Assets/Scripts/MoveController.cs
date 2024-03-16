using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
public class MoveController : MonoBehaviour
{
    private NewControls playerActionsAsset;
    private InputAction move;
    private InputAction run;
    //movement fields
    private Rigidbody rb;
    [SerializeField]
    public float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    public float maxSpeed = 5f;
    public Vector3 forceDirection = Vector3.zero;
    public float animValue;
    [SerializeField] private float turnSpeed;
    [SerializeField]
    private Camera playerCamera;
    
    
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new NewControls();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        move = playerActionsAsset.Player.Move;
        run = playerActionsAsset.Player.Run;
        playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (run.IsPressed() && move.IsPressed())
        {
            movementForce = 3;
            animValue = 1;
        }
        else if(move.IsPressed())
        {
            movementForce = 1;
            animValue = 0.5f;
        }
        else
        {
            animValue = 0;
        }
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;
        
        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, this.rb.rotation, turnSpeed);

        }


        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }


}
