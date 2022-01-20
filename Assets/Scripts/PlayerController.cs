using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{

    public Camera cameraJoueur; //Caméra du joueur
    public GameObject devantJoueur; //Référence au devant du joueur
    public GameObject Joueur; //GameObject du joueur
    public float longueurActiver; //Longueur entre le sort pour l'activer
    public float vitesse = 7f; //Vitesse du personnage
    public float vitesseTourne; //Vitesse de rotation du personnage
    public float forceDuSaut; //Force du jump
    public float gravite; //Gravité
    private CharacterController controleur; //Référence au character controler
    private float velocitePersoY; //Vélocité
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1); //Valeur du crouch
    private Vector3 normalScale = new Vector3(1, 1, 1); //Valeur normale 1,1,1
    private bool isCrouch = false; //Bool pour savoir si le joueur est crouched
    public bool levierActiver = false; //Bool pour savoir si le levier est activé
    public string sortEnQuestion; //Sort choisi
    public GameObject shortcut; //??
    bool caissePrise; //Bool pour savoir quelle caisse est prise

    void Start()
    {
        //Raccourci pour le character controler
        controleur = GetComponent<CharacterController>();

        //Racourci pour la caisse choisie
        shortcut = GameObject.FindGameObjectWithTag("caisseChoisie");

        //Activer la caméra pour le joueur local seulement
        if (photonView.IsMine)
        {
            cameraJoueur.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
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

            //Vérifier si je suis au sol, et l'assigner à une variable
            bool auSol = controleur.isGrounded;

            //Si on a appuyer sur la barre d'espace, sauter
            if (Input.GetKeyDown(KeyCode.Space) && auSol)
            {
                //Saut       
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

            //Crouch, si on appuie sur le bouton ctrl
            if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouch == false)
            {
                transform.localScale = crouchScale;
                isCrouch = true;
                if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouch)
                {
                    transform.localScale = normalScale;
                    isCrouch = false;
                }
            }

            //Ramasser un item avec e
            if (Input.GetKeyDown(KeyCode.E))
            {
                AmasserObjet();
            }

            //Jeter un item avec e
            if (Input.GetKeyDown(KeyCode.F) && caissePrise)
            {
                caissePrise = false;
                shortcut.GetComponent<Rigidbody>().useGravity = true;
                shortcut.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                shortcut.GetComponent<Rigidbody>().mass = 1;
                shortcut.transform.parent = null;
                shortcut.tag = "caisse";
            }
        }
    }

    //Fonction appeleé quand on ramasse un objet
    public void AmasserObjet()
    {
        //Déclarer le raycast "hit"
        RaycastHit hit;

        //Si la caméra vise bien la caisse à la bonne distance
        //En Appuyant sur F, un laser part tout droit de la caméra à une longueur précise et renvoie l'objet en contact
        //Séparer les codes d'amasserObjet dans un autre script
        if (Physics.Raycast(cameraJoueur.transform.position, cameraJoueur.transform.forward, out hit, longueurActiver))
        {
            //Si c'est un levier...
            if (hit.collider.tag == "levier")
            {
                //Et qu'il n'est pas déjà activé...
                if (!levierActiver)
                {
                    //Activer l'animation du levier
                    levierActiver = true;
                    hit.collider.gameObject.GetComponent<Animator>().SetBool("activeLevier", true);
                }
                else
                {
                    //Désactiver l'animation du levier
                    levierActiver = false;
                    hit.collider.gameObject.GetComponent<Animator>().SetBool("activeLevier", false);
                }
            }

            //Si c'est une caisse...
            if (hit.collider.tag == "caisse" && !caissePrise)
            {
                //Prendre la caisse
                caissePrise = true;
                hit.transform.parent = gameObject.transform;
                hit.transform.position = devantJoueur.transform.position;
                hit.transform.LookAt(Joueur.transform);
                hit.rigidbody.useGravity = false;
                hit.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                hit.rigidbody.mass = 0.01f;
                hit.transform.gameObject.tag = "caisseChoisie";
            }

            //Si c'est un sort...
            if (hit.collider.tag == "sort")
            {
                print("je prend le sort" + hit.collider.name);
                sortEnQuestion = hit.collider.name;
                //a la base, c'étais gameObject.AddComponent(sortEnQuestion);
                //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Scrips/PlayerController.cs (172,17)", sortEnQuestion);
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
