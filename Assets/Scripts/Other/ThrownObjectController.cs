using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class ThrownObjectController : MonoBehaviour
{
    private Vector3 _startPosition;

    public float scaleDistance = 20;
    public AnimationCurve scaleCurve;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(_startPosition, transform.position);
        float scaledDistance = distance / scaleDistance;
        
        float scale = scaleCurve.Evaluate(scaledDistance);
        
        transform.localScale = new Vector3(
            scale, 
            scale, 
            scale
        );

    }
}
