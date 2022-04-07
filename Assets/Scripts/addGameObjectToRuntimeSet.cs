using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addGameObjectToRuntimeSet : MonoBehaviour
{
    public GameObjectRuntimeSet gameObjectRuntimeSet;
    // Start is called before the first frame update
    private void OnEnable()
    {
        gameObjectRuntimeSet.AddToList(this.gameObject);
    }

    private void OnDisable()
    {
        gameObjectRuntimeSet.RemoveFromList(this.gameObject);
    }
}
