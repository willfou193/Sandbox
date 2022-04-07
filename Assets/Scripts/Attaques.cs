using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Attaques", fileName = "Attaque", order = 1)]
public class Attaques : ScriptableObject
{
    public string nom;
    public Sprite icone;
    public float DegatsEnnemi;
    public float DegatsJoueur;
    public GameObject particuleAttaque;

    private void OnValidate()
    {
        //Si le nom entré est vide
        if (string.IsNullOrEmpty(nom))
        {
            //Le nom par défaut devient le nom de la classe.
            nom = this.name;
        }

        //Si la valeur d'une attaque est trop basse
        if(DegatsEnnemi < 3)
        {
            //Faire apparaître un message dans la console.
            Debug.Log($"<color=orange>{nom} est trop faible!</color>");
        }
    }

    public void Print()
    {
        Debug.Log(nom + " fait " + DegatsEnnemi + " dégâts aux ennemis.");
    }
}
