using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Projectile : MonoBehaviourPunCallbacks
{
    public GameObject hitPrefab; //R�f�rence � la particule hit
    public AudioClip impact; //Son de l'impact

    void Start()
    {
        //La d�truire sur r�seau apr�s 3 secondes
        StartCoroutine(detruireObjet(gameObject, 5f));

        //Ignorer la collision avec les arbres, les buissons...
        Physics.IgnoreLayerCollision(10, 11);
        //Ignorer la collision avec soi-m�me
        //Physics.IgnoreLayerCollision(8, 11);
    }

    //Lorsque la balle collide avec un objet...
    void OnCollisionEnter(Collision collisionInfo) {
        if (photonView.IsMine)
        {
            //Jouer le son d'impact sur r�seau
            photonView.RPC("JoueSonShoot", RpcTarget.All);

            //Trouver le point de contact
            ContactPoint contact = collisionInfo.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            //Instancier la particule hit sur r�seau
            if (hitPrefab != null)
            {
                GameObject hitVFX = PhotonNetwork.Instantiate(hitPrefab.name, pos, rot);
                hitVFX.SetActive(true);

                //La d�truire sur r�seau apr�s 0.5secondes
                StartCoroutine(detruireObjet(hitVFX.gameObject, 0.5f));
            }

            StartCoroutine(detruireObjet(gameObject, 0f));
        }
    }

    //D�truire l'objet
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
