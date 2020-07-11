﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float dashForce = 500f;
    public float jumpForce = 500f;
    public float speed = 5f;

    public int jumpCount = 1;
    public int dashCount = 0;
    public int attackCount = 0;

    private Rigidbody body;
    private GameObject jumpCounterObj;
    private Text jumpCounter;
    private GameObject dashCounterObj;
    private Text dashCounter;
    private GameObject attackCounterObj;
    private Text attackCounter;

    private GameObject projectile;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();

        jumpCounterObj = GameObject.Find("jumpCounter");
        jumpCounter = jumpCounterObj.GetComponent<Text>();

        dashCounterObj = GameObject.Find("dashCounter");
        dashCounter = dashCounterObj.GetComponent<Text>();

        attackCounterObj = GameObject.Find("attackCounter");
        attackCounter = dashCounterObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 movement = Vector3.zero;
        movement.x = Input.GetAxis("Horizontal");

        body.AddForce(movement * speed);

        jump();

        if (Input.GetButtonDown("Dash") && (dashCount > 0) && movement.x != 0){
            body.AddForce(movement * dashForce);
            dashCount --;
        }

        if (Input.GetButtonDown("Fire1") && (attackCount > 0)){
            spawnProjectile();
        }

        jumpCounter.text = jumpCount.ToString();
        dashCounter.text = dashCount.ToString();
        attackCounter.text = attackCount.ToString();

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

        string pickupName = other.gameObject.tag.ToString();

        switch(pickupName){
            case "jump":
                jumpCount ++;
                break;
            case "dash":
                dashCount ++;
                break;
            case "attack":
                attackCount ++;
                break;
            case "enemy":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "Finish":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }

        Destroy(powerUp);
    }

    private void spawnProjectile(){
        GameObject instProjectile = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        Rigidbody instProjectileRigidbody = instProjectile.GetComponent<Rigidbody>();
        instProjectileRigidbody.AddForce(Vector3.forward * 10);
    }
}
