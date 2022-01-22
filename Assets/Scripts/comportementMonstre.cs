using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class comportementMonstre : MonoBehaviourPunCallbacks
{
    public GameObject[] destinations; //Tableau des destinations possibles
    private NavMeshAgent navAgent; // Raccourci pour le navAgent
    public Slider sliderMonstre; //Slider de la barre de vie du monstre
    public float vieMonstre; //Vie du monstre
    public bool delaiHit; //D�lai pour par qu'il se fasse hit � multiple reprises

    public bool mort; //Bool�enne d�tectant la mort du monstre

    void Start()
    {
        //Raccourci du navAgent
        navAgent = GetComponent<NavMeshAgent>();

        //Remplir le tableau des destinations
        //� CHANGER PLUS TARD
        destinations = GameObject.FindGameObjectsWithTag("arbre");
        
        //Au d�part, chercher une cible
        chercherProchaineCible();
    }

    void Update()
    {
        //ROTATION DES BARRES DE VIE DES MONSTRES
        //Les sliders des autres monstres ont la m�me rotation que la cam�ra locale du joueur
        
        sliderMonstre.gameObject.transform.rotation = Camera.main.transform.rotation;

        //MORT DU MONSTRES
        if (mort == false && vieMonstre <= 0)
        {
            print(gameObject.name + "est mort");
            //Signaler qu'il est mort
            mort = true;

            //D�truire le monstre en RPC
            photonView.RPC("mortMonstre", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);

        }
    }

    //Fonction appel�e lorsque le monstre change de cible
    public void chercherProchaineCible()
    {
        //Piger un nombre al�atoire dans les destinations
        int destinationAleatoire = Random.Range(0, destinations.Length);

        //Changer la destination du monstre
        navAgent.SetDestination(destinations[destinationAleatoire].transform.position);

    }

    //Lorsque le monstre touche une cible, changer de cible...
    public void OnCollisionEnter(Collision infoCol)
    {
        if (infoCol.gameObject.tag == "arbre")
        {
            //Temporairement enlever le tag de l'arbre pour pas que le monstre re-target le m�me arbre
            infoCol.gameObject.tag = "Untagged";
            infoCol.gameObject.GetComponent<Collider>().enabled = false;

            //Remettre le tag apr�s 5 secondes
            StartCoroutine(remettreTag(infoCol.gameObject, 3f));

            //Rafra�chir le tableau des arbres
            destinations = GameObject.FindGameObjectsWithTag("arbre");

            //Appeler la fonction pour changer de cible
            chercherProchaineCible();
        }

        //Si je suis touch� par une balle de
        //***************ATTAQUE DE BASE - TARREV ********************
        if (infoCol.gameObject.tag == "BalleTarrev" && delaiHit == false)
        {
            //Indiquer au d�lai qu'il a �t� hit
            delaiHit = true;

            //Dire aux autres joueurs que le joueur a �t� touch� par une balle
            photonView.RPC("AppliquerDegatsBalleTarrev", RpcTarget.AllBuffered, deplacementAbiletesTarrev.degatAttaqueTarrev);
        }
    }

    //Fonction appel�e quand le joueur est touch� par une balle de
    //***************ATTAQUE DE BASE - TARREV ********************
    [PunRPC]
    IEnumerator AppliquerDegatsBalleTarrev(float degats)
    {
        //Baisser la vie et rafra�chir le slider
        vieMonstre -= degats;
        sliderMonstre.value = vieMonstre;

        yield return new WaitForSeconds(0.5f);

        //Indiquer qu'il peut maitenant �tre hit un autre fois
        delaiHit = false;
    }

    //Fonction appell�e pour remettre la cible � la normale
    public IEnumerator remettreTag(GameObject objetCollision, float delai)
    {
        //D�lai
        yield return new WaitForSeconds(delai);

        //Remettre le tag arbre
        objetCollision.gameObject.tag = "arbre";

        //Remettre son collider
        objetCollision.gameObject.GetComponent<Collider>().enabled = true;
    }

    //Fonction appel�e quand le monstre est mort
    [PunRPC]
    public void mortMonstre(int photonViewID)
    {
        //D�truire le monstre
        PhotonNetwork.Destroy(PhotonView.Find(photonViewID));
    }
}
