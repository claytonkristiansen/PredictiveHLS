using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatsController : MonoBehaviour
{
    public Text LightsOnText, LightsOffText, OnToTotalTimeRatio, OffToTotalTimeRatio;
    public Text PowerSavedText, MoneySavedText;
    public Text PercentageUsageText;

    /// <summary>
    /// Energy (in kilowatt/hours) used in roadways in the US
    /// </summary>
    public float TotalEnergyUsedOnRoadwayLighting;
    /// <summary>
    /// Price of a kilowatt/hour in the US
    /// </summary>
    public float PriceOfKilowattHour;
    public float PercentOfRoadsUsingTheSystem;
    // Use this for initialization
    void Start ()
    {
        TotalEnergyUsedOnRoadwayLighting = TotalEnergyUsedOnRoadwayLighting * PercentOfRoadsUsingTheSystem;
        float OffIntensityToOnIntensity = RoadController.LightsOffIntensity / RoadController.LightsOnIntensity;
        LightsOnText.text = "Total Time On: " + StaticVariables.CommulativeLightsOnTime.ToString();
        LightsOffText.text = "Total Time Off: " + StaticVariables.CommulativeLightsOffTime.ToString();
        float i = (StaticVariables.CommulativeLightsOnTime / (StaticVariables.CommulativeLightsOnTime + StaticVariables.CommulativeLightsOffTime));
        float w = (StaticVariables.CommulativeLightsOffTime / (StaticVariables.CommulativeLightsOnTime + StaticVariables.CommulativeLightsOffTime));
        OnToTotalTimeRatio.text = "On to Total Time Ratio: " + i.ToString();
        OffToTotalTimeRatio.text = "Off to Total Time Ratio: " + w.ToString();
        float powerSaved = TotalEnergyUsedOnRoadwayLighting * w * OffIntensityToOnIntensity;
        float moneySaved = powerSaved * PriceOfKilowattHour;
        PercentageUsageText.text = "With " + (PercentOfRoadsUsingTheSystem * 100).ToString() + "% of the U.S. Using this System:";
        PowerSavedText.text = "Power Saved: " + powerSaved.ToString() + " Kilowatt/Hours";
        MoneySavedText.text = "Money Saved: $" + moneySaved.ToString();
        StaticVariables.LReset();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
