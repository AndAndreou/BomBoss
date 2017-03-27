using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour {

    private bool enableTake;

    // Use this for initialization
    void Start () {
        enableTake = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            enableTake = !enableTake;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == GameRepository.bombTag)
        {
            if (enableTake)
            {
                other.GetComponent<BombHover>().Attach(this.transform.position);

                //other.GetComponent<Rigidbody>().isKinematic = true;
                //Destroy(other.GetComponent<Rigidbody>());
                //other.transform.position = this.transform.position;
                //other.transform.SetParent(this.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == GameRepository.bombTag)
        {
            if (enableTake)
            {
                other.GetComponent<BombHover>().Detach();

                //other.gameObject.AddComponent<Rigidbody>();
                //other.GetComponent<Rigidbody>().isKinematic = false;
                //other.transform.SetParent(null);
            }
        }
    }
}
