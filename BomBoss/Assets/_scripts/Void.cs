using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Void : MonoBehaviour {

    public float damage = 100;
    public CharacterStatus characterStatus;


   // when a player enters this void, simply deal enough damage to kill him instantly
    private void OnTriggerEnter(Collider other)
    {
        print("Dead!"); //for debugging
        GameObject go = GameObject.FindWithTag("Player");
        go.GetComponent<CharacterStatus>().applyDamage(damage);
        //characterStatus = GetComponent<CharacterStatus>(); //so that we can access the function from the other script
        //characterStatus.currHealth = 0;

    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
