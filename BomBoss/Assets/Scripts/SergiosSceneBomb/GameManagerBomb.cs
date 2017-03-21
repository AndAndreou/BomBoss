using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundState
{
    public const int stateStarting = 0;
    public const int stateRunning = 1;
    public const int statePaused = 2;
    public const int stateUnPausing = 3;
    public const int stateFinished = 4;

    public int state;

    public RoundState()
    {

    }

    public RoundState(int newState)
    {
        state = newState;
    }

    public override string ToString()
    {
        string msg = "";
        if (state == stateStarting)
        {
            msg = "STARTING";
        }
        else if (state == stateRunning)
        {
            msg = "RUNNING";
        }
        else if (state == statePaused)
        {
            msg = "PAUSED";
        }
        else if (state == stateUnPausing)
        {
            msg = "UNPAUSING";
        }
        else if (state == stateFinished)
        {
            msg = "FINISHED";
        }
        else
        {
            msg = "UNKNOWN";
        }
        return msg;
    }
}

public class GameManagerBomb : MonoBehaviour {

    public GameObject bomb;
    private Vector3 offset;

    // UI
    public Text roundInfoText;
    // Times
    public int maxRoundTime = 120; // secs
    public float timeElapsed;

    public RoundState currentState = new RoundState(); // Can be STARTING, RUNNING, PAUSED, FINISHED

    // Use this for initialization
    void Start () {
        offset = transform.position - bomb.transform.position;
        currentState.state = RoundState.stateFinished;
        SetState(new RoundState(RoundState.stateStarting));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseRound();
        }

        IncrementTimers(Time.deltaTime);
        HandleRoundState();
        UpdateUI();
    }

    void LateUpdate()
    {
        transform.position = bomb.transform.position + offset;
    }

    void UpdateUI()
    {
        roundInfoText.text = string.Format("Round Status: {0} Timer: {1:00.00}", currentState.ToString(), timeElapsed);
    }

    void TogglePauseRound()
    {
        if (currentState.state == RoundState.stateRunning)
        {
            SetState(new RoundState(RoundState.statePaused));
        }
        else if (currentState.state == RoundState.statePaused)
        {
            SetState(new RoundState(RoundState.stateUnPausing));
        }
    }

    void PauseRound()
    {
        bomb.GetComponent<BombController>().Pause();
    }

    void UnPauseRound()
    {
        bomb.GetComponent<BombController>().UnPause();
        SetState(new RoundState(RoundState.stateRunning));
    }

    void ResetTimers()
    {
        timeElapsed = 0;
    }

    void IncrementTimers(float deltaTime)
    {
        if (currentState.state == RoundState.stateRunning)
        {
            timeElapsed += deltaTime;
        }
    }

    void SetState(RoundState newState)
    {
        // State Transition Validation Logic
        if (currentState.state == RoundState.stateStarting && newState.state != RoundState.stateRunning)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState.state == RoundState.stateRunning && (newState.state != RoundState.statePaused && newState.state != RoundState.stateFinished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState.state == RoundState.statePaused && newState.state != RoundState.stateUnPausing)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState.state == RoundState.stateUnPausing && (newState.state != RoundState.stateRunning && newState.state != RoundState.stateFinished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState.state == RoundState.stateFinished && newState.state != RoundState.stateStarting)
        {
            MyLog("Invalid round state transition");
        }

        // Update the currentState so we can have it when calling e.g. the bomb's functions
        currentState = newState;

        // Actions to do AFTER updating the currentState
        if (newState.state == RoundState.stateStarting)
        {
            MyLog(newState.ToString());
            ResetTimers();
            bomb.GetComponent<BombController>().Arm();
        }
        else if (newState.state == RoundState.stateRunning)
        {
            MyLog(newState.ToString());
        }
        else if (newState.state == RoundState.statePaused)
        {
            MyLog(newState.ToString());
            PauseRound();
        }
        else if (newState.state == RoundState.stateUnPausing)
        {
            MyLog(newState.ToString());
            UnPauseRound();
        }
        else if (newState.state == RoundState.stateFinished)
        {
            MyLog(newState.ToString());
            bomb.GetComponent<BombController>().Explode();
        }
        else
        {
            MyLog("Unknown round state");
        }
    }

    void HandleRoundState()
    {
        if (currentState.state == RoundState.stateStarting)
        {
            SetState(new RoundState(RoundState.stateRunning));
        }
        else if (currentState.state == RoundState.stateRunning)
        {
            if (timeElapsed > maxRoundTime)
            {
                SetState(new RoundState(RoundState.stateFinished));
            }
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("GameManager-{0}", msg));
    }
}
