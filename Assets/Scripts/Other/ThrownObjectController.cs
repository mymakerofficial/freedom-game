using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ThrownObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            transform.localScale.x + 0.01f, 
            transform.localScale.y + 0.01f, 
            transform.localScale.z + 0.01f
        );

        if (transform.localScale.x >= 1)
        {
            Destroy(GetComponent<ThrownObjectController>());
        }
    }
}
