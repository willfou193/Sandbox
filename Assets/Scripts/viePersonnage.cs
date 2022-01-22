using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class viePersonnage : MonoBehaviourPunCallbacks
{
    public Slider sliderJoueur; //Slider de la barre de vie du joueur
    public Slider sliderJoueurUI; //Slider de la barre de vie du joueur UI
    public float vieJoueur; //Vie du joueur
    public static bool mort; //Booléenne détectant la mort du joueur
    public AudioClip sonMort; //Son de mort du joueur
    public GameObject ecranMort; //Référence à l'écran de mort
    public float currentTime = 0f; //Temps courant du death timer
    public float startingTime = 5f; //Temps courant du death timer
    public TextMeshProUGUI deathTimer; //Référence au deathtimer
    public GameObject[] positionsSpawn; //Référence au spawner de joueur
    public bool delaiHit; //Délai pour par qu'il se fasse hit à multiple reprises

    void Start()
    {
        currentTime = startingTime;

        positionsSpawn = GameObject.FindGameObjectsWithTag("positions");
    }

    public void Awake()
    {
        //Ne pas détruire le personnage s'il va dans d'autre scène et il revient.
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //ROTATION DES BARRES DE VIE DES AUTRES ONT LA MÊME QUE TOI
        if (!photonView.IsMine)
        {
            //Les sliders des autres joueurs ont la même rotation que la caméra locale du joueur
            sliderJoueur.gameObject.transform.rotation = Camera.main.transform.rotation;

            sliderJoueurUI.gameObject.SetActive(false);
        }

        //MORT DU JOUEUR
        if (mort == false && vieJoueur <= 0 && photonView.IsMine)
        {
            //Signaler qu'il est mort
            mort = true;

            //Appeler la fonction qui joue le son de mort en RPC pour tous
            photonView.RPC("JoueSonMort", RpcTarget.All);

            //Activer l'écran de mort
            ecranMort.SetActive(true);
        }

        //COUNTDOWN TIMER
        if (mort == true && photonView.IsMine)
        {
            currentTime -= 1 * Time.deltaTime;
            deathTimer.text = currentTime.ToString("0");

            if (currentTime <= 0)
            {
                //Désactiver l'écran de mort
                ecranMort.SetActive(false);

                sliderJoueurUI.value = (vieJoueur + 1f);

                //Réanimer le joueur
                photonView.RPC("Revivre", RpcTarget.AllBuffered);

                //Indiquer qu'il est en vie
                mort = false;

                //Remettre le timer à sa valeur max
                currentTime = startingTime;

                //Téléporter le joueur à un endroit random
                int nombreRandom = Random.Range(0, positionsSpawn.Length);
                gameObject.transform.position = positionsSpawn[nombreRandom].gameObject.transform.position;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Si je suis touché par une balle
        if (photonView.IsMine && collision.gameObject.tag == "BalleTarrev" && delaiHit == false)
        {
            //Indiquer au délai qu'il a été hit
            delaiHit = true;

            sliderJoueurUI.value = (vieJoueur - deplacementAbiletesTarrev.degatAttaqueTarrev);
            //Dire aux autres joueurs que le joueur a été touché par une balle
            photonView.RPC("AppliquerDegats", RpcTarget.AllBuffered, deplacementAbiletesTarrev.degatAttaqueTarrev);
        }
    }

    //Fonction appelée quand le joueur est touché par une balle
    [PunRPC]
    IEnumerator AppliquerDegats(float degats)
    {
        //Baisser la vie et rafraîchir le slider
        vieJoueur -= degats;
        sliderJoueur.value = vieJoueur;
        sliderJoueurUI.value = vieJoueur;

        yield return new WaitForSeconds(0.5f);

        //Indiquer qu'il peut maitenant être hit un autre fois
        delaiHit = false;
    }

    //Fonction appelé pour le son quand le joueur meurt
    [PunRPC]
    public void JoueSonMort()
    {
        GetComponent<AudioSource>().PlayOneShot(sonMort);
    }

    //Fonction appelée quand le joueur revient en vie
    [PunRPC]
    public void Revivre()
    {
        //Lui redonner tout sa vie et rafraîchir le slider
        vieJoueur = 1f;
        sliderJoueur.value = vieJoueur;
        sliderJoueurUI.value = vieJoueur;
    }
}
