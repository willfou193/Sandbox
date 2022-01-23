using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Projectile : MonoBehaviourPunCallbacks
{
    public GameObject hitPrefab; //Référence à la particule hit
    public AudioClip impact; //Son de l'impact

    void Start()
    {
        //La détruire sur réseau après 3 secondes
        StartCoroutine(detruireObjet(gameObject, 5f));

        //Ignorer la collision avec les arbres, les buissons...
        Physics.IgnoreLayerCollision(10, 11);
        //Ignorer la collision avec soi-même
        //Physics.IgnoreLayerCollision(8, 11);
    }

    //Lorsque la balle collide avec un objet...
    void OnCollisionEnter(Collision collisionInfo) {
        if (photonView.IsMine)
        {
            //Jouer le son d'impact sur réseau
            photonView.RPC("JoueSonShoot", RpcTarget.All);

            //Trouver le point de contact
            ContactPoint contact = collisionInfo.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            //Instancier la particule hit sur réseau
            if (hitPrefab != null)
            {
                GameObject hitVFX = PhotonNetwork.Instantiate(hitPrefab.name, pos, rot);
                hitVFX.SetActive(true);

                //La détruire sur réseau après 0.5secondes
                StartCoroutine(detruireObjet(hitVFX.gameObject, 0.5f));
            }

            StartCoroutine(detruireObjet(gameObject, 0f));
        }
    }

    //Détruire l'objet
    public IEnumerator detruireObjet(GameObject objetADetruire, float delai)
    {
        if (photonView.IsMine)
        {
            yield return new WaitForSeconds(delai);
            PhotonNetwork.Destroy(objetADetruire);
        }
    }

    //Fonction permettant de faire le son du tir
    [PunRPC]
    void JoueSonShoot()
    {
        GetComponent<AudioSource>().PlayOneShot(impact);
    }
}
