using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLampScript : MonoBehaviour
{
    public LIGHT_CONDITION thisLightsCondition;
    /// <summary>
    /// Sets light to intensity
    /// </summary>
    /// <param name="intensity">Intensity to set light to</param>
    /// <returns></returns>
    public void SetLightToIntensity(float intensity, LIGHT_CONDITION condition)
    {
        thisLightsCondition = condition;
        transform.FindChild("Directional light").GetComponent<Light>().intensity = intensity;
        transform.FindChild("Point light").GetComponent<Light>().intensity = intensity;
    }
    /// <summary>
    /// Sets the lights spot angle
    /// </summary>
    /// <param name="spotAngle"></param>
    public void SetLightSpotAngle(float spotAngle)
    {
        transform.FindChild("Directional light").GetComponent<Light>().spotAngle = spotAngle;
    }

    void FixedUpdate()
    {
        if(thisLightsCondition == LIGHT_CONDITION.ON)
        {
            StaticVariables.CommulativeLightsOnTime += Time.deltaTime;
        }
        else if(thisLightsCondition == LIGHT_CONDITION.OFF)
        {
            StaticVariables.CommulativeLightsOffTime += Time.deltaTime;
        }
    }
}
