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
    
    private Vector3 _lastPosition;
    private Vector3 _lastRotation;

    [Header("Config")] 
    public float forceMultiplier = 1;
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
        _obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    
    private void Cancel()
    {
        Destroy(_obj);
    }
    
    private void Release()
    {
        _obj.GetComponent<Rigidbody>().AddTorque(_currentForce * forceMultiplier, ForceMode.VelocityChange);
        _obj.GetComponent<Rigidbody>().AddTorque(_currentTorque, ForceMode.VelocityChange);
        _obj.AddComponent<ThrownObjectController>();
        _obj = null;
    }
    
    void FixedUpdate()
    {
        if (_obj != null)
        {
            _obj.transform.position = transform.position;
            _obj.transform.rotation = transform.rotation;
            
            Vector3 positionDelta = ((transform.position - player.transform.position) - _lastPosition);
            _lastPosition = transform.position - player.transform.position;
            
            Vector3 rotationDelta = ((transform.rotation.eulerAngles) - _lastRotation);
            _lastRotation = transform.rotation.eulerAngles;
    
            _currentForce = positionDelta * Time.fixedDeltaTime;
            _currentTorque = rotationDelta * Time.fixedDeltaTime;
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.HSVToRGB(0.8f, 0.8f, 1f);
        Gizmos.DrawSphere(transform.position, 0.03f);
    }
}
