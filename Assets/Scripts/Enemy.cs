using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10.0f;

    private Vector3 direction;
    private Rigidbody body;

    private bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.right;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.transform.position += direction * speed;
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != "floor") {
            direction = -direction;
        }

        if(collision.gameObject.tag == "Player") {
            Application.LoadLevel(Application.loadedLevel);

            Debug.Log("Player!");
        }
    }
}
