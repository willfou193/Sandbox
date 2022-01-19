using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestionConnexion : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI champNomJoueurTexte; //Champ de texte pour le nom du joueur
    public TextMeshProUGUI texteBoutonConnexion; //Texte du bouton qui permet de se connecter au lobby
    public TMP_InputField ChampCreerPartie; //Champ de texte du nom de la partie lors de la création
    public TextMeshProUGUI nomSalle; //Nom de la salle
    public GameObject ConnexionServeur; //Interface de connexion (premier écran)
    public GameObject InterfaceLobby; //Interface du lobby avec créer et joindre
    public GameObject InterfaceSalle; //Interface lorsqu'on a rejoint une salle
    RoomOptions roomOptions = new RoomOptions(); //Paramètres de la salle
    public ItemSalle itemSallePrefab; //Prefab d'un nom de salle à instancier
    List<ItemSalle> listeItemsSalles = new List<ItemSalle>(); //Liste de salles
    public Transform contentObjet; //Conteneur de la liste des salles
    public float tempsRafraichissement = 1.5f; //Délai pour éviter un bug
    float prochainRafraichissement; //Délai pour éviter un bug


    public void ConnexionLobby()
    {
        //Seulement si le champ n'est pas vide
        if (champNomJoueurTexte.text.Length > 1)
        {
            texteBoutonConnexion.text = "Connexion...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LocalPlayer.NickName = champNomJoueurTexte.text;
        }
    }

    //Quand il est connecté au serveur...
    public override void OnConnectedToMaster()
    {
        //Joindre le Lobby
        PhotonNetwork.JoinLobby();

        //Activer l'interface du Lobby
        InterfaceLobby.SetActive(true);

        //Désactiver l'interface de connexion
        ConnexionServeur.SetActive(false);
    }

    //Créer une partie avec un bouton
    public void CreerPartie()
    {
        //Déterminer les options de la salle (RoomOptions)
        roomOptions.MaxPlayers = 5;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        if (ChampCreerPartie.text.Length >= 1)
        {
            //Créer la salle avec les bonnes propriétés
            PhotonNetwork.CreateRoom(ChampCreerPartie.text, roomOptions, null);
        }
    }

    //Quand on rejoint une salle...
    public override void OnJoinedRoom()
    {
        //Storer le texte d'attente dans la variable afin de l'afficher
        //TexteAttente.text = "Ton ami va se connecter d'ici peu...";

        //Désactiver l'interface du Lobby
        InterfaceLobby.SetActive(false);

        //Activer la salle d'attente
        InterfaceSalle.SetActive(true);

        //Nom de la salle
        nomSalle.text = "Nom de la salle: " + PhotonNetwork.CurrentRoom.Name;

    }

    //Lorsque la liste des salles se fait rafraîchir par Photon...
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= prochainRafraichissement)
        {
            RafraichirListeSalles(roomList);
            prochainRafraichissement = Time.time + tempsRafraichissement;
        }
    }

    //Lorsque la liste des salles se fait rafraîchir par Photon, cette fonction est appelée...
    public void RafraichirListeSalles(List<RoomInfo> list)
    {
        //Détruire toutes les salles dans la liste des salles
        foreach (ItemSalle item in listeItemsSalles)
        {
            Destroy(item.gameObject);
        }
        listeItemsSalles.Clear();

        //Instancier un prefab d'une salle et intégrer le bon nom, soit celui de la salle créée
        foreach (RoomInfo room in list)
        {
            if (room.RemovedFromList)
            {
                return;
            }
            ItemSalle newRoom = Instantiate(itemSallePrefab, contentObjet);
            newRoom.determinerNomSalle(room.Name);
            listeItemsSalles.Add(newRoom);
        }
    }

    //Bouton pour rejoindre une salle. Utiliser le nom du bouton de la salle
    public void JoindreSalle(string nomSalle)
    {
        PhotonNetwork.JoinRoom(nomSalle);
    }

    //Bouton pour quitter la salle
    public void QuitterSalle()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Quand on a quitter la salle...
    public override void OnLeftRoom()
    {
        //Retourner au lobby
        InterfaceSalle.SetActive(false);
        InterfaceLobby.SetActive(true);
    }
}
