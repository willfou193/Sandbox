using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportPlayer : MonoBehaviour
{
    void OnEnable()
    {
        eventManager.OnClicked += Teleport;
    }

    void OnDisable()
    {
        eventManager.OnClicked -= Teleport;
    }
    void Teleport()
    {
        Vector3 pos = transform.position;
        pos.y = Random.Range(1f, 10f);
        transform.position = pos;
    }
}
