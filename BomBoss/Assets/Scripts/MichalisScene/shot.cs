using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shot : MonoBehaviour {

    public float shotSpeed;
    public float damageDealt;

    public GameObject mainProjectile;
    public ParticleSystem mainParticleSystem;

    // Use this for initialization
    void Start()
    {
        Vector3 fireAhead = this.transform.forward;
        GetComponent<Rigidbody>().velocity = fireAhead * shotSpeed;
        mainProjectile.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        if (mainParticleSystem.IsAlive() == false)
        {
            mainProjectile.SetActive(false);
        }
    }
}
