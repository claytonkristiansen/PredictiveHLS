using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightController : MonoBehaviour
{
    public bool CarInSection;
    public LIGHT_STATUS m_LightStatus;
    public float thing;
    public float m_ThisLightOn;
    public float m_ThisLightOff;

    // Use this for initialization
    void Start()
    {
        StaticVariables.LReset();
        m_ThisLightOn = 0;
        m_ThisLightOff = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!StaticVariables.isPaused)
        {
            thing = gameObject.transform.FindChild("Directional light").GetComponent<Light>().intensity;
            if (gameObject.transform.FindChild("Directional light").GetComponent<Light>().intensity ==
                GameObject.FindGameObjectWithTag("Road").GetComponent<HighwayController>().ActiveBrightness)
            {
                m_ThisLightOn = m_ThisLightOn + Time.deltaTime;
                StaticVariables.CommulativeLightsOnTime += Time.deltaTime;
            }
            else
            {
                m_ThisLightOff = m_ThisLightOn + Time.deltaTime;
                StaticVariables.CommulativeLightsOffTime += Time.deltaTime;
            }
            StaticVariables.TotalLightsTime += Time.deltaTime;
        }
        else if (StaticVariables.isPaused)
        {

        }
    }
    public bool GetCarInSection()
    {
        return CarInSection;
    }

    public void SetCarInSection(bool value)
    {
        CarInSection = value;
    }
}
