using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class Thruster
{
    [Space]
    public string name;
    [Header("Input")] 
    public InputActionReference inputAction;
    [Tooltip("Add to rotation in the direction of movement")] public float addedMovementDegrees;
    [Header("Transform")] 
    public Transform transform;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
    [Header("Force")] 
    [UnityEngine.Min(0.1f)][Max(10f)]public float forceFactor;

    public Vector3 relativePosition
    {
        get
        {
            Vector3 offset = this.transform.worldToLocalMatrix.inverse * this.offsetPosition;
            Vector3 position = this.transform.position + offset;
            return position;
        }
    }

    public Vector3 relativeDirection
    {
        get
        {
            Vector3 dir = Quaternion.Euler(this.offsetRotation) * Vector3.down;
            dir = this.transform.worldToLocalMatrix.inverse * dir;
            return dir.normalized;
        }
    }

    public Vector3 relativeDirectionInjected(Vector3 inject)
    {
        return relativeDirectionInjected(inject, null);
    }

    /// <summary>
    /// Adds movenemt direction relative to camera to rotation
    /// </summary>
    /// <param name="inject">Movement directional vector</param>
    /// <param name="localTransform">camera transform</param>
    public Vector3 relativeDirectionInjected(Vector3 inject, [CanBeNull] Transform localTransform)
    {
        if (localTransform)
        {
            Matrix4x4 cameraLocalMatrix = Matrix4x4.TRS(
                localTransform.position,
                Quaternion.Euler(
                    0,
                    localTransform.rotation.eulerAngles.y,
                    localTransform.rotation.eulerAngles.z
                ),
                localTransform.localScale
            );

            inject = (cameraLocalMatrix * (Quaternion.Euler(inject) * Vector3.forward));
        }

        Vector3 dir = Quaternion.Euler(this.offsetRotation) * Vector3.down;
        dir = Quaternion.Euler(inject) * dir;
        dir = this.transform.worldToLocalMatrix.inverse * dir;
        return dir.normalized;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class ThrusterController : MonoBehaviour
{
    private Controls _controls;
    private Rigidbody _rigidbody;

    [Space]
    [UnityEngine.Min(0)] public float thrusterForce;
    [Space]
    public List<Thruster> thrusters;
    [Space] 
    public GameObject camera;

    void Awake()
    {
        _controls = new Controls();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }
    
    void FixedUpdate()
    {
        foreach (var thruster in thrusters)
        {
            // get input value
            float input = _controls.FindAction(thruster.inputAction.name).ReadValue<float>();
            
            // calculate force vector
            Vector3 force = thruster.relativeDirectionInjected(new Vector3(
                _controls.Humanoid.Move.ReadValue<Vector2>().y, 
                0, 
                -_controls.Humanoid.Move.ReadValue<Vector2>().x
            ) * thruster.addedMovementDegrees, camera.transform);
            force = force * (thrusterForce * thruster.forceFactor) * Physics.gravity.y * _rigidbody.mass * input * Time.fixedDeltaTime;

            // apply force
            _rigidbody.AddForceAtPosition(force, thruster.relativePosition, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < thrusters.Count; i++)
        {
            var thruster = thrusters[i];

            Gizmos.color = Color.HSVToRGB((1 / (float)(thrusters.Count + 1)) * i, 0.8f, 1f);
            Gizmos.DrawSphere(thruster.relativePosition, 0.05f);
            if (_controls != null)
            {
                Gizmos.DrawRay(thruster.relativePosition, thruster.relativeDirectionInjected(new Vector3(_controls.Humanoid.Move.ReadValue<Vector2>().y, 0, -_controls.Humanoid.Move.ReadValue<Vector2>().x) * thruster.addedMovementDegrees));
            }
            else
            {
                Gizmos.DrawRay(thruster.relativePosition, thruster.relativeDirection);
            }
        }
    }
}
