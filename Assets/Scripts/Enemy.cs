using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10.0f;
    public int hp = 1;

    private Vector3 direction;
    private Rigidbody body;
    
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
        if (hp <= 0){
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != "floor") {
            direction = -direction;
        }

        if(collision.gameObject.tag == "projectile") {
            hp --;
        }
    }
}
