using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ItemPerso : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI nomJoueur; //Nom du joueur
    Image backgroundImage; //Fond de la carte de joueur
    public Color highlightColor; //Couleur du fond de la carte de joueur
    public GameObject leftArrowButton; //Fl�che gauche de la carte de joueur
    public GameObject rightArrowButton; //Fl�che droite de la carte de joueur
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable(); //Tableau Photon de propri�t�s du joueur
    public Image playerAvatar; //Avatar de joueur
    public Sprite[] avatars; //Tableau de sprites des avatars disponibles
    Player player; //Joueur premade par photon

    
    private void Awake()
    {
        //Shortcut
        backgroundImage = GetComponent<Image>();
    }

    public void SetPlayerInfo(Player _player)
    {
        //Indiquer que le texte du joueur est aussi le vrai Plyaer/NickName du joueur
        nomJoueur.text = _player.NickName;

        //Associer le joueur au joueur de photon
        player = _player;

        //Rafraichir la liste des joueurs avec le joueur en question
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        backgroundImage.color = highlightColor;
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        //Si le sprite est 0, aller vers la gauche et diminuer de 1
        if((int)playerProperties["playerAvatar"] == 0)
        {
            playerProperties["playerAvatar"] = avatars.Length - 1;
        } 
        //Sinon, diminuer de 1 la valeur pr�sente
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;
        }
        //Indiquer ces choix � Photon
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        
        //Si l'avatar choisi est 1 de moins que le nombre d'avatars, le mettre � 0
        if ((int)playerProperties["playerAvatar"] == avatars.Length - 1)
        {
            playerProperties["playerAvatar"] = 0;
        }
        //Sinon, l'incr�menter de 1
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
        }
        //Indiquer ces choix � Photon
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    //Appeler � tout le monde quand une propri�t� de l'array a �t� chang�
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //Si le joueur est celui dont lequel on modifie ses propri�t�s
        if (player == targetPlayer)
        {
            //Appeler la fonction de rafraichissement
            UpdatePlayerItem(targetPlayer);
        }
    }

    //Fonction qui permet de rafra�chir la carte du joueur qui modifie ses propri�t�s
    void UpdatePlayerItem(Player player)
    {
        //S'assurer que le joueur a bien la propri�t� playerAvatar
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            //Changer le sprite
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];

            //Garder en m�moire quel avatar a �t� choisi
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        //S'il n'en avait pas, juste le mettre � 0
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}
