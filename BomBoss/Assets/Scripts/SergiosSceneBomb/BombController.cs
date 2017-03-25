using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombState
{
    public const int stateArmed = 0;
    public const int stateExploding = 1;
    public const int stateExploded = 2;
    public const int statePaused = 3;
    public const int stateUnPausing = 4;

    public int state;

    public BombState ()
    {
        
    }

    public BombState (int newState)
    {
        state = newState;
    }

    public override string ToString ()
    {
        string msg = "";
        if (state == stateArmed)
        {
            msg = "ARMED";
        }
        else if (state == stateExploding)
        {
            msg = "EXPLODING";
        }
        else if (state == stateExploded)
        {
            msg = "EXPLODED";
        }
        else if (state == statePaused)
        {
            msg = "PAUSED";
        }
        else if (state == stateUnPausing)
        {
            msg = "UNPAUSING";
        }
        else
        {
            msg = "UNKNOWN";
        }
        return msg;
    }
}

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
    public BombState currentState = new BombState(); // Can be ARMED, EXPLODING, EXPLODED, PAUSED
    public BombState beforePauseState;
    public Vector3 beforePauseVelocity;
    public Vector3 beforePauseAngularVelocity;
    private bool unpausing;

    public Transform spawnPoint; // Spawn point
    public GameObject gameManager;

    // Use this for initialization
    void Awake () {
        currentState.state = BombState.statePaused;
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
        if (currentState.state == BombState.stateArmed)
        {
            timeElapsedArmed += deltaTime;
        }
        else if (currentState.state == BombState.stateExploding)
        {
            timeElapsedExploding += deltaTime;
        }
        else if (currentState.state == BombState.stateExploded)
        {
            timeElapsedExploded += deltaTime;
        }
    }

    void SetState (BombState newState)
    {
        // State Transition Validation Logic
        if (currentState.state == BombState.stateArmed && newState.state != BombState.stateExploding && newState.state != BombState.statePaused)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState.state == BombState.stateExploding && newState.state != BombState.stateExploded)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState.state == BombState.stateExploded && newState.state != BombState.stateArmed)
        {
            MyLog("Invalid bomb state transition");
        }

        if (currentState.state == BombState.statePaused && newState.state != BombState.stateUnPausing)
        {
            MyLog("Invalid bomb state transition");
        }

        // Special validations
        if (newState.state == BombState.stateArmed && gameManager.GetComponent<GameManagerBomb>().currentState.state == RoundState.stateFinished)
        {
            // If round ended don't allow re-arming of bomb
            return;
        }

        // Store the previousState
        BombState previousState = currentState;
        // Update the currentState
        currentState = newState;

        // Actions to do AFTER updating the currentState
        if (newState.state == BombState.stateArmed)
        {
            MyLog(newState.ToString());
            if (! unpausing)
            {
                // Only reset timers when not called after unpause
                ResetTimers();
                currentExplodeTime = Random.Range(minBombExplodeTime, maxBombExplodeTime);
                currentExplodeTimeYellow = ((float)currentExplodeTime) / 3; // One third
                currentExplodeTimeRed = currentExplodeTimeYellow * 2; // Two thirds
            }
            else
            {
                unpausing = false;
            }
        }
        else if (newState.state == BombState.stateExploding)
        {
            MyLog(newState.ToString());
            StopMovement();
        }
        else if (newState.state == BombState.stateExploded)
        {
            MyLog(newState.ToString());
        }
        else if (newState.state == BombState.statePaused)
        {
            MyLog(newState.ToString());
        }
        else if (newState.state == BombState.stateUnPausing)
        {
            MyLog(newState.ToString());
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
        SetState(new BombState(BombState.stateArmed));
    }

    public void Pause()
    {
        // Store the current values for pause state, velocity and angularVelocity
        beforePauseState = new BombState(currentState.state);
        beforePauseVelocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, this.GetComponent<Rigidbody>().velocity.y, this.GetComponent<Rigidbody>().velocity.z);
        beforePauseAngularVelocity = new Vector3(this.GetComponent<Rigidbody>().angularVelocity.x, this.GetComponent<Rigidbody>().angularVelocity.y, this.GetComponent<Rigidbody>().angularVelocity.z);
        // Stop all forces on bomb
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        SetState(new BombState(BombState.statePaused));
    }

    public void UnPause()
    {
        if (beforePauseState != null)
        {
            this.GetComponent<Rigidbody>().velocity = beforePauseVelocity;
            this.GetComponent<Rigidbody>().angularVelocity = beforePauseAngularVelocity;

            SetState(new BombState(BombState.stateUnPausing));
        }
    }

    public void Explode()
    {
        SetState(new BombState(BombState.stateExploding));
    }

    public void VoidCollided()
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
        if (currentState.state == BombState.stateArmed)
        {
            if (timeElapsedArmed > currentExplodeTime)
            {
                SetState(new BombState(BombState.stateExploding));
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
        else if (currentState.state == BombState.stateExploding)
        {
            if (timeElapsedExploding > maxTimeElapsedExploding)
            {
                SetState(new BombState(BombState.stateExploded));
            }
        }
        else if (currentState.state == BombState.stateExploded)
        {
            if (timeElapsedExploded > maxTimeElapsedExploded)
            {
                SetState(new BombState(BombState.stateArmed));
            }
        }
    }

    // Update is called once per frame
    void Update () {
        IncrementTimers(Time.deltaTime);
        HandleBombState();
        UpdateUI();
	}

    //void fixedupdate ()
    //{

    //    if (currentstate.state == bombstate.statearmed) // bomb can move only when armed
    //    {

    //        // movement of bomb (only necessary for testing)
    //        float movehorizontal = input.getaxis("horizontal");
    //        float movevertical = input.getaxis("vertical");

    //        vector3 movement = new vector3(movehorizontal, 0.0f, movevertical);
    //        getcomponent<rigidbody>().addforce(movement * 10);
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
