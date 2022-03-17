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
    private float _cameraPitch;
    
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

        Matrix4x4 cameraLocalMatrix = Matrix4x4.TRS(
            camera.transform.position,
            Quaternion.Euler(
                0,
                camera.transform.rotation.eulerAngles.y,
                camera.transform.rotation.eulerAngles.z
            ),
            camera.transform.localScale
        );

        // calculate relative movement vector
        Vector3 move = _velocity.normalized * playerSpeed * _velocity.magnitude * Time.fixedDeltaTime;
        move = cameraLocalMatrix * move;

        // calculate rotation
        Vector3 rotate = new Vector3(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + _controls.Humanoid.Turn.ReadValue<Vector2>().x, 
            transform.rotation.eulerAngles.z
        );

        // move rigidbody
        //_rigidbody.MovePosition(transform.position + move);
        
        _rigidbody.AddForce(move * 100, ForceMode.Acceleration);
        
        // look yaw
        _rigidbody.MoveRotation(Quaternion.Euler(rotate));
        
        // camera pitch
        _cameraPitch = Mathf.Clamp(_cameraPitch - _controls.Humanoid.Turn.ReadValue<Vector2>().y, -90, 90);
        camera.transform.rotation = Quaternion.Euler(_cameraPitch, _rigidbody.transform.rotation.eulerAngles.y, 0);
    }
}
