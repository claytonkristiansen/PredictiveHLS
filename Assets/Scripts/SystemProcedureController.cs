using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemProcedureController : MonoBehaviour
{
    public void ProcedureChanged()
    {
        if (GetComponent<Dropdown>().value == 0)
        {
            StaticVariables.SetSystemProcedure(SYSTEM_PROCEDURE.SECTION_BASED);
        }
        if (GetComponent<Dropdown>().value == 1)
        {
            StaticVariables.SetSystemProcedure(SYSTEM_PROCEDURE.PREDICTION_BASED);
        }
        if (GetComponent<Dropdown>().value == 2)
        {
            StaticVariables.SetSystemProcedure(SYSTEM_PROCEDURE.BOTH);
        }
    }
}
