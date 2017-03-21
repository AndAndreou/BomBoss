using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider other)
    {
        MyLog(string.Format("Collided with: {0}", other.name));
        BombController bomb = other.GetComponent<BombController>();

        if (bomb!= null)
        {
            bomb.VoidCollided();
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Void-{0}", msg));
    }
}
