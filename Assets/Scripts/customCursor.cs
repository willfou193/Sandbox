using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customCursor : MonoBehaviour
{
    private void Awake(){
        transform.position = Input.mousePosition;
    }    
    private void Update(){
        transform.position = Input.mousePosition;
    }
}
