using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    private Controls _controls;
    
    void Awake()
    {
        _controls = new Controls();

        _controls.General.Escape.performed += _ => StopCursorLock();
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }
    
    void Start()
    {
        StartCursorLock();
    }
    
    private void StartCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void StopCursorLock()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
