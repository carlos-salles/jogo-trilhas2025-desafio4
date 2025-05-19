using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Snake : MonoBehaviour
{
    [SerializeField]
    float viewDistance;
    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position + Vector3.left * 1f;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.left * viewDistance);
        Gizmos.DrawRay(pos, Vector3.down * 1f);
    }
}
