using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    public GameObject balle; // Référence au gameObject de la balle
    private GameObject effectASpawn;
    public float vitesseBalle; // Vitesse de la balle
    private bool peutTirer; // Est-ce que le personnage peut tirer
    public GameObject muzzlePrefab;
    public AudioClip gunshot;

    // Start is called before the first frame update
    void Start()
    {
        peutTirer = true;
          if(muzzlePrefab != null) {
            GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.SetActive(true);
            muzzleVFX.transform.forward = gameObject.transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)){
            SpawnVfx();
        }
    }


    void SpawnVfx(){
        if(peutTirer){
            peutTirer = false;
            muzzlePrefab.SetActive(true);
            Invoke("RechargeTire",0.2f);
            GetComponent<AudioSource>().PlayOneShot(gunshot);
            GameObject cloneBalle = Instantiate(balle, balle.transform.position, balle.transform.rotation);
            cloneBalle.SetActive(true);
            cloneBalle.GetComponent<Rigidbody>().velocity = cloneBalle.transform.forward * vitesseBalle;
        }   
    }
    void RechargeTire(){
        peutTirer = true;
        muzzlePrefab.SetActive(false);
    }
    
}
