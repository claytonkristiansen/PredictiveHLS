using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarSpawner : MonoBehaviour
{
    /// <summary>
    /// The speed of the cars when they are spawned in
    /// </summary>
    public float CarSpeed = 10;

    /// <summary>
    /// Car object to be spawned
    /// </summary>
    public GameObject CarPrefab;
    /// <summary>
    /// Transform used to determine where to spawn the car
    /// </summary>
    public Transform SpawnLocation;
    /// <summary>
    /// Road sections with sensors behind the car's spawning position
    /// </summary>
    private RoadSection[] BeginningRoadSections;
    /// <summary>
    /// The number of road sections in the area without sensors the cars can reach
    /// </summary>
    public int numberOfRoadSegmentsWithoutSensors;
    /// <summary>
    /// GameObject 
    /// </summary>
    public RoadController RoadControllerScript;

    /// <summary>
    /// Car spawn rate per 60 seconds
    /// </summary>
    public float MaxCarsPerMinute;

    public System.Random rnd;

    private float timeCounter = 0;
    public float minuteCounter = 0;

    private float minimumTimeBetweenCars = 1;

    public int numCarsSpawned = 0;
	// Use this for initialization
	void Start ()
    {
        rnd = new System.Random();
        BeginningRoadSections = new RoadSection[numberOfRoadSegmentsWithoutSensors];
        for (int i = 0; i < numberOfRoadSegmentsWithoutSensors; i++)
        {
            BeginningRoadSections[i] = RoadControllerScript.RoadSections[i].GetComponent<RoadSection>();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(minuteCounter >= 60)
        {
            numCarsSpawned = 0;
            minuteCounter = 0;
        }
        if (timeCounter >= minimumTimeBetweenCars)
        {
            timeCounter = 0;
            int randNum = rnd.Next(0, 100);
            float ratio = ((float)1 / (float)3) * (float)100;
            if (randNum <= ratio)
            {
                if (numCarsSpawned < MaxCarsPerMinute)
                {
                    numCarsSpawned++;
                    SpawnCar();
                    for (int i = 0; i < BeginningRoadSections.Length && (StaticVariables.systemProcedure == SYSTEM_PROCEDURE.SECTION_BASED || StaticVariables.systemProcedure == SYSTEM_PROCEDURE.BOTH); i++)
                    {
                        BeginningRoadSections[i].IncreaseCarsInSection();
                    }
                }
            }
        }
        minuteCounter += Time.deltaTime;
        timeCounter += Time.deltaTime;
    }

    public void SpawnCar()
    {
        Instantiate(CarPrefab, SpawnLocation.position, SpawnLocation.rotation).GetComponent<SelfDrivingCarScript>().m_Speed = CarSpeed;
    }
}
