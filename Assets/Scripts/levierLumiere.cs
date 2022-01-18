using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levierLumiere : MonoBehaviour
{
    public GameObject refBool;
    public GameObject lumiere;
    void ActiverDesactiverLumiere()
    {
        if (refBool.GetComponent<PlayerController>().levierActiver == true)
        {
            lumiere.GetComponent<MeshRenderer>().material.color = Color.green;
            
        }
        else
        {
            lumiere.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
