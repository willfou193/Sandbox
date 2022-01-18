using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject hitPrefab;
    public AudioClip inpact;
    public GameObject joueur;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 4f);
      
    }

    void OnCollisionEnter(Collision other) {
        joueur.GetComponent<AudioSource>().PlayOneShot(inpact);
        ContactPoint contact = other.contacts [0];
        Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if(hitPrefab != null){
            GameObject hitVFX = Instantiate (hitPrefab,pos,rot);
            hitVFX.SetActive(true);
            Destroy(hitVFX,0.5f);
        }
        
        Destroy(gameObject);
    }
}
