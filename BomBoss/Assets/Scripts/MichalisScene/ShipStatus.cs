using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatus : MonoBehaviour {

    ShipController shipController;
    MovementEngine movementEngine;
    Rigidbody rigidbody;

    //for Healing
    public float maxHealth;
    public float currHealth;

    //for jumping
    Vector3 jumpVector;
    public float jumpForce;
    public float jumpsLeft;
    public bool hasJump = true;

    //for shield
    public bool hasShield = false;
    public float shieldtDuration;
    public float maxShieldHealth;
    public float currShieldHealth;

    //for boost
    public bool hasBoost = true;
    public float boostDuration;


    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        jumpVector = new Vector3(0.0f, 1.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {

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
            rigidbody.AddForce(jumpVector * jumpForce, ForceMode.Impulse);// TO DO: make the jump smoother
        }

        if (Input.GetKeyDown("m") && hasShield)
        {
            print("M key was pressed, activated shield");
            StartCoroutine(shieldMe());
        }

    }//END OF FIXED UPDATE


    //Used to substruct health from the player and check if his HP reaches zero
    public void applyDamage(float damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            shipController.Die();           
        }
    }

    //Boost mechanics
    IEnumerator boostMe()
    {
        float timePassed = 0; //Used for time counting

        while (timePassed <= boostDuration) //check if the boost duration is over
        {
            movementEngine.MaxSpeed = 40;
            movementEngine.MaxForwardAcceleration = 40;
            //accelaration = 5; //Had to use a hard-coded value. I tried to do: accelaration *= 5; but the value hit infinity. TODO find a better solution for this
            timePassed += Time.deltaTime; // increment our timer
            yield return null;
        }

        while (timePassed > 0) //used to return the value back to its original value over time
        {
            movementEngine.MaxSpeed = 25;
            movementEngine.MaxForwardAcceleration = 20;
           // accelaration = 2; //TODO: find a better solution for this that is not hardcoded in
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





}
