using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Transform))]
public class LineToGameObject : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        GetComponent<SpringJoint>().spring = 2f;
        GetComponent<SpringJoint>().damper = 0.1f;
        GetComponent<SpringJoint>().tolerance = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        // set line renderer points
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, target.transform.position);
    }
}
