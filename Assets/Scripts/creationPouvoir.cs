using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class creationPouvoir : MonoBehaviour
{
    public Attaques attaque;
    public TextMeshProUGUI texteAttaque;
    public TextMeshProUGUI texteDegatsEnnemis;
    public TextMeshProUGUI texteDegatsJoueur;
    public Image spriteImage;

    void Start()
    {
        //Nom et valeurs de l'attaque
        texteAttaque.text = attaque.nom;
        texteDegatsEnnemis.text = attaque.DegatsEnnemi.ToString();
        texteDegatsJoueur.text = attaque.DegatsJoueur.ToString();

        //Artwork de l'attaque
        spriteImage.sprite = attaque.icone;
    }
}
