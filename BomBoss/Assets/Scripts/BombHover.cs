using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHover : MonoBehaviour {

    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;

    public LayerMask raycastMask;

    private Rigidbody rigidbody;

    [HideInInspector]
    public Vector3 attachPoint;
    [HideInInspector]
    public bool setAttachPoint;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        setAttachPoint = false;
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
        rigidbody = GetComponent<Rigidbody>();
        if (setAttachPoint ==true)
        {
            //this.transform.position = attachPoint;
            rigidbody.velocity = (attachPoint - this.transform.position) / Time.deltaTime ;
           // rigidbody.AddForce(attachPoint - this.transform.position);
            return;
        }

        Ray ray = new Ray(transform.position, new Vector3(transform.position.x, transform.position.y-1000f, transform.position.z));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight, raycastMask))
        {
            if (hit.transform.tag != GameRepository.hovercraftTag)
            {
                float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
                Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
                rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
            }
        }
    }

}
