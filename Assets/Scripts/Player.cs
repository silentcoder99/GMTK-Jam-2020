using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float dashForce = 1f;
    public float jumpForce = 500f;
    public float speed = 5f;

    public int jumpCount = 1;
    public int dashCount = 0;
    public int attackCount = 0;

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

        if (Input.GetButtonDown("Dash") && (dashCount > 0)){
            body.AddForce(movement * dashForce);
        }
    }

    void jump(){
        Vector3 jumpVector = Vector3.zero;

        if (Input.GetButtonDown("Jump") && (jumpCount > 0)){
            jumpVector.y = 1;
            body.AddForce(transform.up * jumpForce);
            jumpCount --;
        }
    }

    private void OnTriggerEnter(Collider other){
        GameObject powerUp = other.gameObject;

        string pickupName = other.gameObject.name;

        switch(pickupName){
            case "JumpPickup":
                jumpCount ++;
                break;
            case "DashPickup":
                dashCount ++;
                break;
        }

        Destroy(powerUp);
    }
}
