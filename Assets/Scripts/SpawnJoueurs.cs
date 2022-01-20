using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnJoueurs : MonoBehaviour
{
    public GameObject[] prefabJoueurs; //Tous les prefabs de modèles de joueurs possibles
    public Transform[] spawnPoints; //Tableau des positions aléatoires possibles
    // Start is called before the first frame update
    void Start()
    {
        //Piger un nombre aléatoire
        int nombreRandom = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[nombreRandom];

        //Associer le joueur à faire spawn avec ce qu'il a choisi comme avatar
        GameObject joueurAfaireSpawn = prefabJoueurs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

        //Instancier le joueur
        PhotonNetwork.Instantiate(joueurAfaireSpawn.name, spawnPoint.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
