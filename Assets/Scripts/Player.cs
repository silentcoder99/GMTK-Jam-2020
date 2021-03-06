﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //physics variables
    private float dashForce = 1000f;
    private float jumpForce = 500f;
    private float speed = 10f;
    private float airSpeed = 10f;
    private float horizontalDrag = 0.1f;
    private float verticalDrag = 0.05f;

    private float movingFriction = 0f;
    private float stoppingFriction = 2f;

    //number of jumps, dashes, attacks.
    private int jumpCount = 1;
    private int dashCount = 1;
    private int attackCount = 1;

    //UI variables
    private GameObject jumpCounterObj;
    private Text jumpCounter;
    private GameObject dashCounterObj;
    private Text dashCounter;
    private GameObject attackCounterObj;
    private Text attackCounter;
    private GameObject cameraObj;
    
    private Text emptyText;
    private float emptyTextTimer = 0f;

    //ridigbody and sound variables
    private Rigidbody body;
    public Rigidbody projectile;
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip shootClip;
    public AudioClip emptyClip;

    private AudioSource jumpSource;
    private AudioSource dashSource;
    private AudioSource shootSource;
    private AudioSource emptySource;

    private Vector3 movement;
    private bool onFloor;
    private bool facingRight = true;
    

    void Awake() {
        jumpSource = gameObject.AddComponent<AudioSource>();
        dashSource = gameObject.AddComponent<AudioSource>();
        shootSource = gameObject.AddComponent<AudioSource>();
        emptySource = gameObject.AddComponent<AudioSource>();

        jumpSource.clip = jumpClip;
        dashSource.clip = dashClip;
        shootSource.clip = shootClip;
        emptySource.clip = emptyClip;
        emptySource.volume = 0.35f;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        setFriction(stoppingFriction);
        body.drag = 1f;
        body.freezeRotation = true;
        movement = Vector3.zero;

        //create UI
        jumpCounterObj = GameObject.Find("jumpCounter");
        jumpCounter = jumpCounterObj.GetComponent<Text>();

        dashCounterObj = GameObject.Find("dashCounter");
        dashCounter = dashCounterObj.GetComponent<Text>();

        attackCounterObj = GameObject.Find("attackCounter");
        attackCounter = attackCounterObj.GetComponent<Text>();

        emptyText = GameObject.Find("EmptyMessage").GetComponent<Text>();
        emptyText.enabled = false;

        //define camera
        cameraObj = GameObject.Find("Main Camera");
        
        //starting number of abillities per scene
        string sceneName = SceneManager.GetActiveScene().name;
        switch(sceneName){
            case "Level 0":
                jumpCount = 0;
                dashCount = 0;
                attackCount = 0;
                break;

            case "Level 0.5":
                jumpCount = 0;
                dashCount = 0;
                attackCount = 0;
                break;

            case "Level 1":
                jumpCount = 3;
                dashCount = 3;
                attackCount = 3;
                break;

            case "Level 2":
                jumpCount = 1;
                dashCount = 1;
                attackCount = 0;
                break;

            case "Level 3":
                jumpCount = 0;
                dashCount = 0;
                attackCount = 0;
                break;

            case "Ending":
                jumpCount = 99;
                dashCount = 99;
                attackCount = 99;
                break;

            default:
                jumpCount = 1;
                dashCount = 1;
                attackCount = 1;
                break;
        }

        if (checkpointStats.checkpointReached){
            returnToCheckpoint(checkpointStats.checkpointX, checkpointStats.checkpointY);
        }

        checkpointStats.checkpointReached = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if(emptyTextTimer >= 0) {
            emptyTextTimer -= Time.deltaTime;
        }

        if(emptyTextTimer < 0) {
            emptyText.enabled = false;
        }

        movement.x = Input.GetAxis("Horizontal");
        if (movement.x > 0 && !facingRight) {
            facingRight = true;

            transform.eulerAngles = new Vector3(0, 0, 0);
            cameraObj.transform.RotateAround(transform.position, Vector3.up, 180);
            emptyText.gameObject.transform.RotateAround(transform.position, Vector3.up, 180);
        }
        else if (movement.x < 0 && facingRight) {
            facingRight = false;

            transform.eulerAngles = new Vector3(0, 180, 0);
            cameraObj.transform.RotateAround(transform.position, Vector3.up, 180);
            emptyText.gameObject.transform.RotateAround(transform.position, Vector3.up, 180);
        }

        jump();

        if (Input.GetButtonDown("Dash")) {
            if(dashCount > 0) {
                dash(facingRight);
            }
            else {
                emptySource.Play();
                showEmptyMessage("No Dashes");
            }
        }

        if (Input.GetButtonDown("Fire1")) {
            if(attackCount > 0) {
                spawnProjectile(facingRight);
            }
            else {
                emptySource.Play();
                showEmptyMessage("No Fires");
            }
        }

        jumpCounter.text = jumpCount.ToString();
        dashCounter.text = dashCount.ToString();
        attackCounter.text = attackCount.ToString();

        if ((transform.position.y < -20) || (Input.GetButtonDown("Reset"))){
            kill();
        }
    }

    void FixedUpdate() {
        //Set friction based on input
        if(movement.x != 0 && Mathf.Sign(movement.x) == Mathf.Sign(body.velocity.x)) {
            setFriction(movingFriction);
        }
        else {
            setFriction(stoppingFriction);
        }

        //Apply horizontal and vertical drag
        Vector3 drag = Vector3.zero;
        drag.x = Mathf.Pow(body.velocity.x, 2) * horizontalDrag;
        drag.y = Mathf.Pow(body.velocity.y, 2) * verticalDrag;
        if(body.velocity.x > 0) {
            drag.x *= -1;
        }
        if(body.velocity.y > 0) {
            drag.y *= -1;
        }
        body.AddForce(drag);

        if(onFloor) {
            body.AddForce(movement * speed);
        }
        else {
            body.AddForce(movement * airSpeed);
        }
    }

    void showEmptyMessage(string message) {
        emptyText.text = message;
        emptyText.enabled = true;
        emptyTextTimer = 0.5f;
    }

    void setFriction(float friction) {
        Collider collider = GetComponent<Collider>();
        collider.material.dynamicFriction = friction;
        collider.material.staticFriction = friction;
    }

    void jump(){
        Vector3 jumpVector = Vector3.zero;

        if (Input.GetButtonDown("Jump")) {
            if(jumpCount > 0) {
                jumpVector.y = 1;
                body.AddForce(transform.up * jumpForce);
                jumpCount --;

                jumpSource.Play();
            }
            else {
                emptySource.Play();
                showEmptyMessage("No Jumps");
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        GameObject objectEncountered = other.gameObject;

        string objectName = other.gameObject.tag.ToString();

        switch(objectName){
            case "jump":
                jumpCount ++;
                break;
            case "dash":
                dashCount ++;
                break;
            case "attack":
                attackCount ++;
                break;
            case "checkpoint":
                checkpointTouched();
                break;
            case "Finish":
                nextLevel();
                break;
        }
        Destroy(objectEncountered);
    }

    void OnCollisionEnter(Collision other){
        GameObject objectEncountered = other.gameObject;

        string objectName = other.gameObject.tag.ToString();

        switch(objectName){
            case "enemy":
                kill();
                break;
        }

        if(objectName == "floor") {
            onFloor = true;

            Debug.Log("Hit floor");
        } 
    }

    void OnCollisionExit(Collision other) {
        if(other.gameObject.tag == "floor") {
            onFloor = false;

            Debug.Log("In air");
        }
    }

    private void dash(bool facingRight){
        if (facingRight){
            body.AddForce(Vector3.right * dashForce);
        }else{
            body.AddForce(Vector3.left * dashForce);
        }
        dashCount --;

        dashSource.Play();
    }

    private void spawnProjectile(bool facingRight){
            Rigidbody clone, negclone;
            if (facingRight){
                clone = Instantiate(projectile, (transform.position + new Vector3(1, 0, 0)), transform.rotation);
                clone.velocity = Vector3.right * 15;
            }else{
                negclone = Instantiate(projectile, (transform.position - new Vector3(1, 0, 0)), transform.rotation);
                negclone.transform.Rotate(0, 180, 0);
                negclone.velocity = -Vector3.right * 15;
            }
        attackCount --;   

        shootSource.Play();
    }

    private void kill(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void checkpointTouched(){
        checkpointStats.checkpointReached = true;
        checkpointStats.checkpointX = this.gameObject.transform.position.x;
        checkpointStats.checkpointY = this.gameObject.transform.position.y;  

        checkpointStats.checkpointJump = jumpCount;
        checkpointStats.checkpointDash = dashCount;
        checkpointStats.checkpointAttack = attackCount;
    }

    private void returnToCheckpoint(float xx, float yy){
        jumpCount = checkpointStats.checkpointJump;
        dashCount = checkpointStats.checkpointDash;
        attackCount = checkpointStats.checkpointAttack;
        transform.position = new Vector3(xx, yy, 0);
    }

    private void nextLevel(){
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}
