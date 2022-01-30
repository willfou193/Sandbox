using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boiteDeplacement : MonoBehaviour
{
    public float range;  //Range droit et gauche
    public float vitesse; //Vitesse de la boite
    private Vector3 positionDepart; //Position de départ
    public bool lache; //Bool qui détermine si la boite est lâchée ou pas
    public float forceGravite; //Gravité qui va pousser la boite vers le bas
    public bool forceApplique; //Déterminé si il a déjà été pull down
    public AudioClip sonSpawn; //Quand la boite apparait
    public AudioClip sonTouche; //Quand la boite touche qqch
    public bool sonJoue; //Le son ne joue pas constamment

    void Start()
    {
        positionDepart = transform.position;
        
    }

    void Update()
    {
        if(generationBoites.finJeu == false)
        {
            if (lache == false)
            {
                Vector3 deplacement = positionDepart;
                deplacement.x += range * Mathf.Sin(Time.time * vitesse);
                transform.position = deplacement;
            }

            if (Input.GetKeyDown(KeyCode.Space) && forceApplique == false)
            {
                lache = true;
                forceApplique = true;
                GetComponent<Rigidbody>().velocity = new Vector3(0, forceGravite, 0);
                GetComponent<AudioSource>().PlayOneShot(sonSpawn);
            }
        }

        if(generationBoites.finJeu == true && sonJoue == false)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (sonJoue == false)
        {
            sonJoue = true;
            GetComponent<AudioSource>().PlayOneShot(sonTouche);
            gameObject.tag = "boite";
        }

        if (collision.gameObject.name == "vide")
        {
            Destroy(gameObject);
        }
    }
}
