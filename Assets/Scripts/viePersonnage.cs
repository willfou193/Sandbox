using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class viePersonnage : MonoBehaviourPunCallbacks
{
    public Slider sliderJoueur; //Slider de la barre de vie du joueur
    public float vieJoueur; //Vie du joueur
    public static bool mort; //Bool�enne d�tectant la mort du joueur
    public AudioClip sonMort; //Son de mort du joueur
    public GameObject ecranMort; //R�f�rence � l'�cran de mort

    void Start()
    {
        //Vie du joueur, en local, est � 1 au d�but de la partie ou quand il se d�co-reco
        /*if (photonView.IsMine)
        {
            vieJoueur = 1f;
        }*/
    }

    public void Awake()
    {
        //Ne pas d�truire le personnage s'il va dans d'autre sc�ne et il revient.
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //ROTATION DES BARRES DE VIE DES AUTRES ONT LA M�ME QUE TOI
        if (!photonView.IsMine)
        {
            //Les sliders des autres joueurs ont la m�me rotation que la cam�ra locale du joueur
            sliderJoueur.gameObject.transform.rotation = Camera.main.transform.rotation;
        }

        //MORT DU JOUEUR
        if (mort == false && vieJoueur <= 0 && photonView.IsMine)
        {
            //Signaler qu'il est mort
            mort = true;

            //Appeler la fonction qui joue le son de mort en RPC pour tous
            photonView.RPC("JoueSonMort", RpcTarget.All);

            //Activer l'�cran de mort
            ecranMort.SetActive(true);
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        //Si je suis touch� par une balle
        if (photonView.IsMine && collision.gameObject.tag == "BalleTarrev")
        {
            //Dire aux autres joueurs que le joueur a �t� touch� par une balle
            photonView.RPC("AppliquerDegats", RpcTarget.AllBuffered, 0.25f);
        }
    }

    //Fonction appel�e quand le joueur est touch� par une balle
    [PunRPC]
    public void AppliquerDegats(float degats)
    {
        //Baisser la vie et rafra�chir le slider
        vieJoueur -= degats;
        sliderJoueur.value = vieJoueur;
    }

    //Fonction appel� pour le son quand le joueur meurt
    [PunRPC]
    public void JoueSonMort()
    {
        GetComponent<AudioSource>().PlayOneShot(sonMort);
    }
}
