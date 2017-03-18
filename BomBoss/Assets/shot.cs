using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shot : MonoBehaviour {

    public float shotSpeed;
    public float damageDealt;

    // Use this for initialization
    void Start()
    {
        Vector3 fireAhead = new Vector3(0,0,1);
        GetComponent<Rigidbody>().velocity = fireAhead * shotSpeed;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
