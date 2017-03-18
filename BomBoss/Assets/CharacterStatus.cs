using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {

    public CharacterControler characterControler;

    private Rigidbody rigidbody;
    public float speed, tilt, accelaration;

    //for Healing
    public float maxHealth;
    public float currHealth;

    //for jumping
    Vector3 jump;
    public float jumpForce;
    public bool isGrounded;
    public float jumpsLeft;
    private bool hasJump = true;


    //for shield
    private bool hasShield = false;
    public float shieldtDuration;
    public float maxShieldHealth;
    public float currShieldHealth;

    //for boost
    private bool hasBoost = true;
    public float boostDuration;

    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -20.0F, 0);
        jump = new Vector3(0.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void FixedUpdate() {

        //Check input from user to activate the power-ups
        if (Input.GetKeyDown("b") && hasBoost)
        {
            print("B key was pressed, activated boost");
            StartCoroutine(boostMe());
        }

        if (Input.GetKeyDown("n") && hasJump && jumpsLeft > 0)
        {
            print("N key was pressed, activated jump");
            jumpsLeft--;
            rigidbody.AddForce(jump * jumpForce, ForceMode.Impulse);// TO DO: make the jump smoother
        }

        if (Input.GetKeyDown("m") && hasShield)
        {
            print("M key was pressed, activated shield");
            StartCoroutine(shieldMe());
        }

    }

    //Used to substruct health from the player and check if his HP reaches zero
    public void applyDamage(float damage) {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0; 
            Die();
        }
    }

    //Used to remove control from the player and respawn him at a set position
    IEnumerator Die()
    {
        print("dead!");
        characterControler.isControllable = false;
        gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(2);
        rigidbody.transform.position = new Vector3(1f, 0.15f, 0f);
        revivePlayer();
    }

    //Used to give control back to the player
    void revivePlayer()
    {
        gameObject.SetActive(true);
        characterControler.isControllable = true;
        currHealth = maxHealth;
    }


    //Boost mechanics
    IEnumerator boostMe()
    {
        float timePassed = 0; //Used for time counting

        while (timePassed <= boostDuration) //check if the boost duration is over
        {
            accelaration = 5; //Had to use a hard-coded value. I tried to do: accelaration *= 5; but the value hit infinity. TODO find a better solution for this
            timePassed += Time.deltaTime; // increment our timer
            yield return null;
        }

        while (timePassed > 0) //used to return the value back to its original value over time
        {
            accelaration = 2; //TODO: find a better solution for this that is not hardcoded in
            timePassed -= Time.deltaTime;
            yield return null;
        }
        hasBoost = false;

    }//End of boostME IEnumerator

    //Shield mechanics
    IEnumerator shieldMe()
    {
        float shieldTimePassed = 0; //Used for time counting

        while (shieldTimePassed <= shieldtDuration) //check if the boost duration is over
        {
            //TODO: Check if player was hit by a missile or AI bot

            //when shield cannot take any more hits, deativate it
            if (currShieldHealth <= 0)
            {
                hasShield = false; //TODO: Will the shield be a particle / object? If so then does it need to be set: active=false ?
            }
            yield return null;
        }
        hasShield = false;
    }//End of shieldME IEnumerator


    //For checking when the player collides with the power ups, to pick them up
    void OnTriggerEnter(Collider other)
    {
        //When Health is picked up
        if (other.gameObject.CompareTag("HealthPowerUp"))
        {
            other.gameObject.SetActive(false);
            maxHealth += maxHealth / 2; //heals the player for half of his max health as soon as he picks up the power-up
        }

        //When Boost is picked up
        if (other.gameObject.CompareTag("BoostPowerUP"))
        {
            other.gameObject.SetActive(false);
            hasBoost = true; //enables the boost power-up
        }

        //When Jump is picked up
        if (other.gameObject.CompareTag("JumpPowerUP"))
        {
            other.gameObject.SetActive(false);
            hasJump = true; //enables the jump power-up
            jumpsLeft = 2; //sets the remaining jumps to 2 so that the player can double jump
        }

        //When Shield is picked up
        if (other.gameObject.CompareTag("ShieldPowerUP"))
        {
            other.gameObject.SetActive(false);
            hasShield = true; //enables the shield power-up
            currShieldHealth = maxShieldHealth;
        }

    }

}
