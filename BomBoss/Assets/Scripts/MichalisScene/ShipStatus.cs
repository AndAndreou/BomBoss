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
    Vector3 jumpVector = Vector3.up;
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
    public float maxTurboSpeed;
    public float maxTurboAcceleration;

    public HoverControl hoverControl; // To fetch myPlayer

    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        //jumpVector = new Vector3(0.0f, 1.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        
        //Check input from user to activate the power-ups
        if (Input.GetButtonDown("Boost" + hoverControl.myPlayer.ToString()) && hasBoost)
        {
            print("B boost key was pressed, activated boost");
           
            StartCoroutine(boostMe());
        }
        
       if (Input.GetButtonDown("Jump" + hoverControl.myPlayer.ToString()) && hasJump && jumpsLeft > 0)
       {
           print("N jump key was pressed, activated jump");
           
           rigidbody.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
            jumpsLeft--;
            if(jumpsLeft == 0)
            {
                hasJump = false;
            }

        }
        /*
        if (Input.GetButtonDown("Shield"  + hoverControl.myPlayer.ToString()) && hasShield)
        {
            print("M shield key was pressed, activated shield");
            StartCoroutine(shieldMe());
        }
        */
    }//END OF FIXED UPDATE


    //Used to substract health from the player and check if his HP reaches zero
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
            gameObject.GetComponent<MovementEngine>().MaxForwardAcceleration = maxTurboAcceleration;
            gameObject.GetComponent<MovementEngine>().MaxSpeed = maxTurboSpeed;
            
            timePassed += Time.deltaTime; // increment our timer
            yield return null;
        }

        if (timePassed >= boostDuration) //used to return the value back to its original value over time
        {
            gameObject.GetComponent<MovementEngine>().MaxForwardAcceleration = 20;
            gameObject.GetComponent<MovementEngine>().MaxSpeed = 25;
            hasBoost = false;

            yield return null;
        }
        

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

    //get a power up
    public void GetPowerUp(PowerUpType powerUp, float value)
    {
        switch (powerUp)
        {
            case PowerUpType.health:
                currHealth += value;
                Debug.Log("Health: " + currHealth);
                break;
            case PowerUpType.shield:
                currShieldHealth += value;
                Debug.Log("Shield: " + currShieldHealth);
                break;
            case PowerUpType.jump:
                jumpsLeft = 2;
                hasJump = true;
                Debug.Log("Jump: " + jumpsLeft);
                break;
            case PowerUpType.boost:
                //this.hasBoost = true;
                hasBoost = true;
               // boostDuration += value;
                Debug.Log("Boost: " + boostDuration);
                break;
        }
    }


}
