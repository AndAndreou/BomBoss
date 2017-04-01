﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    //for firing
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public int overheatRate;
    public float overheatTotal;
    public int maxOverheat = 100;
    public float cooldownRate;

    private bool isOverheated = false;
    private float nextFire;

    public Transform shipSpawn; // Spawn position

    // Use this for initialization
    void Start () {

        //0.1seconds after the game starts the function " cannonColldown " will be called.
        //The function will be called every X seconds (as set by the "cooldownRate" variable in the player inspector)
        InvokeRepeating("cannonCooldown", 0.1f, cooldownRate);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1") && (Time.time > nextFire) && (isOverheated == false))
        {
            nextFire = Time.time + fireRate; //Set the next available time for firing a missile
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //spawn a missile
            overheatTotal += overheatRate; //Everytime the cannon fires, it's heat rises

            //check if the cannon has reached maximum overheat and needs to cool down
            if (overheatTotal >= maxOverheat)
            {
                overheatTotal = maxOverheat;
                isOverheated = true;
            }
        }

    }

    public void Die()
    {
       // print("this works");
               
        gameObject.SetActive(false);
       
        // Move it to the spawn point
        this.transform.position = shipSpawn.position;
        //yield return new WaitForSeconds(seconds);

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

       
        gameObject.SetActive(true);
       
    }

    //used for cannon mechanics
    void cannonCooldown()
    {
        overheatTotal -= overheatRate;//reduce the overheat every second
        //When overheat reaches 0 (or below 0) then set the counter to 0 and change the flag. Player can now fire again
        if (overheatTotal <= 0)
        {
            overheatTotal = 0;
            isOverheated = false; // not the player can shoot with the cannon again
        }
    }

}//END OF MONOBEHAVOUR
