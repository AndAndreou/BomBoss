using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnet : MonoBehaviour {

    public LayerMask includeLayers;

    List<Collider> TriggerList = new List<Collider>();
    public float force;

   
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        foreach(Collider trigger in TriggerList)
        {
            BombHover bh = trigger.GetComponent<BombHover>();
            if ((bh == null) || ((bh != null) &&(bh.setAttachPoint ==false)))
            {
                Vector3 v = this.transform.position - trigger.gameObject.transform.position;
                float d = Vector3.Distance(trigger.gameObject.transform.position, this.transform.position);
                Rigidbody r = trigger.gameObject.GetComponent<Rigidbody>();
                r.AddForce(v.normalized * force / d);
                Vector3 vAddForce = v.normalized * force / d;
                MyLog(vAddForce.ToString());
            }
        }
		
	}
 
     //called when something enters the trigger
     void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & includeLayers) != 0)
        {

            //if the object is not already in the list
            if (!TriggerList.Contains(other))
            {
                //add the object to the list
                TriggerList.Add(other);
            }
        }
    }

    //called when something exits the trigger
    void OnTriggerExit(Collider other)
    {
        //if the object is in the list
        if (TriggerList.Contains(other))
        {
            //remove it from the list
            TriggerList.Remove(other);
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Manget-{0}", msg));
    }
}
