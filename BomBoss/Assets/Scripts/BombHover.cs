using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHover : MonoBehaviour {

    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;

    public LayerMask raycastMask;

    private Rigidbody myrigidbody;

    [HideInInspector]
    public Vector3 attachPoint;
    [HideInInspector]
    public bool setAttachPoint;
    private bool respawn;

    public Magnet magnet;

    void Awake()
    {
        myrigidbody = GetComponent<Rigidbody>();
        setAttachPoint = false;
        respawn = false;
    }

    private void OnTriggerStay(Collider other)
    {
       
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Hook")
        {
            this.transform.position = new Vector3(this.transform.position.x, other.transform.position.y, this.transform.position.z);
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
    */
    // Update is called once per frame
    void FixedUpdate() {
        //myrigidbody = GetComponent<Rigidbody>(); //Was called on awake not needed here
        if (setAttachPoint == true)
        {
            //this.transform.position = attachPoint;
            myrigidbody.velocity = (attachPoint - this.transform.position) / Time.deltaTime ;
           // rigidbody.AddForce(attachPoint - this.transform.position);
            return;
        }

        if (!respawn)
        {
            Ray ray = new Ray(transform.position, new Vector3(transform.position.x, transform.position.y - 1000f, transform.position.z));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, hoverHeight, raycastMask))
            {
                if (hit.transform.tag != GameRepository.hovercraftTag)
                {
                    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
                    Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
                    myrigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
                }
            }
        }
    }

    public void Attach(Vector3 myAttachPoint)
    {
        attachPoint = myAttachPoint;
        setAttachPoint = true;
        respawn = false;
    }

    public void Detach()
    {
        setAttachPoint = false;
        respawn = false;
    }

    public void Respawn(Vector3 mySpawnPoint)
    {
        
        // Completely release bomb
        magnet.Detach(GetComponent<Collider>());
        attachPoint = mySpawnPoint;
        setAttachPoint = false;
        respawn = true;
    }
}
