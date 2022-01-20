using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnJoueurs : MonoBehaviour
{
    public GameObject[] prefabJoueurs; //Tous les prefabs de mod�les de joueurs possibles
    public Transform[] spawnPoints; //Tableau des positions al�atoires possibles
    // Start is called before the first frame update
    void Start()
    {
        //Piger un nombre al�atoire
        int nombreRandom = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[nombreRandom];
        GameObject joueurAfaireSpawn;

        if(PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"] == null)
        {
            joueurAfaireSpawn = prefabJoueurs[0];
        }
        else
        {
            joueurAfaireSpawn = prefabJoueurs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }

        //Instancier le joueur
        PhotonNetwork.Instantiate(joueurAfaireSpawn.name, spawnPoint.position, Quaternion.identity);
    }
}
