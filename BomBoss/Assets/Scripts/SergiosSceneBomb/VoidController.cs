using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidController : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //MyLog(string.Format("Collided with: {0}", other.name));

        if (other.tag == GameRepository.bombTag)
        {
            BombController bomb = other.GetComponent<BombController>();

            if (bomb != null)
            {
                bomb.VoidCollided();
            }

        }

        if (other.tag == GameRepository.hovercraftTag)
        {
            MyLog(string.Format("Collided with: {0}", other.tag));
            // This is no good now because we have 4 hover cars
            //GameObject ship = GameObject.Find("HoverCar");
            //if (ship != null)
            //{
            //    ship.GetComponent<ShipController>().Die();
            //}

            ShipController shipController = other.GetComponentInParent<ShipController>();
            if (shipController != null)
            {
                shipController.Die();
            }

        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Void-{0}", msg));
    }
}