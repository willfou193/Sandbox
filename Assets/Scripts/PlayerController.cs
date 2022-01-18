using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Camera cameraJoueur;
    public GameObject devantJoueur;
    public GameObject Joueur;
    
  
    public float longueurActiver;
    public float vitesse = 7f;
    public float vitesseTourne;
    public float forceDuSaut;
    public float gravite;
    private CharacterController controleur;
    private float velocitePersoY;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 normalScale = new Vector3(1, 1, 1);
    private bool isCrouch = false;
    public bool levierActiver = false;
    public string sortEnQuestion; 
    public GameObject shortcut;
    
    bool caissePrise;
    
   
    // Start is called before the first frame update
    void Start()
    {
        controleur = GetComponent<CharacterController>();
        
        shortcut =  GameObject.FindGameObjectWithTag("caisseChoisie");

    }

    // Update is called once per frame
    void Update()
    {
        //Gère la rotation de la capsule
        float tourne = Input.GetAxis("Mouse X") * vitesseTourne * Time.deltaTime;
        transform.Rotate(0f, tourne, 0f);

        //On récupère la valeur de
        Vector3 valeursInputs = Vector3.zero;
        valeursInputs.x = Input.GetAxisRaw("Horizontal");
        valeursInputs.z = Input.GetAxisRaw("Vertical");

        //On converti un déplacement local en valeur selon les axes du monde
        Vector3 deplacement = transform.TransformDirection(valeursInputs * vitesse);



        if (controleur.isGrounded && velocitePersoY < 0) velocitePersoY = 0f;

        // est-ce que je suis au sol
        bool auSol = controleur.isGrounded;

        if(Input.GetKeyDown(KeyCode.Space) && auSol)
        {
            //saut possible ->            
            velocitePersoY = forceDuSaut;                        
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            vitesse = 15;           
        }
        else
        {
            vitesse = 10f;          
        }
        //On prend la velocite Y auquuel on ajoute la gravité
        velocitePersoY += gravite * Time.deltaTime;
        deplacement.y = velocitePersoY;
        // on deplace notre character controller
        controleur.Move(deplacement * Time.deltaTime);

        //crouch
        GererCrouch();
        GestionAnim(deplacement);

        if (Input.GetKeyDown(KeyCode.E))
        {
            AmasserObjet();
        }
        if (Input.GetKeyDown(KeyCode.F) && caissePrise)
        {
            

            print("drop la bacaisse dans l'fond la boite à bois");
            caissePrise = false;
            shortcut.GetComponent<Rigidbody>().useGravity = true;
            shortcut.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            shortcut.GetComponent<Rigidbody>().mass = 1;
            shortcut.transform.parent = null;
            shortcut.tag = "caisse";
           


        }

    }

    void GestionAnim(Vector3 valeurDeplacement)
    {
        valeurDeplacement.y = 0f;
       
    
    }


    void GererCrouch() 
    {
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
        
        
    }

    public void AmasserObjet()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(cameraJoueur.transform.position, cameraJoueur.transform.forward, out hit, longueurActiver))
        //En Appuyant sur F, un laser part tout droit de la caméra à une longueur précise et renvoie l'objet en contact
        //Séparer les codes d'amasserObjet dans un autre script
        {
            
            if (hit.collider.tag == "levier")
            {
                if (!levierActiver)
                {   
                    levierActiver = true;
                    hit.collider.gameObject.GetComponent<Animator>().SetBool("activeLevier", true);                
                    print("levier Activé");
                    
                    
                }
                else
                {
                    levierActiver = false;
                    print("levier Désactivé");
                    hit.collider.gameObject.GetComponent<Animator>().SetBool("activeLevier", false);                 
                }
            }
            if(hit.collider.tag == "caisse" && !caissePrise)
            {
                print("Je prend la caisse");
                caissePrise = true;
                hit.transform.parent = gameObject.transform;
                hit.transform.position = devantJoueur.transform.position;
                hit.transform.LookAt(Joueur.transform);
                hit.rigidbody.useGravity = false;
                hit.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                hit.rigidbody.mass = 0.01f;
                hit.transform.gameObject.tag = "caisseChoisie";   
            }

            if(hit.collider.tag == "sort"){
                print("je prend le sort" + hit.collider.name);
                
                sortEnQuestion = hit.collider.name;
                //a la base, c'étais gameObject.AddComponent(sortEnQuestion);
                UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Scrips/PlayerController.cs (172,17)", sortEnQuestion);
                Destroy(hit.collider.gameObject);  
            }


        }
    }
}
