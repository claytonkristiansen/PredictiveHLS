using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Procedure the system will use
/// </summary>
public enum SYSTEM_PROCEDURE
{
    /// <summary>
    /// Uses cars entering and exiting sections to activate lights
    /// </summary>
    SECTION_BASED = 0,
    /// <summary>
    /// Uses a computer to predict and adapt to where the car will be
    /// </summary>
    PREDICTION_BASED,
    /// <summary>
    /// Uses both systems at the same time
    /// </summary>
    BOTH
};

/// <summary>
/// All static members of this class are attached to the class itself and are effectively "global"
/// </summary>
[System.Serializable]
public static class StaticVariables
{
    /// <summary>
    /// 
    /// </summary>
    public static bool isPaused;
    public static bool paused = false;
    public static float CommulativeLightsOnTime;
    public static float CommulativeLightsOffTime;
    public static float TotalLightsTime;
    public static bool WasUnpaused;
    public static SYSTEM_PROCEDURE systemProcedure;
    public static float totalTimeRunning = 0;

    public static void SetSystemProcedure(SYSTEM_PROCEDURE procedure)
    {
        systemProcedure = procedure;
    }
    public static float GetOnToOffRatio()
    {
        return CommulativeLightsOnTime / CommulativeLightsOffTime;
    }
    public static void LReset()
    {
        CommulativeLightsOffTime = 0;
        CommulativeLightsOnTime = 0;
    }
}


public class RoadController : MonoBehaviour
{
    /// <summary>
    /// Number of cars on the road
    /// </summary>
    public int numberOfCarsOnRoad = 0;
    /// <summary>
    /// Intensity of the lights when they are "off"
    /// </summary>
    public static float LightsOffIntensity;
    public float lightsOffIntensity;
    /// <summary>
    /// Intensity of the lights when they are "on"
    /// </summary>
    public static float LightsOnIntensity;
    public float lightsOnIntensity;
    /// <summary>
    /// Number of road sections made
    /// </summary>
    public int numberOfRoadSections;
    /// <summary>
    /// Collection that holds all of the road sections
    /// </summary>
    public Collection<GameObject> RoadSections;
    /// <summary>
    /// GameObject used as a RoadSection
    /// </summary>
    public GameObject RoadSectionObject;
    /// <summary>
    /// GameObject used as the last RoadSection
    /// </summary>
    public GameObject RoadSectionEndObject;
    /// <summary>
    /// GameObject used as the first RoadSection
    /// </summary>
    public GameObject RoadSectionStartObject;
    /// <summary>
    /// Length of the road sections in Unity Units
    /// </summary>
    public float RoadSectionLength;
    /// <summary>
    /// First right lane sensor on the road
    /// </summary>
    public GameObject FirstRightSensorOnRoad;
    /// <summary>
    /// Last right lane sensor on the road
    /// </summary>
    public GameObject LastRightSensorOnRoad;
    /// <summary>
    /// First left lane sensor on the road
    /// </summary>
    public GameObject FirstLeftSensorOnRoad;
    /// <summary>
    /// Last left lane sensor on the road
    /// </summary>
    public GameObject LastLeftSensorOnRoad;
    // Use this for initialization
    void Awake ()
    {
        //Sets road section length//
        RoadSectionLength = RoadSectionObject.GetComponent<RoadSection>().GetLengthOfRoadSection();

        //Sets the On and Off intensities//
        RoadController.LightsOffIntensity = lightsOffIntensity;
        RoadController.LightsOnIntensity = lightsOnIntensity;

        //Initializing RoadSections//
        RoadSections = new Collection<GameObject>();

        //Temporary Vector3 used to set positions of the road sections//
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        //Temporary Quaternion used to set positions of the road sections//
        Quaternion spawnRotation = new Quaternion(0, 0, 0, 0);

        for (int i = 0; i < numberOfRoadSections; i++)
        {
            if(i == 0)
            {
                RoadSections.Add(Instantiate(RoadSectionStartObject, spawnPosition, spawnRotation, transform));
                spawnPosition.Set(spawnPosition.x + RoadSectionLength, spawnPosition.y, spawnPosition.z);
            }
            else if (i == numberOfRoadSections - 1)
            {
                RoadSections.Add(Instantiate(RoadSectionEndObject, spawnPosition, spawnRotation, transform));
                spawnPosition.Set(spawnPosition.x + RoadSectionLength, spawnPosition.y, spawnPosition.z);
            }
            else
            {
                RoadSections.Add(Instantiate(RoadSectionObject, spawnPosition, spawnRotation, transform));
                spawnPosition.Set(spawnPosition.x + RoadSectionLength, spawnPosition.y, spawnPosition.z);
            }
        }
        for (int i = 0; i < RoadSections.Count; i++)
        {
            if (i == 0 || i == 1)
            {
                //If the active scene is ManualCarScene then make the first few sections start off with one car because the entry sensors are behind the car as it is spawned
                if (SceneManager.GetActiveScene().name == "ManualCarScene")
                {
                    RoadSections[i].GetComponent<RoadSection>().SetCarsInSection(1);
                }
                if (i == 0)
                {
                    RoadSections[i].GetComponent<RoadSection>().RightEntrySensor = GameObject.Find("Decoy");
                    RoadSections[i].GetComponent<RoadSection>().RightExitSensor = RoadSections[i + 1].GetComponent<RoadSection>().RightLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().LeftEntrySensor = RoadSections[i + 2].GetComponent<RoadSection>().LeftLaneSensor;
                    LastLeftSensorOnRoad = RoadSections[i].GetComponent<RoadSection>().LeftExitSensor;
                }
                else
                {
                    RoadSections[i].GetComponent<RoadSection>().RightEntrySensor = GameObject.Find("Decoy");
                    RoadSections[i].GetComponent<RoadSection>().RightExitSensor = RoadSections[i + 1].GetComponent<RoadSection>().RightLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().LeftEntrySensor = RoadSections[i + 2].GetComponent<RoadSection>().LeftLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().LeftExitSensor = RoadSections[i - 1].GetComponent<RoadSection>().LeftLaneSensor;
                }
                if (i == 1)
                {
                    FirstRightSensorOnRoad = RoadSections[i].GetComponent<RoadSection>().RightLaneSensor;
                }
            }
            else if (i == RoadSections.Count - 1 || i == RoadSections.Count - 2)
            {
                if (i == RoadSections.Count - 1)
                {
                    
                    RoadSections[i].GetComponent<RoadSection>().RightEntrySensor = RoadSections[i - 2].GetComponent<RoadSection>().RightLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().LeftEntrySensor = GameObject.Find("Decoy");
                    RoadSections[i].GetComponent<RoadSection>().LeftExitSensor = RoadSections[i - 1].GetComponent<RoadSection>().LeftLaneSensor;
                    LastRightSensorOnRoad = RoadSections[i].GetComponent<RoadSection>().RightExitSensor;

                }
                else
                {
                    RoadSections[i].GetComponent<RoadSection>().LeftEntrySensor = GameObject.Find("Decoy");
                    RoadSections[i].GetComponent<RoadSection>().RightExitSensor = RoadSections[i + 1].GetComponent<RoadSection>().RightLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().RightEntrySensor = RoadSections[i - 2].GetComponent<RoadSection>().RightLaneSensor;
                    RoadSections[i].GetComponent<RoadSection>().LeftExitSensor = RoadSections[i - 1].GetComponent<RoadSection>().LeftLaneSensor;
                    FirstLeftSensorOnRoad = RoadSections[i].GetComponent<RoadSection>().LeftLaneSensor;
                }
            }
            else
            {
                RoadSections[i].GetComponent<RoadSection>().RightEntrySensor = RoadSections[i - 2].GetComponent<RoadSection>().RightLaneSensor;
                RoadSections[i].GetComponent<RoadSection>().RightExitSensor = RoadSections[i + 1].GetComponent<RoadSection>().RightLaneSensor;
                RoadSections[i].GetComponent<RoadSection>().LeftEntrySensor = RoadSections[i + 2].GetComponent<RoadSection>().LeftLaneSensor;
                RoadSections[i].GetComponent<RoadSection>().LeftExitSensor = RoadSections[i - 1].GetComponent<RoadSection>().LeftLaneSensor;
                if (i == 2)
                {
                    //If the active scene is ManualCarScene then make the first few sections start off with one car because the entry sensors are behind the car as it is spawned
                    if (SceneManager.GetActiveScene().name == "ManualCarScene")
                    {
                        RoadSections[i].GetComponent<RoadSection>().SetCarsInSection(1);
                    }
                }
            }
        }
	}

    private void FixedUpdate()
    {
        if(FirstRightSensorOnRoad.GetComponent<SensorPairController>().DataReady)
        {
            numberOfCarsOnRoad++;
        }
        if (LastRightSensorOnRoad.GetComponent<SensorPairController>().DataReady)
        {
            if (numberOfCarsOnRoad >= 1)
            {
                numberOfCarsOnRoad--;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StatsScene");
        }
        StaticVariables.totalTimeRunning += Time.deltaTime;
    }

    public void TogglePause()
    {
        if (!StaticVariables.paused)
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
            Time.timeScale = 0.0f;
            StaticVariables.paused = true;
        }
        else if (StaticVariables.paused)
        {
            SceneManager.UnloadSceneAsync("UI");
            Time.timeScale = 1.0f;
            StaticVariables.paused = false;
        }
    }
}
