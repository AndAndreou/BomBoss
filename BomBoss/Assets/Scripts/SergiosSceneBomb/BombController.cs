using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombController : MonoBehaviour {

    // UI
    public Text bombInfoText;
    // Times
    public int minBombExplodeTime = 10;
    public int maxBombExplodeTime = 20;
    public int currentExplodeTime; // To see when to change color of bomb
    public float currentExplodeTimeRed; // Near explode time
    public float currentExplodeTimeYellow;
    public float timeElapsedArmed;
    public float timeElapsedExploding;
    public float timeElapsedExploded;
    public float maxTimeElapsedExploding = 2;
    public float maxTimeElapsedExploded = 3;
    public BombState currentState = BombState.paused; // Can be ARMED, EXPLODING, EXPLODED, PAUSED
    public BombState beforePauseState = BombState.isNull;
    public Vector3 beforePauseVelocity;
    public Vector3 beforePauseAngularVelocity;
    private bool unpausing;

    public Transform spawnPoint; // Spawn point
    public GameObject gameManager;

    public GameObject explosionEffect;

    // Use this for initialization
    void Awake () {
        //currentState.myState = BombState.statePaused; //Moved this to variable declaration
        maxTimeElapsedExploding = explosionEffect.GetComponent<ParticleSystem>().main.duration;
        UpdateUI();
	}

    void ResetTimers ()
    {
        timeElapsedArmed = 0;
        timeElapsedExploding = 0;
        timeElapsedExploded = 0;
    }

    void IncrementTimers (float deltaTime)
    {
        if (currentState == BombState.armed)
        {
            timeElapsedArmed += deltaTime;
        }
        else if (currentState == BombState.exploding)
        {
            timeElapsedExploding += deltaTime;
        }
        else if (currentState == BombState.exploded)
        {
            timeElapsedExploded += deltaTime;
        }
    }

    void SetState (BombState newState)
    {
        // State Transition Validation Logic
        if (currentState == BombState.armed && newState != BombState.exploding && newState != BombState.paused)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.exploding && newState != BombState.exploded)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.exploded && newState != BombState.armed)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState == BombState.paused && newState != BombState.unPausing && newState != BombState.armed)
        {
            MyLog("Invalid bomb state transition");
        }

        // Special validations
        if (newState == BombState.armed && gameManager.GetComponent<GameManagerBomb>().currentState == RoundState.finished)
        {
            // If round ended don't allow re-arming of bomb
            return;
        }

        // Store the previousState
        BombState previousState = currentState;
        // Update the currentState
        currentState = newState;

        // Actions to do AFTER updating the currentState
        if (newState == BombState.armed)
        {
            MyLog(newState.ToString());
            if (! unpausing)
            {
                // Only reset timers when not called after unpause
                ResetTimers();
                currentExplodeTime = Random.Range(minBombExplodeTime, maxBombExplodeTime);
                currentExplodeTimeYellow = ((float)currentExplodeTime) / 3; // One third
                currentExplodeTimeRed = currentExplodeTimeYellow * 2; // Two thirds

                if (previousState == BombState.exploded)
                {
                    ResetToSpawnPoint();
                }

                gameObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                unpausing = false;
            }
        }
        else if (newState == BombState.exploding)
        {
            MyLog(newState.ToString());
            // Detach bomb from car
            this.GetComponent<BombHover>().Respawn(spawnPoint.position);
            gameObject.GetComponent<Renderer>().enabled = false;
            explosionEffect.GetComponent<ExplosionController>().PlayEffect();
            //explosionEffect.GetComponent<ParticleSystem>().Play();
            //Debug.Log("Looping: " + explosionEffect.GetComponent<ParticleSystem>().main.loop);
            StopMovement();
        }
        else if (newState == BombState.exploded)
        {
            MyLog(newState.ToString());
            explosionEffect.GetComponent<ExplosionController>().StopEffect();
            //explosionEffect.GetComponent<ParticleSystem>().Stop(); // Not needed because I disabled looping in all particle systems
        }
        else if (newState == BombState.paused)
        {
            MyLog(newState.ToString());
            if (beforePauseState == BombState.exploding)
            {
                explosionEffect.GetComponent<ExplosionController>().PauseEffect();
                //explosionEffect.GetComponent<ParticleSystem>().Pause();// playbackSpeed = 0f;
            }
        }
        else if (newState == BombState.unPausing)
        {
            MyLog(newState.ToString());
            if (beforePauseState == BombState.exploding)
            {
                explosionEffect.GetComponent<ExplosionController>().PlayEffect();
                //explosionEffect.GetComponent<ParticleSystem>().Play(); // playbackSpeed = 1f;
            }
            unpausing = true;
            SetState(beforePauseState);
        }
        else
        {
            MyLog("Unknown bomb state");
        }
    }

    public void Arm()
    {
        BombState newState = BombState.armed;
        SetState(newState);
    }

    public void Pause()
    {
        // Store the current values for pause state, velocity and angularVelocity
        beforePauseState = currentState;
        beforePauseVelocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);
        beforePauseAngularVelocity = new Vector3(this.GetComponent<Rigidbody>().angularVelocity.x, this.GetComponent<Rigidbody>().angularVelocity.y, this.GetComponent<Rigidbody>().angularVelocity.z);
        // Stop all forces on bomb
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        SetState(BombState.paused);
    }

    public void UnPause()
    {
        if (beforePauseState != BombState.isNull)
        {
            this.GetComponent<Rigidbody>().velocity = beforePauseVelocity;
            this.GetComponent<Rigidbody>().angularVelocity = beforePauseAngularVelocity;

            SetState(BombState.unPausing);
        }
    }

    public void Explode()
    {
        SetState(BombState.exploding);
    }

    public void VoidCollided()
    {
        ResetToSpawnPoint();
    }

    void ResetToSpawnPoint()
    {
        // Hide bomb and disable collider
        this.GetComponent<Renderer>().enabled = false;
        this.GetComponent<SphereCollider>().enabled = false;
        // Move it to the spawn point
        this.transform.position = spawnPoint.position;
        StopMovement();
        // Show bomb and enable collider
        this.GetComponent<Renderer>().enabled = true;
        this.GetComponent<SphereCollider>().enabled = true;
    }

    void StopMovement()
    {
        // Stop all forces on bomb
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
	
    void HandleBombState()
    {
        if (currentState == BombState.armed)
        {
            if (timeElapsedArmed > currentExplodeTime)
            {
                SetState(BombState.exploding);
            }
            else
            {
                // Handle the updating of the bomb color
                if (timeElapsedArmed < currentExplodeTimeYellow)
                {
                    this.GetComponent<Renderer>().material.color = Color.green;
                }
                else if (timeElapsedArmed < currentExplodeTimeRed && timeElapsedArmed >= currentExplodeTimeYellow)
                {
                    this.GetComponent<Renderer>().material.color = Color.yellow;
                }
                else 
                {
                    this.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else if (currentState == BombState.exploding)
        {
            if (timeElapsedExploding > maxTimeElapsedExploding)
            {
                SetState(BombState.exploded);
            }
        }
        else if (currentState == BombState.exploded)
        {
            if (timeElapsedExploded > maxTimeElapsedExploded)
            {
                SetState(BombState.armed);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        IncrementTimers(Time.deltaTime);
        HandleBombState();
        UpdateUI();
	}

    //void FixedUpdate()
    //{

    //    if (currentState == BombState.armed) // bomb can move only when armed
    //    {

    //        // movement of bomb (only necessary for testing)
    //        float movehorizontal = Input.GetAxis("Horizontal");
    //        float movevertical = Input.GetAxis("Vertical");

    //        Vector3 movement = new Vector3(movehorizontal, 0.0f, movevertical);
    //        GetComponent<Rigidbody>().AddForce(movement * 10);
    //    }
    //}

    void UpdateUI ()
    {
        bombInfoText.text = string.Format("Bomb Status: {0} TimerArmed: {1:00.00} TimerExploding: {2:00.00} TimerExploded: {3:00.00}", currentState.ToString(), timeElapsedArmed, timeElapsedExploding, timeElapsedExploded);
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("Bomb-{0}", msg));
    }
}
