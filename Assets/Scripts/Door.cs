using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("clicked on door");
    }

    // Update is called once per frame
    void OneMouseDown()
    {
        rb.AddForce(-transform.forward * 500f);
        rb.useGravity = true;
        Debug.Log("clicked on door");
    }
}
