using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// enum for the condition of lights in a RoadSection
/// </summary>
public enum LIGHT_CONDITION
{
    /// <summary>
    /// Lights are on
    /// </summary>
    ON = 0,
    /// <summary>
    /// Lights are off
    /// </summary>
    OFF = 1
}


public class RoadSection : MonoBehaviour
{
    /// <summary>
    /// Spot angle of the lights in the section
    /// </summary>
    public float LightSpotAngle;
    /// <summary>
    /// Sensor in the right lane of this RoadSection
    /// </summary>
    public GameObject RightLaneSensor;
    /// <summary>
    /// Sensor in the left lane of this RoadSection
    /// </summary>
    public GameObject LeftLaneSensor;
    /// <summary>
    /// Light condition of the RoadSection. Either ON or OFF
    /// </summary>
    public LIGHT_CONDITION LightCondition = LIGHT_CONDITION.OFF;
    /// <summary>
    /// Number of cars currently traveling through this RoadSection
    /// </summary>
    public int numCarsInSection = 0;
    /// <summary>
    /// Length of the Road Section
    /// </summary>
    public float RoadSectionLength;
    /// <summary>
    /// Collection containing all the lights in this section
    /// </summary>
    public Collection<StreetLampScript> SectionLights;
    /// <summary>
    /// Right lane entry sensor for the RoadSection
    /// </summary>
    public GameObject RightEntrySensor;
    /// <summary>
    /// Right lane exit sensor for the RoadSection
    /// </summary>
    public GameObject RightExitSensor;
    /// <summary>
    /// Left lane entry sensor for the RoadSection
    /// </summary>
    public GameObject LeftEntrySensor;
    /// <summary>
    /// Left lane exit sensor for the RoadSection
    /// </summary>
    public GameObject LeftExitSensor;

    // Use this for initialization
    /// <summary>
    /// Called before start
    /// </summary>
    void Awake()
    {
        //Initializing SectionLights//
        SectionLights = new Collection<StreetLampScript>();

        //Calculates length of the road section//
        RoadSectionLength = transform.FindChild("Ground").transform.localScale.x * 10;

        //Temporary array of Street Lamps Transforms//
        Transform[] streetLampArray = GetChildrenOfName(transform.FindChild("Lights").gameObject, "Street Lamp");

        //Adds the lights in the section to the SectionLights Collection//
        for (int i = 0; i < streetLampArray.Length; i++)
        {
            SectionLights.Add(streetLampArray[i].gameObject.GetComponent<StreetLampScript>());

            if (LightCondition == LIGHT_CONDITION.ON)
            {
                SectionLights[i].SetLightToIntensity(RoadController.LightsOnIntensity, LIGHT_CONDITION.ON);
            }
            else if (LightCondition == LIGHT_CONDITION.OFF)
            {
                SectionLights[i].SetLightToIntensity(RoadController.LightsOffIntensity, LIGHT_CONDITION.OFF);
            }

            SectionLights[i].SetLightSpotAngle(LightSpotAngle);
        }
    }

    /// <summary>
    /// Calculated before every physics update
    /// </summary>
    private void FixedUpdate()
    {
        //GameObject.Find("Computer").GetComponent<Computer>().FixedUpdate();
        //GameObject.Find("Computer").GetComponent<Computer>().TellCalledThisFrame();
        if (StaticVariables.systemProcedure == SYSTEM_PROCEDURE.SECTION_BASED || StaticVariables.systemProcedure == SYSTEM_PROCEDURE.BOTH)
        {
            //Checking the Right sensors for cars going in and out of the section
            if (RightEntrySensor.GetComponent<SensorPairController>().DataReady || LeftEntrySensor.GetComponent<SensorPairController>().DataReady)
            {
                IncreaseCarsInSection();
            }
            if (RightExitSensor.GetComponent<SensorPairController>().DataReady || LeftExitSensor.GetComponent<SensorPairController>().DataReady)
            {
                DecreaseCarsInSection();
            }
            if (numCarsInSection >= 1)
            {
                SetLightCondition(LIGHT_CONDITION.ON);
            }
            else
            {
                SetLightCondition(LIGHT_CONDITION.OFF);
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------//
    //Methods//
    //------------------------------------------------------------------------------------------------------------------------------------------//

    public float GetLengthOfRoadSection()
    {
        return transform.FindChild("Ground").transform.localScale.x * 10;
    }

    public void IncreaseCarsInSection()
    {
        numCarsInSection++;
        if(LightCondition == LIGHT_CONDITION.OFF)
        {
            SetLightCondition(LIGHT_CONDITION.ON);
        }
    }

    /// <summary>
    /// Decreases the number of cars in the section
    /// </summary>
    public void DecreaseCarsInSection()
    {
        if (numCarsInSection >= 1)
        {
            numCarsInSection--;
            if (numCarsInSection == 0 && LightCondition == LIGHT_CONDITION.ON)
            {
                SetLightCondition(LIGHT_CONDITION.OFF);
            }
        }
    }

    /// <summary>
    /// Sets the number of cars in the section
    /// </summary>
    /// <param name="number">Number of cars in section</param>
    public void SetCarsInSection(int number)
    {
        numCarsInSection = number;
        if (LightCondition == LIGHT_CONDITION.OFF)
        {
            SetLightCondition(LIGHT_CONDITION.ON);
        }
    }

    /// <summary>
    /// Returns an array of all children with name "name" from a parent GameObject
    /// </summary>
    /// <param name="parent">The parent GameObject</param>
    /// <param name="name">The name of the children being seeked</param>
    /// <returns>Array of child GameObjects</returns>
    Transform[] GetChildrenOfName(GameObject parent, string name)
    {
        int childCount = parent.transform.childCount;
        Transform[] childrenArray = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            childrenArray[i] = parent.transform.GetChild(i);
        }
        return childrenArray;
    }
	
    /// <summary>
    /// Sets the LIGHT_CONDITION of the RoadSection
    /// </summary>
    /// <param name="lightCondition">The LIGHT_CONDITION of the RoadSection</param>
    /// <returns></returns>
    public bool SetLightCondition(LIGHT_CONDITION lightCondition)
    {
        LightCondition = lightCondition;
        if (lightCondition == LIGHT_CONDITION.ON)
        {
            for (int i = 0; i < SectionLights.Count; i++)
            {
                SectionLights[i].SetLightToIntensity(RoadController.LightsOnIntensity, LIGHT_CONDITION.ON);
            }
            return true;
        }
        if (lightCondition == LIGHT_CONDITION.OFF)
        {
            for (int i = 0; i < SectionLights.Count; i++)
            {
                SectionLights[i].SetLightToIntensity(RoadController.LightsOffIntensity, LIGHT_CONDITION.OFF);
            }
            return true;
        }
        else return false;
    }
}
