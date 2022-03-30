using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class ThrusterController : MonoBehaviour
{
    private Controls _controls;
    private Rigidbody _playerRigidboy;

    [Header("Thruster")]
    [UnityEngine.Min(0)] public float thrusterForce;
    [Header("Input")] 
    public InputActionReference inputAction;
    [Space] 
    public GameObject player;

    void Start()
    {
        _playerRigidboy = player.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputAction.action.Enable();
    }
    
    private void OnDisable()
    {
        inputAction.action.Disable();
    }

    private float inputValue => inputAction.action.ReadValue<float>();

    private Vector3 force => -transform.up * 
        (thrusterForce * Physics.gravity.y * (_playerRigidboy != null ? _playerRigidboy.mass : 0) * inputValue);

    void FixedUpdate()
    {
        // apply force
        _playerRigidboy.AddForceAtPosition(force  * Time.fixedDeltaTime, transform.position, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.HSVToRGB(0f, 0.8f, 1f);
        Gizmos.DrawSphere(transform.position, 0.03f);
        Gizmos.DrawRay(transform.position, -transform.up);
        Gizmos.DrawSphere(transform.position - this.force.normalized * inputValue, 0.02f);
    }
}
