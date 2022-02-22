using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class HumanoidMovementController : MonoBehaviour
{
    private Controls _controls;
    private Rigidbody _rigidbody;

    private float _distToGround;

    private Vector3 _velocity;
    
    [Space]
    public float playerSpeed = 2.0f;
    public float jumpForce = 1.0f;
    [Space] 
    public GameObject camera;
    
    void Awake()
    {
        _controls = new Controls();
        _rigidbody = GetComponent<Rigidbody>();

        _distToGround = GetComponent<Collider>().bounds.extents.y;

        _controls.Humanoid.Jump.performed += _ => Jump();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }

    private bool IsGrounded
    {
        get
        {
            return !Physics.Raycast(transform.position, Vector3.down, _distToGround * 2 + 0.3f);
        }
    }

    private void Jump()
    {
        if(!IsGrounded) return;
        
        // add jump force
        _rigidbody.AddForce(0, Mathf.Sqrt(jumpForce * -3.0f * Physics.gravity.y * _rigidbody.mass), 0, ForceMode.Impulse);
    }
    
    void FixedUpdate()
    {
        _velocity.x = _controls.Humanoid.Move.ReadValue<Vector2>().x;
        _velocity.z = _controls.Humanoid.Move.ReadValue<Vector2>().y;
        
        // create object with camera transform without x rotation
        GameObject turnReferenceObject = new GameObject();
        turnReferenceObject.transform.parent = camera.transform.parent;
        turnReferenceObject.transform.position = camera.transform.position;
        turnReferenceObject.transform.rotation = Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.z);
        turnReferenceObject.transform.localScale = camera.transform.localScale;

        // calculate relative movement vector
        Vector3 move = _velocity.normalized * playerSpeed * _velocity.magnitude * Time.fixedDeltaTime;
        move = turnReferenceObject.transform.worldToLocalMatrix.inverse * move;
        
        // destroy teporary game object
        Destroy(turnReferenceObject);

        // calculate rotation
        Vector3 rotate = new Vector3(0,
            transform.rotation.eulerAngles.y + _controls.Humanoid.Turn.ReadValue<Vector2>().x, 0);

        // move and rotate rigidbody
        _rigidbody.MovePosition(transform.position + move);
        _rigidbody.MoveRotation(Quaternion.Euler(rotate));
        
        /*
        camera.transform.rotation = Quaternion.Euler(new Vector3(            
            Mathf.Clamp(camera.transform.rotation.eulerAngles.x - _controls.Humanoid.Turn.ReadValue<Vector2>().y, -90, 90),
            camera.transform.rotation.eulerAngles.y,
            camera.transform.rotation.eulerAngles.z
        ));
        */
    }
}
