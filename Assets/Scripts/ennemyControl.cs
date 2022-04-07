using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ennemyControl : MonoBehaviour
{
    public NavMeshAgent navMeshAgent; //Référence au navmesh du monstre
    //public ScriptableFloat ennemySpeed; //Référence à notre donnée de vitesse
    public GameObjectRuntimeSet playerSet; //Reference to the player

    private Transform currentTarget; //Target of our ennemy

    void Start()
    {
        //The target is our first player on the list
        currentTarget = playerSet.GetItemIndex(0).transform;
    }

    void Update()
    {
        //navMeshAgent.speed = ennemySpeed.value;

        //If we already have a target
        if(currentTarget != null)
        {
            navMeshAgent.SetDestination(currentTarget.position);
        }
    }

    public void getRandomPlayer()
    {
        currentTarget = playerSet.GetItemIndex(0).transform;
    }
}
