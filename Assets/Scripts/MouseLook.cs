using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 125f; //Sensitivité de la souris
    float xRotation = 0f; //Rotation 

    void Start()
    {
       //Lock la souris
       // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (viePersonnage.mort == false)
        {
            //Aller chercher la valeur x et y de la souris
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 1.5f * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 1.5f * Time.deltaTime;

            //Faire tourner la caméra
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
