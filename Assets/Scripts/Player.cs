using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float dashForce = 1.0f;
    public float jumpForce = 20.0f;
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

        jump();

        if(Input.GetButtonDown("Dash")) {
            body.AddForce(movement * dashForce);
        }
    }

    void jump(){
        Vector3 jumpVector = Vector3.zero;

        if (Input.GetButtonDown("Jump")){
            jumpVector.y = 1;
            body.AddForce(transform.up * jumpForce);
        }
    }
}
