using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectTeamUIController : MonoBehaviour {

    [Header("Animators")]
    public Animator[] controllers = new Animator[4];

    [Header("UIComponent")]
    public Text[] numberOfControllers = new Text[4];

    [Header("Colors")]
    public Color readyColor;
    public Color unReadyColor;

    private int[] teamOfEachController = new int[4];
    private bool[] playerReady = new bool[4];

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ReadInputs();
        CheckIfAllReadyOrGoBack();
    }

    private void ReadInputs()
    {
        for(int i = 0; i < teamOfEachController.Length; i++)
        {

            //ready button
            bool readyInput = Input.GetButton("ShootPlayer" + (i + 1).ToString());
            if((readyInput == true) && (teamOfEachController[i] != 0))
            {
                playerReady[i] = true;
                
            }

            //back button
            bool backInput = Input.GetButton("MagnetPlayer" + (i + 1).ToString());
            if (backInput == true)
            {
                //if all player is unready and some one press back button, then go to previous sscene
                if(!playerReady[0] && !playerReady[0] && !playerReady[0] && !playerReady[0] && backInput)
                {
                    GoPrevScene();
                }
                playerReady[i] = false;
            }

            if (playerReady[i] == true)
            {
                continue;
            }

            float horizontalInput = Input.GetAxis("HorizontalPlayer" + (i+1).ToString());
            if (horizontalInput != 0)
            {
                if (horizontalInput > 0)
                {
                    teamOfEachController[i] = Mathf.Min(teamOfEachController[i] + 1, 1);
                }
                else
                {
                    teamOfEachController[i] = Mathf.Max(teamOfEachController[i] - 1, -1);
                }
            }
        }
        SetUIControllersAnimator();
        Input.ResetInputAxes();
    }

    private void SetUIControllersAnimator()
    {
        for (int i = 0; i < teamOfEachController.Length; i++)
        {
            controllers[i].SetInteger("Team", teamOfEachController[i]);
            if (playerReady[i] == true)
            {
                numberOfControllers[i].color = readyColor;
            }
            else
            {
                numberOfControllers[i].color = unReadyColor;
            }
        }
    }

    private void CheckIfAllReadyOrGoBack()
    {
        bool isAllReady = true;
        bool isAllUnReady = true;
        int teamsCounter = 0;
        for (int i = 0; i < playerReady.Length; i++)
        {
            isAllReady = isAllReady && playerReady[i];
            isAllUnReady = isAllUnReady && !playerReady[i];
            teamsCounter += teamOfEachController[i];
        }

        if(isAllReady == true)
        {
            if(teamsCounter == 0) //means teams is balance
            {
                GoNextScene();
            }
            return;
        }

    }

    private void GoNextScene()
    {
        SceneManager.LoadScene("newMergeScene3");
    }

    private void GoPrevScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
