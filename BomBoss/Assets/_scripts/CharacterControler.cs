using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
//public class PlayerParameters
//{

//}

 

public class CharacterControler : MonoBehaviour {

    public bool isControllable = true;

    //reference to the statue script
    public CharacterStatus characterStatus;

    //for Healing
    public float maxHealth;

    //for moving
    public float speed, tilt, accelaration;
    private Rigidbody rigidbody;
    private Vector3 rotateDirection = Vector3.zero;
    public float rotateSpeed = 1.0f;

    //for firing
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate, nextFire;
    public int overheatRate, overheatTotal;
    public float cooldownRate;
    public bool isOverheated = false;

    
    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();

        //0.1seconds after the game starts the function " cannonColldown " will be called.
        //The function will be called every X seconds (as set by the "cooldownRate" variable in the player inspector)
        InvokeRepeating("cannonCooldown", 0.1f, cooldownRate);
        Physics.gravity = new Vector3(0, -20.0F, 0);
  
    }//End of Start


    // Update is called once per frame
    void Update () {
        if (Input.GetButton("Fire1") && (Time.time > nextFire) && (isOverheated == false) )
        {
            nextFire = Time.time + fireRate; //Set the next available time for firing a missile
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //spawn a missile
            overheatTotal += overheatRate; //Everytime the cannon fires, it's heat rises

            //check if the cannon has reached maximum overheat and needs to cool down
            if (overheatTotal >= 100)
            {
                overheatTotal = 100;
                isOverheated = true;
            }                
        }

       
    }//End of Update
    
    
    //==========================================================================================================//

    //do physics (transformations and rotations) in this function
    void FixedUpdate()
    {
        //check if the player is alive or not
        if (!isControllable)
            Input.ResetInputAxes();

        //get the user input in horizontal and vertical axis
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(0, 0, moveVertical);

        rigidbody.AddRelativeForce(movement * speed * accelaration, ForceMode.Impulse);
      //  rigidbody.velocity = (movement * speed * accelaration);
       
        //Find rotation based upon axis if need to turn
        if (moveHorizontal > 0)                                           //right turn
            rotateDirection = new Vector3(0, 1, 0);
        else if (moveHorizontal < 0)                                   //left turn
            rotateDirection = new Vector3(0, -1, 0);
        else                                                             //not turning
            rotateDirection = new Vector3(0, 0, 0);

        //rigidbody.transform.rotation *= Quaternion.Euler(rotateDirection * rotateSpeed);
        rigidbody.transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime, Space.Self);
     //   rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tilt);


    }


    //==========================================================================================================//


    //used for cannon mechanics
    void cannonCooldown()
    {
        overheatTotal -= 10;//reduce the overheat every second
        //When overheat reaches 0 (or below 0) then set the counter to 0 and change the flag. Player can now fire again
        if (overheatTotal <= 0)
        {
            overheatTotal = 0;
            isOverheated = false; // not the player can shoot with the cannon again
        }
    }




}//End of CharacterControllerr script

