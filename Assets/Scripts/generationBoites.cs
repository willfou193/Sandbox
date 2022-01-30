using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class generationBoites : MonoBehaviour
{
    //Génération de boites
    public GameObject boitePrefab; //Prefab de la boite
    public Vector3 positionDepart; //Position de départ de la boite
    public bool antiSpam; //Empâcher le spam

    //Timer
    public float tempsPartie; //Temps du jeu
    public float tempsFin; //Temps max
    public TextMeshProUGUI countdownTexte; //Référence au deathtimer
    public static bool finJeu;
    public AudioClip sonFinJeu;

    //Score
    public TextMeshProUGUI texteScore; //Texte affichant le score du joueur
    public GameObject[] tableauScore; //Tableau comptant toutes les boîtes
    public TextMeshProUGUI highScore; //HighScore

    void Start()
    {
        spawnBoite();
        texteScore.gameObject.SetActive(false);

        //Montrer le highscore
        highScore.text = PlayerPrefs.GetInt("highScore", 0).ToString();
    }


    void Update()
    {
        if(finJeu == false)
        {
            tempsPartie -= 1 * Time.deltaTime;
            countdownTexte.text = tempsPartie.ToString("0");
            if (Input.GetKeyDown(KeyCode.Space) && antiSpam == false)
            {
                Invoke("spawnBoite", 0.5f);
                antiSpam = true;
            }

            if (tempsPartie <= 0)
            {
                finJeu = true;
                tempsPartie = tempsFin;
                countdownTexte.gameObject.SetActive(false);
                texteScore.gameObject.SetActive(true);
                tableauScore = GameObject.FindGameObjectsWithTag("boite");
                texteScore.text = "Score : " + tableauScore.Length.ToString();
                GetComponent<AudioSource>().PlayOneShot(sonFinJeu);

                //Highscore
                if(tableauScore.Length > PlayerPrefs.GetInt("highScore", 0))
                {
                    PlayerPrefs.SetInt("highScore", tableauScore.Length);
                    highScore.text = tableauScore.Length.ToString();
                }
            }
        }
    }

    public void spawnBoite()
    {
        if(finJeu == false)
        {
            GameObject nouvelleBoite = Instantiate(boitePrefab, positionDepart, Quaternion.identity);
            antiSpam = false;
        }
    }

    public void resetHighScore()
    {
        PlayerPrefs.DeleteAll();
        highScore.text = "0";
    }
}
