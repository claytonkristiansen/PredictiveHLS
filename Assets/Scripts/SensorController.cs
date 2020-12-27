using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    public bool isTriggered;
    public bool lookForTrigger;
    private bool wasTriggeredThisFrame;
    private GameObject collisionObject;
	// Use this for initialization
	void Start ()
    {
        isTriggered = false;
        wasTriggeredThisFrame = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (lookForTrigger)
        {
            if (other.gameObject.transform.CompareTag("Car"))
            {
                //lookForTrigger = false;
                isTriggered = true;
                //wasTriggeredThisFrame = true;
                collisionObject = other.gameObject;
            }
            else
            {
                //wasTriggeredThisFrame = false;
                isTriggered = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        //if (!StaticVariables.isPaused)
        //{
        //    if (!wasTriggeredThisFrame)
        //    {
        //        isTriggered = false;
        //    }
        //    else if (wasTriggeredThisFrame)
        //    {
        //        wasTriggeredThisFrame = false;
        //    }
        //}
        //else if (StaticVariables.isPaused)
        //{

        //}
    }

    public GameObject GetCollisionInfo()
    {
        if(isTriggered)
        {
            return collisionObject;
        }
        else
        {
            return null;
        }
    }
}
