using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnProjectile : MonoBehaviourPunCallbacks
{
    public GameObject emplacementBalle; //Référence au gameObject de la balle
    public float vitesseBalle; //Vitesse de la balle
    private bool peutTirer = true; //Est-ce que le personnage peut tirer
    public GameObject muzzlePrefab; //Référence à un prefab de particule
    public AudioClip gunshot; //Son du tir
    public float cooldownTire; //Cooldown du tir

    void FixedUpdate()
    {
        //Lorsque le joueur appuie sur clique gauche et qu'il peut tirer
        if(Input.GetKey(KeyCode.Mouse0) && peutTirer && photonView.IsMine && viePersonnage.mort == false){

            //Indiquer qu'il peut plus tirer avant un petit délai
            peutTirer = false;

            //Activer la particule de tir
            muzzlePrefab.SetActive(true);

            //Appeler la fonction de cooldown
            Invoke("RechargeTire", cooldownTire);

            //Jouer le son en RPC
            photonView.RPC("JoueSonTir", RpcTarget.All);

            //Instancier la balle sur réseau
            GameObject cloneBalle = PhotonNetwork.Instantiate("Tarrev_AttaqueBase", emplacementBalle.transform.position, emplacementBalle.transform.rotation);
            cloneBalle.SetActive(true);
            cloneBalle.GetComponent<Rigidbody>().velocity = cloneBalle.transform.forward * vitesseBalle;
        }
    }

    //Fonction permettant de réactiver le tir
    void RechargeTire(){
        peutTirer = true;
        muzzlePrefab.SetActive(false);
    }

    //Fonction permettant de faire le son du tir
    [PunRPC]
    void JoueSonTir()
    {
        print("PEW");
        GetComponent<AudioSource>().PlayOneShot(gunshot);
    }
}
