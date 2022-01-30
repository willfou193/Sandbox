using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventaireJoueur : MonoBehaviour
{
    public InventoryObject inventaire;

    public void OnTriggerEnter(Collider other) {
        var item = other.GetComponent<Item>();
        if(item){
            inventaire.AddItem(item.item, 1);
            Destroy(other.gameObject);
        }
    }
    private void OnApplicationQuit() {
    inventaire.Container.Clear();
}
}


