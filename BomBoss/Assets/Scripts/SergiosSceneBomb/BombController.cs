using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombState
{
    public const int stateArmed = 0;
    public const int stateExploding = 1;
    public const int stateExploded = 2;

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
    public BombState currentState = new BombState(); // Can be ARMED, EXPLODING, EXPLODED

    public Transform spawnPoint; // Spawn point

    // Use this for initialization
    void Start () {
        currentState.state = BombState.stateExploded;
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
        if (currentState.state == BombState.stateArmed && newState.state != BombState.stateExploding)
        {
            Debug.Log("Invalid bomb state transition");
        }

        if (currentState.state == BombState.stateExploding && newState.state != BombState.stateExploded)
        {
            Debug.Log("Invalid bomb state transition");
        }

        if (currentState.state == BombState.stateExploded && newState.state != BombState.stateArmed)
        {
            Debug.Log("Invalid bomb state transition");
        }

        if (newState.state == BombState.stateArmed)
        {
            ResetTimers();
            currentExplodeTime = Random.Range(minBombExplodeTime, maxBombExplodeTime);
            currentExplodeTimeYellow = ((float)currentExplodeTime) / 3; // One third
            currentExplodeTimeRed = currentExplodeTimeYellow * 2; // Two thirds

        }
        else if (newState.state == BombState.stateExploding)
        {

        }
        else if (newState.state == BombState.stateExploded)
        {

        }

        currentState = newState;
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

    void FixedUpdate ()
    {
        // Movement of bomb (only necessary for testing)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().AddForce(movement * 10);
    }

    void UpdateUI ()
    {
        bombInfoText.text = string.Format("Bomb Status: {0} TimerArmed: {1:00.00} TimerExploding: {2:00.00} TimerExploded: {3:00.00}", currentState.ToString(), timeElapsedArmed, timeElapsedExploding, timeElapsedExploded);
    }
}
