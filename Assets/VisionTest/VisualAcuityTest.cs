using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VisualAcuityTest : MonoBehaviour
{
    public string participant; // participant name, used for creating output file
    int[] stimulusDirection; // array for the different directions for each trial
    int[] response; // save the response of the participant for each trial
    float[] stimulusSize; // save the size of the stimulus for each trial
    float[] reactionTime; // save the reaction time of the participant for each trial
    int trial = 0; // current trial
    public int maxTrials; // stop measurement after maxTrials
    GameObject stimulus; // gameObject for accessing the stimulus size and direction
    bool measurementStarted = false; // indicates if measurement has started
    float startTime; // measure time at start of trial
    float endTime; // measure time after response
    void Start()
    {
        stimulus = GameObject.Find("Stimulus");
        // initialize the arrays
        stimulusDirection = new int[maxTrials];
        response = new int[maxTrials];
        stimulusSize = new float[maxTrials];
        reactionTime = new float[maxTrials];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) // Start of the measurement
        {
            StartMeasurement();
        }
        
        if (measurementStarted)
        {
            if(Input.GetKeyDown(KeyCode.Keypad6)) // right button of numpad is pressed
            {
                CheckResponse(0);
            }
            if(Input.GetKeyDown(KeyCode.Keypad9)) // key up arrow is pressed
            {
                CheckResponse(1);
            }
            if(Input.GetKeyDown(KeyCode.Keypad8)) // left arrow is pressed
            {
                CheckResponse(2);
            }
            if(Input.GetKeyDown(KeyCode.Keypad7)) // key down arrow is pressed
            {
                CheckResponse(3);
            }
            if(Input.GetKeyDown(KeyCode.Keypad4)) // key down arrow is pressed
            {
                CheckResponse(4);
            }
            if(Input.GetKeyDown(KeyCode.Keypad1)) // key down arrow is pressed
            {
                CheckResponse(5);
            }
            if(Input.GetKeyDown(KeyCode.Keypad2)) // key down arrow is pressed
            {
                CheckResponse(6);
            }
            if(Input.GetKeyDown(KeyCode.Keypad3)) // key down arrow is pressed
            {
                CheckResponse(7);
            }
        }
    }

    void StartMeasurement()
    {
        // do all the stuff that we need to do at the beginning of a new measurement
        Debug.Log("Measurement started");
        measurementStarted = true;
        trial = 0;
        ChangeStimulus(); // new random direction
        // save the initial size
        stimulusSize[trial] = stimulus.transform.localScale.x;
    }

    void ChangeStimulus()
    {
        int randomDirection = Random.Range(0,8);
        //stimulusDirection = randomDirection;
        stimulusDirection[trial] = randomDirection;
        stimulus.transform.rotation = Quaternion.Euler(0,0, randomDirection * 45);
        Debug.Log("Rotation is: " + randomDirection*45);
        startTime = Time.time; // save the current time for start of the trial
    }

    void CheckResponse(int currentResponse) // checks if the participant's response is correct or incorrect
    {
        endTime = Time.time; // save the current time for end of the trial
        reactionTime[trial] = endTime - startTime; // calculate the reaction time for the current trial
        Debug.Log("Reaction time: " + reactionTime[trial]); // show the current reaction time in console
        response[trial] = currentResponse; // save the current response to the response array
        if(currentResponse == stimulusDirection[trial]) // check if response fits to stimulusDirection
        {
            Debug.Log("Correct response!");
            stimulus.transform.localScale /= 1.25f;
        }
        else // incorrect response
        {
            Debug.Log("Incorrect response!");
            stimulus.transform.localScale *= 1.25f;
        }
        // trial is over -> next trial
        trial++; // increase trial counter
        if(trial == maxTrials) // we reached maxTrials -> stop measurement 
        {
            StopMeasurement();
        }
        else
        {
            stimulusSize[trial] = stimulus.transform.localScale.x; // save the current stimulus size
        }
        ChangeStimulus();
    }

    void StopMeasurement()
    {
        Debug.Log("Measurement is done. Final stimulus size: " + stimulus.transform.localScale.x); // print final stimulus size
        stimulus.transform.localScale = new Vector3(1,1,1); // reset stimulus size
        trial = 0; // reset trial counter to start new measurement
        measurementStarted = false;
        SaveData();
    }

    void SaveData()
    {
        StreamWriter sw = new StreamWriter("./measurements/" + participant + ".csv");
        sw.WriteLine("participant: " + participant); // save participant name
        sw.WriteLine("trial,stimulusDirection,response,stimulusSize,reactionTime"); // header line
        for(int i = 0; i < maxTrials; i++)
        {
            sw.WriteLine(i + ","  + stimulusDirection[i] + "," + response[i]
                           + "," + stimulusSize[i] + "," + reactionTime[i]);
        }
        sw.Close(); // close file and save changes
    }
}
