using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class deplacementAbiletesTarrev : MonoBehaviourPunCallbacks
{
    public Camera cameraJoueur; //Caméra du joueur
    public float vitesse = 7f; //Vitesse du personnage
    public float vitesseTourne; //Vitesse de rotation du personnage
    public float forceDuSaut; //Force du jump
    public float gravite; //Gravité
    private CharacterController controleur; //Référence au character controler
    private float velocitePersoY; //Vélocité
    //ATTAQUES
    public static float degatAttaqueTarrev; //Dégâts de l'attaque de base de Tarrev
    public float rayon; //Rayon maximum à l'entour de Tarrev
    public GameObject particulePassif; //Particule du passi à tarrev
    List<float> distances = new List<float>(); //Liste des distances des objets proches
    List<GameObject> ennemisProches = new List<GameObject>(); //Liste des objets proches
    public float distancePlusProche; //Distance la plus proche lorsque le passif change
    public int indexPlusProche; //Index de l'objet le plus proche lorsque le passif change
    public float valeurPassifAConvertir; //Distance de la cible
    public float cooldownPassif = 20f; //Cooldown du passif
    public bool verifierCible = true; //Permet de vérifier si on a changer de cible
    public GameObject ciblePassif; //Cible du passif
    public LineRenderer LineRenderer;

    void Start()
    {
        //Raccourci pour le character controler
        controleur = GetComponent<CharacterController>();

        //Activer la caméra pour le joueur local seulement
        if (photonView.IsMine)
        {
            cameraJoueur.gameObject.SetActive(true);
        }

        //À chaque 20 secondes, Tarrev marque l’ennemi le plus proche.
        InvokeRepeating("marquePassif", 0f, cooldownPassif);

        LineRenderer = GameObject.FindGameObjectWithTag("ligne").GetComponent<LineRenderer>();
    }

    /*
     * À chaque 30 secondes, Tarrev marque l’ennemi le plus proche. 
     *   Tarrev gagne plus d’hauteur de saut et de plus en plus de damage dépendamment de la distance séparant sa marque de lui.
     * 
    */

    void Update()
    {
        if (photonView.IsMine && viePersonnage.mort == false)
        {
            //Gère la rotation du joueur
            float tourne = Input.GetAxis("Mouse X") * vitesseTourne * Time.deltaTime;
            transform.Rotate(0f, tourne, 0f);

            //On récupère la valeur de l'axe de rotation
            Vector3 valeursInputs = Vector3.zero;
            valeursInputs.x = Input.GetAxisRaw("Horizontal");
            valeursInputs.z = Input.GetAxisRaw("Vertical");

            //On converti un déplacement local en valeur selon les axes du monde
            Vector3 deplacement = transform.TransformDirection(valeursInputs * vitesse);

            //Si le joueur est au sol et ne bouge pas, régler la vélocité à 0
            if (controleur.isGrounded && velocitePersoY < 0)
            {
                velocitePersoY = 0f;
            }

            //Si on a appuyer sur la barre d'espace et qu'on est au sol, sauter
            if (Input.GetKeyDown(KeyCode.Space) && controleur.isGrounded)
            {   
                velocitePersoY = forceDuSaut;
            }

            //Vitesse de sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                vitesse = 15f;
            }

            //Sinon, vitesse normale
            else
            {
                vitesse = 10f;
            }

            //On prend la velocite Y auquuel on ajoute la gravité
            velocitePersoY += gravite * Time.deltaTime;
            deplacement.y = velocitePersoY;

            //On deplace notre character controller
            controleur.Move(deplacement * Time.deltaTime);

            deplacement.y = 0f;
        }

        //VALEURS POUR LES ABILITIES/SAUT
        if (ennemisProches.Count != 0 && ennemisProches[indexPlusProche].gameObject != null)
        {
            //Calculer la distance entre Tarrev et l'ennemi le plus proche
            valeurPassifAConvertir = Vector3.Distance(gameObject.transform.position, ennemisProches[indexPlusProche].gameObject.transform.position);

            //Convertir la valeur pour...
            //L'ATTAQUE
            degatAttaqueTarrev = ((valeurPassifAConvertir + 10) / 100);

            //LE SAUT
            forceDuSaut = ((valeurPassifAConvertir / 4f) + 5f);
            //int(degatAttaqueTarrev);

            //Tracer une ligne entre Tarrev et sa cible
            // set the color of the line
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.red;

            // set width of the renderer
            LineRenderer.startWidth = 0.3f;
            LineRenderer.endWidth = 0.3f;

            // set the position
            LineRenderer.SetPosition(0, cameraJoueur.transform.position);
            LineRenderer.SetPosition(1, ciblePassif.transform.position);
        }

        //SI LA CIBLE A ÉTÉ TUÉE, CHANGER DE CIBLE
        if (ciblePassif.gameObject == null && verifierCible == false)
        {
            verifierCible = true;
            marquePassif();
        }
    }

    public void marquePassif()
    {
        verifierCible = false;
        //Aller chercher tous les colliders proche
        Collider[] colliders = Physics.OverlapSphere(transform.position, rayon);

        //Vider la liste
        distances.Clear();
        ennemisProches.Clear();

        //Pour chaque collider trouvé
        foreach (Collider objetProche in colliders)
        {
            //Si il a un rigidbody
            if ((colliders.LongLength > 0) && (objetProche.gameObject.tag == "Player" || objetProche.gameObject.tag == "Ennemi") && (objetProche.gameObject.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID))
            {
                print("marque");
                //Remplir une liste avec toutes les distances entre l'objet et Tarrev
                float distance = Vector3.Distance(gameObject.transform.position, objetProche.gameObject.transform.position);

                //Ajouter l'ennemi à la liste des ennemis proches
                ennemisProches.Add(objetProche.gameObject);

                //Ajouter la distance à la liste des distances proches
                distances.Add(distance);
            }  
        }
        if(distances.Count > 0)
        {
            //Trouver la distance la plus proche
            distancePlusProche = distances.Min();

            //Trouver l'index de la distance la plus proche afin de trouver l'objet le plus proche
            indexPlusProche = distances.IndexOf(distancePlusProche);

            //Instancier la particule
            GameObject nouvelleParticulePassif = PhotonNetwork.Instantiate(particulePassif.name,
            new Vector3(ennemisProches[indexPlusProche].gameObject.transform.position.x, ennemisProches[indexPlusProche].gameObject.transform.position.y + ennemisProches[indexPlusProche].gameObject.GetComponent<Collider>().bounds.size.y, ennemisProches[indexPlusProche].gameObject.transform.position.z), Quaternion.identity, 0);

            //storer la marque
            ciblePassif = ennemisProches[indexPlusProche].gameObject;
            print("ENNEMI PROCHE" + ciblePassif.name);
            //La mettre enfant de la marque
            nouvelleParticulePassif.transform.parent = ennemisProches[indexPlusProche].gameObject.transform;

            //Changer sa couleur si on est owner de la particule
            if (nouvelleParticulePassif.gameObject.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                ParticleSystem.MainModule settingsParticule = nouvelleParticulePassif.GetComponent<ParticleSystem>().main;
                settingsParticule.startColor = new ParticleSystem.MinMaxGradient(Color.yellow);
            }

            //La détruire après 20 secondes
            StartCoroutine(enleverParticule(nouvelleParticulePassif.gameObject, cooldownPassif));
        }
    }

    //Enlever la particule sur le réseau
    public IEnumerator enleverParticule(GameObject particule, float delai)
    {
        //Délai
        yield return new WaitForSeconds(delai);

        //Détruire la particule si l'ennemi n'a pas été tué
        if (particule != null)
        {
            PhotonNetwork.Destroy(particule);
        }
    }
}


