using UnityEngine;

public class textLookAt : MonoBehaviour
{
    public Transform joueur;
    void Update()
    {
        float dist = Vector3.Distance(transform.position, joueur.position); // distance entre le joueur et le texte
        if(dist < 5f){
            gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.transform.LookAt(joueur);   
        }
        else{
             gameObject.GetComponent<Renderer>().enabled = false;
        }

        
    }
}
