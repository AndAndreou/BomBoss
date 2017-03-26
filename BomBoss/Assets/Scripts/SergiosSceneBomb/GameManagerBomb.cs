using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerBomb : MonoBehaviour {

    public GameObject bomb;
    private Vector3 offset;

    // UI
    public Text roundInfoText;
    // Times
    public int maxRoundTime = 120; // secs
    public float timeElapsed;

    public RoundState currentState = RoundState.finished; // Can be STARTING, RUNNING, PAUSED, FINISHED

    // Use this for initialization
    void Start () {
        offset = transform.position - bomb.transform.position;
        //currentState = RoundStateFinished;
        SetState(RoundState.starting);
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
        if (currentState == RoundState.running)
        {
            SetState(RoundState.paused);
        }
        else if (currentState == RoundState.paused)
        {
            SetState(RoundState.unPausing);
        }
    }

    void PauseRound()
    {
        bomb.GetComponent<BombController>().Pause();
    }

    void UnPauseRound()
    {
        bomb.GetComponent<BombController>().UnPause();
        SetState(RoundState.running);
    }

    void ResetTimers()
    {
        timeElapsed = 0;
    }

    void IncrementTimers(float deltaTime)
    {
        if (currentState == RoundState.running)
        {
            timeElapsed += deltaTime;
        }
    }

    void SetState(RoundState newState)
    {
        // State Transition Validation Logic
        if (currentState == RoundState.starting && newState != RoundState.running)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.running && (newState != RoundState.paused && newState != RoundState.finished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.paused && newState != RoundState.unPausing)
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.unPausing && (newState != RoundState.running && newState != RoundState.finished))
        {
            MyLog("Invalid round state transition");
        }

        if (currentState == RoundState.finished && newState != RoundState.starting)
        {
            MyLog("Invalid round state transition");
        }

        // Update the currentState so we can have it when calling e.g. the bomb's functions
        currentState = newState;

        // Actions to do AFTER updating the currentState
        if (newState == RoundState.starting)
        {
            MyLog(newState.ToString());
            ResetTimers();
            bomb.GetComponent<BombController>().Arm();
        }
        else if (newState == RoundState.running)
        {
            MyLog(newState.ToString());
        }
        else if (newState == RoundState.paused)
        {
            MyLog(newState.ToString());
            PauseRound();
        }
        else if (newState == RoundState.unPausing)
        {
            MyLog(newState.ToString());
            UnPauseRound();
        }
        else if (newState == RoundState.finished)
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
        if (currentState == RoundState.starting)
        {
            SetState(RoundState.running);
        }
        else if (currentState == RoundState.running)
        {
            if (timeElapsed > maxRoundTime)
            {
                SetState(RoundState.finished);
            }
        }
    }

    void MyLog(string msg)
    {
        Debug.Log(string.Format("GameManager-{0}", msg));
    }
}
