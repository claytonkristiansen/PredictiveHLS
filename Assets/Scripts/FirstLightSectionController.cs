using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLightSectionController : MonoBehaviour {

    public Light[] lights;

    // Use this for initialization
    void Start ()
    {
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
                lights[k] = GetComponent<Transform>().GetChild(i).FindChild("Directional Light").gameObject.GetComponent<Light>();
            }
        }
        
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = 2;
        }
    }
}
