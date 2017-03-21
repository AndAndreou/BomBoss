using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBomb : MonoBehaviour {

    public GameObject bomb;
    private Vector3 offset;

    // Use this for initialization
    void Start () {
        offset = transform.position - bomb.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void LateUpdate()
    {
        transform.position = bomb.transform.position + offset;
    }
}
