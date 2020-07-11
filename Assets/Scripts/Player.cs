using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float dashForce = 500f;
    public float jumpForce = 500f;
    public float speed = 10f;
    public float airSpeed = 10f;

    public float movingFriction = 0.8f;
    public float stoppingFriction = 1000f;

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

    public Rigidbody projectile;

    private Vector3 movement;
    private bool onFloor;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        setFriction(stoppingFriction);
        movement = Vector3.zero;

        jumpCounterObj = GameObject.Find("jumpCounter");
        jumpCounter = jumpCounterObj.GetComponent<Text>();

        dashCounterObj = GameObject.Find("dashCounter");
        dashCounter = dashCounterObj.GetComponent<Text>();

        attackCounterObj = GameObject.Find("attackCounter");
        attackCounter = attackCounterObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {   
        movement.x = Input.GetAxis("Horizontal");

        jump();

        if (Input.GetButtonDown("Dash") && (dashCount > 0) && movement.x != 0){
            body.AddForce(movement * dashForce);
            dashCount --;
        }

        if (Input.GetButtonDown("Fire1") && (attackCount > 0) && (movement.x > 0)){
            spawnProjectile(true);
        }
        else if (Input.GetButtonDown("Fire1") && (attackCount > 0) && (movement.x < 0)){
            spawnProjectile(false);
        }

        jumpCounter.text = jumpCount.ToString();
        dashCounter.text = dashCount.ToString();
        attackCounter.text = attackCount.ToString();

        if (transform.position.y < -100){
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

        if(onFloor) {
            body.AddForce(movement * speed);
        }
        else {
            body.AddForce(movement * airSpeed);
        }
    }

    void setFriction(float friction) {
        Collider collider = GetComponent<Collider>();
        collider.material.dynamicFriction = friction;
        collider.material.staticFriction = friction;
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
        GameObject objectEncountered = other.gameObject;

        string objectName = other.gameObject.tag.ToString();

        switch(objectName){
            case "jump":
                jumpCount ++;
                Destroy(objectEncountered);
                break;
            case "dash":
                dashCount ++;
                Destroy(objectEncountered);
                break;
            case "attack":
                attackCount ++;
                Destroy(objectEncountered);
                break;
            case "Finish":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
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
            

    }

    private void kill(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void nextLevel(){
        
    }
}
