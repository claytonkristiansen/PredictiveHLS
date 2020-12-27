using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LIGHT_STATUS { ON = 0, OFF = 1};

public class StreetLampController : MonoBehaviour
{
    private Light[] Lights;
    private GameObject[] GameObjects;
    int m_NumberOfLights;

    // Use this for initialization
    void Start ()
    {
        GameObjects = GameObject.FindGameObjectsWithTag("DirectionalLights");
        m_NumberOfLights = GameObject.FindGameObjectsWithTag("DirectionalLights").Length;
        Lights = new Light[m_NumberOfLights];
        for (int i = 0; i <= m_NumberOfLights - 1; i++)
        {
            Lights[i] = GameObjects[i].GetComponent<Light>();
        }
	}
}
