using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float dashForce = 1.0f;
    public float jumpForce = 1.0f;
    public float speed = 1.0f;

    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 movement = Vector3.zero;
        movement.x = Input.GetAxis("Horizontal");

        body.AddForce(movement * speed);

        if(Input.GetButtonDown("Dash")) {
            body.AddForce(movement * dashForce);
        }
    }
}
