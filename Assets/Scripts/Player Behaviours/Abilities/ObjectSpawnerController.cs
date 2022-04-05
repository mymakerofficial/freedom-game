using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSpawnerController : MonoBehaviour
{
    private GameObject _obj;

    private Vector3 _currentForce;
    private Vector3 _currentTorque;
    
    private Vector3 _lastLocalPosition;
    private Vector3 _lastPlayerPosition;
    private Vector3 _lastRotation;

    [Header("Config")] 
    public float forceMultiplier = 1;
    public float handForceMultiplier = 1;
    public AnimationCurve objectScaleCurve;
    public GameObject player;
    [Header("Object")]
    public GameObject spawnObject;
    [Header("Input")]
    public InputActionReference inputAction;

    

    // Start is called before the first frame update
    void Start()
    {
        inputAction.action.performed += Input;
    }
    
    private void OnEnable()
    {
        inputAction.action.Enable();
    }

    private void OnDestroy()
    {
        inputAction.action.Disable();
    }

    private void Input(InputAction.CallbackContext e)
    {
        if (Math.Round(e.ReadValue<float>()) == 1)
        {
            Create();
        }
        else
        {
            Release();
        }
    }

    private void Create()
    {
        if (_obj != null) return;
        _obj = Instantiate(spawnObject, transform.position, transform.rotation);
        _obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    
    private void Cancel()
    {
        Destroy(_obj);
    }
    
    private void Release()
    {
        // apply forces to object
        _obj.GetComponent<Rigidbody>().AddForce(_currentForce, ForceMode.VelocityChange);
        _obj.GetComponent<Rigidbody>().AddTorque(_currentTorque, ForceMode.VelocityChange);
        _obj.AddComponent<ThrownObjectController>();
        _obj.GetComponent<ThrownObjectController>().scaleCurve = objectScaleCurve;
        _obj = null;
    }
    
    void FixedUpdate()
    {
        if (_obj != null)
        {
            // fix object transform
            _obj.transform.position = transform.position;
            _obj.transform.rotation = transform.rotation;
            
            // get hand velocity
            Vector3 localPositionDelta = ((transform.position - player.transform.position) - _lastLocalPosition);
            _lastLocalPosition = transform.position - player.transform.position;
            
            // get player velocity
            Vector3 playerPositionDelta = ((player.transform.position) - _lastPlayerPosition);
            _lastPlayerPosition = player.transform.position;
            
            // get hand torque
            Vector3 rotationDelta = ((transform.rotation.eulerAngles) - _lastRotation);
            _lastRotation = transform.rotation.eulerAngles;
    
            // calculate velocities
            Vector3 newForce = (localPositionDelta * handForceMultiplier + playerPositionDelta) * forceMultiplier;
            _currentForce += (newForce - _currentForce) / 6;
            _currentTorque = rotationDelta;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.HSVToRGB(0.8f, 0.8f, 1f);
        Gizmos.DrawSphere(transform.position, 0.03f);
        Gizmos.DrawRay(transform.position, _currentForce);
    }
}
