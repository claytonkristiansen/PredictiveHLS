using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSectionController : MonoBehaviour
{
    public GameObject EntrySensor, ExitSensor;
    private GameObject HighwayReference;
    public Light[] lights;
    private float ActiveBrightness, InactiveBrightness;
    public bool startOn;
    public float numberOfCars;
    private int EnterResetDelay, ExitResetDelay;

    // Use this for initialization
    void Start()
    {
        EnterResetDelay = 0;
        ExitResetDelay = 0;
        HighwayReference = GameObject.FindGameObjectWithTag("Road");
        int lightChildren = 0;
        for (int i = 0; i < GetComponent<Transform>().childCount; i++)
        {
            if (GetComponent<Transform>().GetChild(i).tag == "Lights")
            {
                lightChildren++;
            }
        }
        lights = new Light[lightChildren];
        for (int i = 0, k = 0; i < GetComponent<Transform>().childCount; i++, k++)
        {
            if (GetComponent<Transform>().GetChild(i).tag == "Lights")
            {
                lights[k] = GetComponent<Transform>().GetChild(i).GetChild(7).GetComponent<Light>();
            }
        }

        ActiveBrightness = GameObject.FindGameObjectWithTag("Road").GetComponent<HighwayController>().ActiveBrightness;
        InactiveBrightness = GameObject.FindGameObjectWithTag("Road").GetComponent<HighwayController>().InactiveBrightness;

        if (startOn)
        {
            numberOfCars = 1;
        }
        else
        {
            numberOfCars = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (EntrySensor.GetComponent<SensorPairController>().DataReady)
        {
            if (name == "LightSection (1)")
            { }
            bool check = true;
            if (EnterResetDelay == 1)
            {
                //EntrySensor.GetComponent<SensorPairController>().LReset();
                EnterResetDelay = 0;
                check = false;
            }
            else
            {
                EnterResetDelay = 1;
            }
            if (check)
            {
                numberOfCars++;
            }

        }
        if (ExitSensor.GetComponent<SensorPairController>().DataReady)
        {

            bool check = true;
            if (ExitResetDelay == 1)
            {
                //ExitSensor.GetComponent<SensorPairController>().LReset();
                ExitResetDelay = 0;
                check = false;
            }
            else
            {
                ExitResetDelay = 1;
            }
            if (numberOfCars > 0 && check)
            {
                numberOfCars--;
            }
        }


        if (HighwayReference.GetComponent<HighwayController>().systemProcedure == SYSTEM_PROCEDURE.SECTION_BASED || HighwayReference.GetComponent<HighwayController>().systemProcedure == SYSTEM_PROCEDURE.BOTH)
        {
            if (numberOfCars >= 1)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].transform.parent.gameObject.GetComponent<LightController>().CarInSection = true;
                    lights[i].intensity = ActiveBrightness;
                }
            }
            if (numberOfCars < 1)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].transform.parent.gameObject.GetComponent<LightController>().CarInSection = false;
                    lights[i].intensity = InactiveBrightness;
                }
            }
        }
    }
}
