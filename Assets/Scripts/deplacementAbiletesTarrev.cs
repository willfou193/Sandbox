using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class deplacementAbiletesTarrev : MonoBehaviourPunCallbacks
{
    public Camera cameraJoueur; //Cam�ra du joueur
    public float vitesse = 7f; //Vitesse du personnage
    public float vitesseTourne; //Vitesse de rotation du personnage
    public float forceDuSaut; //Force du jump
    public float gravite; //Gravit�
    private CharacterController controleur; //R�f�rence au character controler
    private float velocitePersoY; //V�locit�
    //ATTAQUES
    public static float degatAttaqueTarrev; //D�g�ts de l'attaque de base de Tarrev
    public float rayon; //Rayon maximum � l'entour de Tarrev
    public GameObject particulePassif; //Particule du passi � tarrev
    List<float> distances = new List<float>(); //Liste des distances des objets proches
    List<GameObject> ennemisProches = new List<GameObject>(); //Liste des objets proches
    public float distancePlusProche; //Distance la plus proche lorsque le passif change
    public int indexPlusProche; //Index de l'objet le plus proche lorsque le passif change
    public float valeurPassifAConvertir; //Distance de la cible
    public float cooldownPassif = 20f; //Cooldown du passif


    void Start()
    {
        //Raccourci pour le character controler
        controleur = GetComponent<CharacterController>();

        //Activer la cam�ra pour le joueur local seulement
        if (photonView.IsMine)
        {
            cameraJoueur.gameObject.SetActive(true);
        }

        //� chaque 20 secondes, Tarrev marque l�ennemi le plus proche.
        InvokeRepeating("marquePassif", 1f, cooldownPassif);
    }

    /*
     * � chaque 30 secondes, Tarrev marque l�ennemi le plus proche. 
     *   Tarrev gagne plus d�hauteur de saut et de plus en plus de damage d�pendamment de la distance s�parant sa marque de lui.
     * 
    */

    void Update()
    {
        if (photonView.IsMine && viePersonnage.mort == false)
        {
            //G�re la rotation du joueur
            float tourne = Input.GetAxis("Mouse X") * vitesseTourne * Time.deltaTime;
            transform.Rotate(0f, tourne, 0f);

            //On r�cup�re la valeur de l'axe de rotation
            Vector3 valeursInputs = Vector3.zero;
            valeursInputs.x = Input.GetAxisRaw("Horizontal");
            valeursInputs.z = Input.GetAxisRaw("Vertical");

            //On converti un d�placement local en valeur selon les axes du monde
            Vector3 deplacement = transform.TransformDirection(valeursInputs * vitesse);

            //Si le joueur est au sol et ne bouge pas, r�gler la v�locit� � 0
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

            //On prend la velocite Y auquuel on ajoute la gravit�
            velocitePersoY += gravite * Time.deltaTime;
            deplacement.y = velocitePersoY;

            //On deplace notre character controller
            controleur.Move(deplacement * Time.deltaTime);

            deplacement.y = 0f;
        }

        //VALEURS POUR LES ABILITIES/SAUT
        if (ennemisProches.Count != 0)
        {
            //Calculer la distance entre Tarrev et l'ennemi le plus proche
            valeurPassifAConvertir = Vector3.Distance(gameObject.transform.position, ennemisProches[indexPlusProche].gameObject.transform.position);

            //Convertir la valeur pour...
            //L'ATTAQUE
            degatAttaqueTarrev = ((valeurPassifAConvertir + 10) / 100);

            //LE SAUT
            forceDuSaut = ((valeurPassifAConvertir / 4f) + 5f);
            print(degatAttaqueTarrev);
        }

        //CHANGER LA COULEUR DE LA PARTICULE SI CE N'EST PAS LA NOTRE
    }

    public void marquePassif()
    {
        
        //Aller chercher tous les colliders proche
        Collider[] colliders = Physics.OverlapSphere(transform.position, rayon);

        //Vider la liste
        distances.Clear();
        ennemisProches.Clear();

        //Pour chaque collider trouv�
        foreach (Collider objetProche in colliders)
        {
            //Si il a un rigidbody
            if (objetProche.GetComponent<Rigidbody>() != null)
            {
                
                //Remplir une liste avec toutes les distances entre l'objet et Tarrev
                float distance = Vector3.Distance(gameObject.transform.position, objetProche.gameObject.transform.position);

                //Ajouter l'ennemi � la liste des ennemis proches
                ennemisProches.Add(objetProche.gameObject);

                //Ajouter la distance � la liste des distances proches
                distances.Add(distance);
            }  
        }

        //Trouver la distance la plus proche
        distancePlusProche = distances.Min();

        //Trouver l'index de la distance la plus proche afin de trouver l'objet le plus proche
        indexPlusProche = distances.IndexOf(distancePlusProche);

        //Instancier la particule
        GameObject nouvelleParticulePassif = PhotonNetwork.Instantiate(particulePassif.name, 
        new Vector3(ennemisProches[indexPlusProche].gameObject.transform.position.x, ennemisProches[indexPlusProche].gameObject.transform.position.y + ennemisProches[indexPlusProche].gameObject.GetComponent<Collider>().bounds.size.y, ennemisProches[indexPlusProche].gameObject.transform.position.z), Quaternion.identity, 0);
        
        //La mettre enfant de la marque
        nouvelleParticulePassif.transform.parent = ennemisProches[indexPlusProche].gameObject.transform;

        //La d�truire apr�s 30 secondes
        StartCoroutine(enleverParticule(nouvelleParticulePassif.gameObject, cooldownPassif));
    }

    //Enlever la particule sur le r�seau
    public IEnumerator enleverParticule(GameObject particule, float delai)
    {
        //D�lai
        yield return new WaitForSeconds(delai);

        //D�truire la particule si l'ennemi n'a pas �t� tu�
        if (particule != null)
        {
            PhotonNetwork.Destroy(particule);
        }
    }
}


