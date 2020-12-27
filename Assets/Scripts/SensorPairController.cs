  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION { NORTH = 0, SOUTH = 1};

public struct SensorCondition
{
    public Vector3 m_Pos;
}

public struct PredictionInfo
{
    /// <summary>
    /// Direction the car is going
    /// </summary>
    public DIRECTION direction;
    public Vector3 m_EnterPos;
    public Vector3 m_ExitPos;
    public float m_TimeBetween;
    public float xDistanceBetweenSensors()
    {
        if (m_ExitPos.x - m_EnterPos.x < 0)
        {
            direction = DIRECTION.SOUTH;
        }
        else if (m_EnterPos.x - m_EnterPos.x >= 0)
        {
            direction = DIRECTION.NORTH;
        }
        return m_ExitPos.x - m_EnterPos.x;
    }
    public float Speed()
    {
        return xDistanceBetweenSensors() / m_TimeBetween;
    }
}

public class SensorPairController : MonoBehaviour
{
    public int indexOrder;
    public GameObject BackSensor;
    public GameObject FrontSensor;
    public Vector3 m_EnterPos;
    public Vector3 m_ExitPos;
    public float xDistanceBetweenSensors;
    public float TimeInfo;
    public float Speed;
    public DIRECTION direction;
    public bool DataReady = false;
    public PredictionInfo PredictionData;
    private bool CountTime;
    public GameObject MostRecentCar;
    /// <summary>
    /// Time after wich to reset the sensors
    /// </summary>
    private float timeToReset;
    /// <summary>
    /// Time left to reset
    /// </summary>
    private float timer;
    /// <summary>
    /// Was the sensor triggered last frame?
    /// </summary>
    public bool triggeredThisFrame = false;

    // Use this for initialization
    void Start ()
    {
        timer = timeToReset;
        CountTime = false;
        PredictionData.m_TimeBetween = 0;
        BackSensor.GetComponent<SensorController>().lookForTrigger = true;
        FrontSensor.GetComponent<SensorController>().lookForTrigger = false;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!StaticVariables.isPaused)
        {
            if (triggeredThisFrame)
            {
                LReset();
                triggeredThisFrame = false;
            }
            if (CountTime)
            {
                PredictionData.m_TimeBetween = PredictionData.m_TimeBetween + Time.deltaTime;
            }
            if (BackSensor.GetComponent<SensorController>().isTriggered)
            {
                PredictionData.m_EnterPos = BackSensor.GetComponent<SensorController>().GetCollisionInfo().transform.position;
                BackSensor.GetComponent<SensorController>().lookForTrigger = false;
                FrontSensor.GetComponent<SensorController>().lookForTrigger = true;
                CountTime = true;
                BackSensor.GetComponent<SensorController>().isTriggered = false;
            }
            if (FrontSensor.GetComponent<SensorController>().isTriggered)
            {
                PredictionData.m_ExitPos = FrontSensor.GetComponent<SensorController>().GetCollisionInfo().transform.position;
                DataReady = true;
                triggeredThisFrame = true;
                CountTime = false;
                MostRecentCar = FrontSensor.GetComponent<SensorController>().GetCollisionInfo();
                BackSensor.GetComponent<SensorController>().lookForTrigger = false;
                FrontSensor.GetComponent<SensorController>().lookForTrigger = false;
                FrontSensor.GetComponent<SensorController>().isTriggered = false;
            }
            m_EnterPos = PredictionData.m_EnterPos;
            m_ExitPos = PredictionData.m_ExitPos;
            xDistanceBetweenSensors = PredictionData.xDistanceBetweenSensors();
            TimeInfo = PredictionData.m_TimeBetween;
            Speed = PredictionData.Speed();
            direction = PredictionData.direction;
        }
        else if (StaticVariables.isPaused)
        {

        }
    }

    public void LReset()
    {
        DataReady = false;
        PredictionData = new PredictionInfo();
        CountTime = false;
        BackSensor.GetComponent<SensorController>().lookForTrigger = true;
        FrontSensor.GetComponent<SensorController>().lookForTrigger = false;
    }
}
