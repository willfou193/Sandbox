using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class comportementMonstre : MonoBehaviourPunCallbacks
{
    public GameObject[] destinations; //Tableau des destinations possibles
    public NavMeshAgent navAgent; // Raccourci pour le navAgent

    void Start()
    {
        //Raccourci du navAgent
        navAgent = GetComponent<NavMeshAgent>();

        //Remplir le tableau des destinations
        //� CHANGER PLUS TARD
        destinations = GameObject.FindGameObjectsWithTag("arbre");
        
        //Au d�part, chercher une cible
        chercherProchaineCible();

        //Ignorer les collisions avec tout sauf les ARBRES et les BALLES
    }

    //Fonction appel�e lorsque le monstre change de cible
    public void chercherProchaineCible()
    {
        print(gameObject.name + "change de cible");

        //Piger un nombre al�atoire dans les destinations
        int destinationAleatoire = Random.Range(0, destinations.Length);

        //Changer la destination du monstre
        navAgent.SetDestination(destinations[destinationAleatoire].transform.position);

    }

    //Lorsque le monstre touche une cible, changer de cible...
    public void OnCollisionEnter(Collision infoCol)
    {
        print(gameObject.name + "a touch� une cible");
        if (infoCol.gameObject.tag == "arbre")
        {
            //Appeler la fonction pour changer de cible
            chercherProchaineCible();

            //Temporairement enlever le tag de l'arbre pour pas que le monstre re-target le m�me arbre
            infoCol.gameObject.tag = "Untagged";
            infoCol.gameObject.GetComponent<Collider>().enabled = false;

            //Remettre le tag apr�s 5 secondes
            StartCoroutine(remettreTag(gameObject, 3f));

            //Rafra�chir le tableau des arbres
            destinations = GameObject.FindGameObjectsWithTag("arbre");
        }
    }

    public IEnumerator remettreTag(GameObject objetCollision, float delai)
    {
        yield return new WaitForSeconds(delai);
        objetCollision.gameObject.tag = "arbre";
        objetCollision.gameObject.GetComponent<Collider>().enabled = true;
    }
}
