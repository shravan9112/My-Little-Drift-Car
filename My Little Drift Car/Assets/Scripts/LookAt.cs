using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    private Rigidbody RB;
    public Vector3 offset;
    public float speed;

    private void Start()
    {
        RB = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 playerForward = (RB.velocity + target.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, target.position + target.TransformVector(offset) + playerForward * (-5f), speed * Time.deltaTime);
        transform.LookAt(target);
    }
}
