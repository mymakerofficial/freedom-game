using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGunController : MonoBehaviour
{
    private Controls _controls;

    private LineRenderer _lineRenderer;
    
    private Vector3 _grapplePoint;
    private SpringJoint _joint;
    private float _distance;
    
    public InputActionReference inputAction;
    public Transform player;

    void Awake()
    {
        _controls = new Controls();

        _lineRenderer = GetComponent<LineRenderer>();

        _controls.FindAction(inputAction.name).performed += GrappleInput;
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }
    
    private void GrappleInput(InputAction.CallbackContext e)
    {
        if (Math.Round(e.ReadValue<float>()) == 1)
        {
            StartGrapple();
        }
        else
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        if(_joint) Destroy(_joint);
        
        _lineRenderer.enabled = true;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            _grapplePoint = hit.point;

            _distance = Vector3.Distance(transform.position, _grapplePoint);

            // create new joint
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _grapplePoint;

            _joint.maxDistance = _distance;
            _joint.minDistance = 0;

            _joint.spring = 4.5f;
            _joint.damper = 7f;
            _joint.massScale = 4.5f;
        }
    }

    private void StopGrapple()
    {
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
        Destroy(_joint);
    }
    
    void Update()
    {
        if (_joint)
        {
            // shorten joint if needed
            float dist = Vector3.Distance(transform.position, _grapplePoint);

            if (dist < _distance)
            {
                _distance = dist;
                
                _joint.maxDistance = _distance;
            }

            // set anchor
            _joint.anchor = player.position - transform.position;
        
            // set line renderer points
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _grapplePoint);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, 0.02f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
