using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 direction;
    private Rigidbody body;

    public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.right;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.AddForce(direction * speed);
    }

    void OnCollisionEnter(Collision collision) {
        direction = -direction;
        Debug.Log("AHHHH");
    }
}
