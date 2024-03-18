using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using DG.Tweening;
public class MoveController : MonoBehaviour
{
    private NewControls playerActionsAsset;
    private InputAction move;
    private InputAction rotateE;
    private InputAction rotateQ;
    private Rigidbody rb;
    [SerializeField] public float movementForce = 1f;
    [SerializeField] public float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;
    [HideInInspector] public float animValue;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject cameraRoot;
    [SerializeField] private int matrixRotation;
    [SerializeField] private int moveRotation;
    bool isRotating;
    private void Awake()
    {
        
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new NewControls();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        move = playerActionsAsset.Player.Move;
        playerActionsAsset.Player.RotateRight.started += RotateRightCamera;
        playerActionsAsset.Player.RotateLeft.started += RotateLeftCamera;
        rotateQ = playerActionsAsset.Player.RotateLeft;
        playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Player.RotateLeft.started -= RotateLeftCamera;
        playerActionsAsset.Player.RotateRight.started -= RotateRightCamera;
        playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        GetInput();
        LookAt();
        Move();
        
    }
    private void GetInput()
    {
        forceDirection.z += -move.ReadValue<Vector2>().x  * movementForce;
        forceDirection.x += move.ReadValue<Vector2>().y  * movementForce;
    }
    private void Move()
    {
        if (move.IsPressed())
        {
            movementForce = 1;
            animValue = 1f;
        }
        else
        {
            animValue = 0;
        }
        cameraRoot.transform.position = transform.position;
        
        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

       

    }
    #region Rotate Camera
    private void RotateLeftCamera(InputAction.CallbackContext obj)
    {
        if (!isRotating)
        {
            moveRotation += 90;
            isRotating = true;
            float currentRotation = cameraRoot.transform.rotation.eulerAngles.y;
            float newRotation = currentRotation + 90;
            matrixRotation += 90;
            cameraRoot.transform.DORotate(new Vector3(0, newRotation, 0), 0.2f, RotateMode.Fast).OnComplete(() => isRotating = false);
        }
    }
    private void RotateRightCamera(InputAction.CallbackContext obj)
    {
        if (!isRotating)
        {
            moveRotation -= 90;
            isRotating = true;
            float currentRotation = cameraRoot.transform.rotation.eulerAngles.y;
            float newRotation = currentRotation - 90;
            matrixRotation -= 90;
            cameraRoot.transform.DORotate(new Vector3(0, newRotation, 0), 0.2f, RotateMode.Fast).OnComplete(() => isRotating = false);
        }            
    }
    #endregion
    private void LookAt()
    {
        if(forceDirection != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, matrixRotation, 0));
            var skewedInput = matrix.MultiplyPoint3x4(forceDirection);
            var relative = (transform.position + skewedInput) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot,400f * Time.deltaTime);
            var movementDirection = forceDirection.normalized;

            // ѕолучаем текущее направление взгл€да камеры
            var cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0f; // ”бираем высотную компоненту, чтобы движение происходило по плоскости

            // ѕреобразуем направление движени€ относительно направлени€ взгл€да камеры
            var rotatedMovementDirection = Quaternion.LookRotation(cameraForward) * Quaternion.Euler(0, matrixRotation-moveRotation, 0) * movementDirection;

            // ”станавливаем преобразованное направление движени€
            forceDirection = rotatedMovementDirection * movementForce;
        }
        //Vector3 direction = rb.velocity;
        //direction.y = 0f;
        //if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        //{
        //    rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rb.rotation, turnSpeed);

        //}
        //else
        //    rb.angularVelocity = Vector3.zero;
    }
}
