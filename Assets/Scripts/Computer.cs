using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
/// <summary>
/// Structure used to represent a virtual car
/// </summary>
public struct CarData
{
    public bool OnRoad;
    public float m_Speed;
    public DIRECTION m_Direction;
    public Vector3 m_ProjectedPosition;
}

public class VirtualCarObject
{
    public CarData Data;
    /// <summary>
    /// Defualt constructor for VirtualCarObject
    /// </summary>
    public VirtualCarObject()
    {
    }
    /// <summary>
    /// Constructor for VirtualCarObject
    /// </summary>
    /// <param name="pos">Current position</param>
    /// <param name="data">CarData reference</param>
    public VirtualCarObject(Vector3 pos, CarData data)
    {
        Data = data;
        Data.m_ProjectedPosition = pos;
    }
    /// <summary>
    /// Constructor for VirtualCarObject
    /// </summary>
    /// <param name="pos">Current position</param>
    /// <param name="Speed">Current speed</param>
    /// <param name="direction">Current direction</param>
    public VirtualCarObject(Vector3 pos, float Speed, DIRECTION direction)
    {
        Data.m_ProjectedPosition = pos;
        Data.m_Speed = Speed;
        Data.m_Direction = direction;
    }
}

public class Computer : MonoBehaviour
{
    public Vector3 ajsfdga = new Vector3(0, 0, 0);
    public bool bob = false;
    public int numCarsOnRoad;
    public RoadController roadControllerScript;
    private Collection<VirtualCarObject> virtualCarCollection;
    private bool calledAlreadyThisFrame;
    public void TellCalledThisFrame()
    {
        calledAlreadyThisFrame = true;
    }
	// Use this for initialization
	void Start ()
    {
        virtualCarCollection = new Collection<VirtualCarObject>();
        if (roadControllerScript == null)
        {
            roadControllerScript = GameObject.Find("Road(Generatable)").GetComponent<RoadController>();
        }
	}
	
	public void FixedUpdate ()
    {
        if (calledAlreadyThisFrame == false)
        {
            //calledAlreadyThisFrame = true;
            if (StaticVariables.systemProcedure == SYSTEM_PROCEDURE.PREDICTION_BASED || StaticVariables.systemProcedure == SYSTEM_PROCEDURE.BOTH)
            {
                numCarsOnRoad = roadControllerScript.numberOfCarsOnRoad;
                if (roadControllerScript.FirstRightSensorOnRoad.GetComponent<SensorPairController>().DataReady)
                {
                    if(virtualCarCollection.Count == 1)
                    { }
                    SensorPairController tempScriptReference = roadControllerScript.FirstRightSensorOnRoad.GetComponent<SensorPairController>();
                    virtualCarCollection.Add(new VirtualCarObject(tempScriptReference.PredictionData.m_ExitPos, tempScriptReference.PredictionData.Speed(), tempScriptReference.PredictionData.direction));
                }
                else if (roadControllerScript.FirstLeftSensorOnRoad.GetComponent<SensorPairController>().DataReady)
                {
                    SensorPairController tempScriptReference = roadControllerScript.FirstLeftSensorOnRoad.GetComponent<SensorPairController>();
                    virtualCarCollection.Add(new VirtualCarObject(tempScriptReference.PredictionData.m_ExitPos, tempScriptReference.PredictionData.Speed(), tempScriptReference.PredictionData.direction));
                }
                RoadSectionChecks();
                MoveVirtualCars();
            }
        }
    }
    private void LateUpdate()
    {
        if(calledAlreadyThisFrame == true)
        {
            calledAlreadyThisFrame = false;
        }
    }

    /// <summary>
    /// Checks each road section and activates it if car is close by
    /// </summary>
    public void RoadSectionChecks()
    {
        for (int i = 0; i < roadControllerScript.RoadSections.Count; i++)
        {
            for(int k = 0; k < virtualCarCollection.Count; k++)
            {
                if (DistanceBetween(roadControllerScript.RoadSections[i].transform.position, virtualCarCollection[k].Data.m_ProjectedPosition) > 200)
                {
                    roadControllerScript.RoadSections[i].GetComponent<RoadSection>().SetLightCondition(LIGHT_CONDITION.OFF);
                }
            }
        }
        for (int i = 0; i < roadControllerScript.RoadSections.Count; i++)
        {
            for (int k = 0; k < virtualCarCollection.Count; k++)
            {
                if (DistanceBetween(roadControllerScript.RoadSections[i].transform.position, virtualCarCollection[k].Data.m_ProjectedPosition) <= 200)
                {
                    roadControllerScript.RoadSections[i].GetComponent<RoadSection>().SetLightCondition(LIGHT_CONDITION.ON);
                }
            }
        }
    }

    public void MoveVirtualCars()
    {
        for (int k = 0; k < virtualCarCollection.Count; k++)
        {
            virtualCarCollection[k].Data.m_ProjectedPosition.Set(virtualCarCollection[k].Data.m_ProjectedPosition.x + virtualCarCollection[k].Data.m_Speed * Time.deltaTime, virtualCarCollection[k].Data.m_ProjectedPosition.y, virtualCarCollection[k].Data.m_ProjectedPosition.z);
        }
    }

    /// <summary>
    /// Finds distance between two Vector3s
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public float DistanceBetween(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt(Mathf.Abs(Mathf.Pow(pos1.x - pos2.x, 2.0f) + Mathf.Pow(pos1.y - pos2.y, 2.0f) + Mathf.Pow(pos1.z - pos2.z, 2.0f)));
    }
}
