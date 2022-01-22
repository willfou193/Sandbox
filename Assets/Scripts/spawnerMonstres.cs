using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class spawnerMonstres : MonoBehaviourPunCallbacks
{
    public GameObject[] emplacementsSpawn; //Tableau des emplacements possibles des monstres
    public GameObject[] monstres; //Tableau contenant tous les prefabs des monstres
    public float cooldownMonstres; //Vitesse � laquelle les monstres vont spawn

    void Start()
    {
        //Invoquer un monstre � chaque x secondes
        InvokeRepeating("Spawn", 0f, cooldownMonstres);

        //Remplir le tableau des emplacements al�atoires avec tous les buissons de la sc�ne
        emplacementsSpawn = GameObject.FindGameObjectsWithTag("buissons");
    }

    //Fonction appel�e afin d'instancier un monstre r�seau sur la sc�ne
    public void Spawn()
    {
        
        if (PhotonNetwork.IsMasterClient == true)
        {
            print("J'ai spawn un monstre");
            //Piger un nombre/monstre al�atoire
            int monstreAleatoire = Random.Range(0, monstres.Length);

            //Piger un autre nombre al�atoire pour la position
            int emplacementAleatoire = Random.Range(0, emplacementsSpawn.Length);

            //Instancier le monstre
            PhotonNetwork.InstantiateRoomObject(monstres[monstreAleatoire].gameObject.name,
            emplacementsSpawn[emplacementAleatoire].gameObject.transform.position, Quaternion.identity, 0, null);
        }
    }
}
