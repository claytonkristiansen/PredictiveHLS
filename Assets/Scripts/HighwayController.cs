using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//https://unity3d.com/learn/tutorials/topics/physics/raycasting
//https://docs.unity3d.com/Manual/Layers.html




public class HighwayController : MonoBehaviour
{
    public float LightOnTime;
    InputField MaxCarsInput;
    InputField carLightingRadiusInput;
    public string MaxCarsInputString;
    public Vector3 ProjectedPos;
    //-------------------------------------------------------------------------------------------------------------------------//
    public float ActiveBrightness;
    //-------------------------------------------------------------------------------------------------------------------------//
    public float InactiveBrightness;
    //-------------------------------------------------------------------------------------------------------------------------//
    public float carLightingRadius;
    //-------------------------------------------------------------------------------------------------------------------------//
    public GameObject[] RightLaneSensors, LeftLaneSensors;
    //-------------------------------------------------------------------------------------------------------------------------//
    public Vector3 ActualCarPos;
    public Transform m_CarTransform;
    //-------------------------------------------------------------------------------------------------------------------------//
    public int MaxCars;
    //-------------------------------------------------------------------------------------------------------------------------//
    private CarData[] cars;
    //-------------------------------------------------------------------------------------------------------------------------//
    private Light[] lightsArray;
    //-------------------------------------------------------------------------------------------------------------------------//
    private int lightsArraySize;
    //-------------------------------------------------------------------------------------------------------------------------//
    //Number of cars in the system
    public int numberOfCars;
    //-------------------------------------------------------------------------------------------------------------------------//
    public SYSTEM_PROCEDURE systemProcedure;


    // Use this for initialization
    public void Start()
    {
        StaticVariables.WasUnpaused = false;
        StaticVariables.isPaused = false;
        //---------------------------------------------------------------------------------------------------------------------//
        numberOfCars = 0;
        //---------------------------------------------------------------------------------------------------------------------//
        cars = new CarData[MaxCars];
        //---------------------------------------------------------------------------------------------------------------------//
        MaxCarsInput = GameObject.FindGameObjectWithTag("MaxCarsInput").GetComponent<InputField>();
        MaxCarsInput.text = MaxCars.ToString();
        carLightingRadiusInput = GameObject.FindGameObjectWithTag("IlluminationRadiusInput").GetComponent<InputField>();
        carLightingRadiusInput.text = carLightingRadius.ToString();
        //---------------------------------------------------------------------------------------------------------------------//
        //Sets up the Light array
        GameObject[] GameObjects = GameObject.FindGameObjectsWithTag("DirectionalLights");
        lightsArray = new Light[GameObjects.Length];
        for (int i = 0; i <= GameObjects.Length - 1; i++)
        {
            lightsArray[i] = GameObjects[i].GetComponent<Light>();
        }
        //Sets the lightsArraySize to the length of lightsArray
        lightsArraySize = lightsArray.Length;
        //---------------------------------------------------------------------------------------------------------------------//
        LeftLaneSensors = GameObject.FindGameObjectsWithTag("LeftLaneSensors");
        //---------------------------------------------------------------------------------------------------------------------//
        RightLaneSensors = GameObject.FindGameObjectsWithTag("RightLaneSensors");
        //---------------------------------------------------------------------------------------------------------------------//
        for (int k = 0; k <= MaxCars - 1; k++)
        {
            cars[k].OnRoad = false;
        }
        //for (int i = 0; i < lightsArray.Length; i++)


        //{
        //    lightsArray[i].intensity = InactiveBrightness;
        //}
    }
    //-------------------------------------------------------------------------------------------------------------------------//
    public float DistanceBetween(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt(Mathf.Abs(Mathf.Pow(pos1.x - pos2.x, 2.0f) + Mathf.Pow(pos1.y - pos2.y, 2.0f) + Mathf.Pow(pos1.z - pos2.z, 2.0f)));
    }
    //-------------------------------------------------------------------------------------------------------------------------//
    public void UpdateMaxCarsValue()
    {
        MaxCars = 0;
        MaxCars = (int)float.Parse(MaxCarsInput.text);
        cars = new CarData[MaxCars];
    }
    //-------------------------------------------------------------------------------------------------------------------------//
    public void UpdatecarLightingRadiusValue()
    {
        if (carLightingRadiusInput.text == "45.894")
        { }
        carLightingRadius = 0;
        carLightingRadius = float.Parse(carLightingRadiusInput.text);
    }
    //-------------------------------------------------------------------------------------------------------------------------//
    private Light[] FindLightsWithinCoodAlongXAxis(Vector3 pos1, Vector3 pos2)
    {
        Light[] tempLightArray = new Light[lightsArray.Length];
        int k = 0;
        for (int i = 0; i <= lightsArraySize - 1; i++)
        {
            if (pos1.x > pos2.x)
            {
                if (lightsArray[i].transform.position.x >= pos1.x && lightsArray[i].transform.position.x <= pos2.x)
                {
                    tempLightArray[k] = lightsArray[i];
                }
            }
            else if (pos2.x > pos1.x)
            {
                if (lightsArray[i].transform.position.x >= pos2.x && lightsArray[i].transform.position.x <= pos1.x)
                {
                    tempLightArray[k] = lightsArray[i];
                }
            }
            else if (pos2.x == pos1.x)
            {
                return null;
            }
        }
        return tempLightArray;
    }
    //-------------------------------------------------------------------------------------------------------------------------//
    // Update is called once per frame
    void FixedUpdate()
    {
        if (StaticVariables.WasUnpaused)
        { }
        if (!StaticVariables.isPaused)
        {
            //Turns off lights if car not near
            for (int i = 0; i < lightsArray.Length; i++)
            {
                if (!lightsArray[i].transform.parent.gameObject.GetComponent<LightController>().CarInSection)
                {
                    lightsArray[i].intensity = InactiveBrightness;
                }
            }

            for (int i = 0, f = 0; (i <= RightLaneSensors.Length - 1 || f <= LeftLaneSensors.Length - 1) && numberOfCars <= MaxCars - 2; i++, f++)
            {
                if (f >= LeftLaneSensors.Length - 1)
                {
                    f = LeftLaneSensors.Length - 1;
                }
                if (RightLaneSensors[i].GetComponent<SensorPairController>().DataReady)
                {
                    bool checkOn = true;
                    if (RightLaneSensors[i].GetComponent<SensorPairController>().indexOrder == 0)
                        for (int k = 0; k <= MaxCars - 1 && checkOn; k++)
                        {
                            if (cars[k].OnRoad == false)
                            {
                                checkOn = false;
                                numberOfCars++;
                                cars[k].m_Direction = RightLaneSensors[i].GetComponent<SensorPairController>().PredictionData.direction;
                                cars[k].m_Speed = RightLaneSensors[i].GetComponent<SensorPairController>().PredictionData.Speed();
                                cars[k].m_ProjectedPosition = RightLaneSensors[i].GetComponent<SensorPairController>().PredictionData.m_ExitPos;
                                cars[k].OnRoad = true;
                            }
                            if (systemProcedure == SYSTEM_PROCEDURE.PREDICTION_BASED)
                            {
                                RightLaneSensors[i].GetComponent<SensorPairController>().LReset();
                            }
                        }
                }
            }

            //---------------------------------------------------------------------------------------------------------------------//
            for (int i = 0; i <= MaxCars - 1; i++)
            {
                if (cars[i].OnRoad)
                {
                    cars[i].m_ProjectedPosition.Set(cars[i].m_ProjectedPosition.x + cars[i].m_Speed * Time.deltaTime, cars[i].m_ProjectedPosition.y, cars[i].m_ProjectedPosition.z);
                }
            }
            //---------------------------------------------------------------------------------------------------------------------//
            for (int i = 0; i <= MaxCars - 1; i++)
            {
                if (((cars[i].m_ProjectedPosition.x > 1000 || cars[i].m_ProjectedPosition.x < 0)) && cars[i].OnRoad && numberOfCars != 1)
                {
                    numberOfCars--;
                    cars[i].OnRoad = false;
                }
            }
            //---------------------------------------------------------------------------------------------------------------------//
            //Checks each light and if cars projected pos is outside of carLightingRadius
            for (int w = 0; w <= lightsArraySize - 1 && numberOfCars >= 1; w++)
            {
                for (int p = 0; p <= MaxCars - 1; p++)
                {
                    if (cars[p].OnRoad)
                    {
                        if (DistanceBetween(lightsArray[w].transform.position, cars[p].m_ProjectedPosition) <= carLightingRadius)
                        {
                            if (systemProcedure == SYSTEM_PROCEDURE.PREDICTION_BASED || systemProcedure == SYSTEM_PROCEDURE.BOTH)
                            {
                                lightsArray[w].intensity = ActiveBrightness;
                            }
                        }
                    }
                }
            }
            //---------------------------------------------------------------------------------------------------------------------//
        }
        else if (StaticVariables.isPaused)
        {

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        //if (StaticVariables.isPaused)
        //{
        //    StaticVariables.isPaused = false;
        //    PauseCanvas.enabled = false;
        //}
        //else
        //{
        //    StaticVariables.isPaused = true;
        //    PauseCanvas.enabled = true;
        //}

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
    //-------------------------------------------------------------------------------------------------------------------------//
}
