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
    public TMP_InputField ChampCreerPartie; //Champ de texte du nom de la partie lors de la cr�ation
    public TextMeshProUGUI nomSalle; //Nom de la salle
    public GameObject ConnexionServeur; //Interface de connexion (premier �cran)
    public GameObject InterfaceLobby; //Interface du lobby avec cr�er et joindre
    public GameObject InterfaceSalle; //Interface lorsqu'on a rejoint une salle
    RoomOptions roomOptions = new RoomOptions(); //Param�tres de la salle
    public ItemSalle itemSallePrefab; //Prefab d'un nom de salle � instancier
    List<ItemSalle> listeItemsSalles = new List<ItemSalle>(); //Liste de salles
    public Transform contentObjet; //Conteneur de la liste des salles
    public float tempsRafraichissement = 1f; //D�lai pour �viter un bug
    float prochainRafraichissement; //D�lai pour �viter un bug
    public List<ItemPerso> playerItemList = new List<ItemPerso>(); //Liste des cartes de joueur
    public ItemPerso itemPersoPrefab; //Prefab de la carte du parent
    public Transform itemPersoParent; //Prefab li� � la carte de joueur
    public GameObject boutonJouer; //Bouton qui permet de d�marer la partie


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

    //Quand il est connect� au serveur...
    public override void OnConnectedToMaster()
    {
        //Joindre le Lobby
        PhotonNetwork.JoinLobby();

        //Activer l'interface du Lobby
        InterfaceLobby.SetActive(true);

        //D�sactiver l'interface de connexion
        ConnexionServeur.SetActive(false);
    }

    //Cr�er une partie avec un bouton
    public void CreerPartie()
    {
        //D�terminer les options de la salle (RoomOptions)
        roomOptions.MaxPlayers = 4;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        if (ChampCreerPartie.text.Length >= 1)
        {
            //Cr�er la salle avec les bonnes propri�t�s
            PhotonNetwork.CreateRoom(ChampCreerPartie.text, roomOptions, null);
        }
    }

    //Quand on rejoint une salle...
    public override void OnJoinedRoom()
    {
        //D�sactiver l'interface du Lobby
        InterfaceLobby.SetActive(false);

        //Activer la salle d'attente
        InterfaceSalle.SetActive(true);

        //Nom de la salle
        nomSalle.text = "Nom de la salle: " + PhotonNetwork.CurrentRoom.Name;

        //Rafraichir la liste de joueurs
        RafraichirListeJoueurs();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Rafraichir la liste de joueurs
        RafraichirListeJoueurs();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Rafraichir la liste de joueurs
        RafraichirListeJoueurs();
    }

    //Lorsque la liste des salles se fait rafra�chir par Photon...
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= prochainRafraichissement)
        {
            RafraichirListeSalles(roomList);
            prochainRafraichissement = Time.time + tempsRafraichissement;
        }
    }

    //Lorsque la liste des salles se fait rafra�chir par Photon, cette fonction est appel�e...
    public void RafraichirListeSalles(List<RoomInfo> list)
    {
        //D�truire toutes les salles dans la liste des salles
        foreach (ItemSalle item in listeItemsSalles)
        {
            Destroy(item.gameObject);
        }
        listeItemsSalles.Clear();

        //Instancier un prefab d'une salle et int�grer le bon nom, soit celui de la salle cr��e
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

    //Rafrahichir la liste des joueurs dans la salle
    void RafraichirListeJoueurs()
    {
        //Tout d�truire
        foreach(ItemPerso item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        //Si il n'y a plus personne dans la salle, renvoyer la fonction
        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        //Pour chaque joueur dans la salle, instancier une nouvelle carte de joueur,
        //et l'ajouter dans la liste
        foreach(KeyValuePair<int, Player> joueur in PhotonNetwork.CurrentRoom.Players)
        {
            ItemPerso newPlayerItem = Instantiate(itemPersoPrefab, itemPersoParent);
            newPlayerItem.SetPlayerInfo(joueur.Value);
            playerItemList.Add(newPlayerItem);

            //V�rifier si c'est le joueur local
            if (joueur.Value == PhotonNetwork.LocalPlayer)
            {
                //Permettre de faire des changements
                newPlayerItem.ApplyLocalChanges();
            }
        }
    }

    private void Update()
    {
        //Seulement afficher le bouton permettatnt de commencer la partie � celui qui a cr�� la salle
        if (PhotonNetwork.IsMasterClient == true && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            boutonJouer.SetActive(true);
        }
        //Sinon, le d�sactiver
        else
        {
            boutonJouer.SetActive(false);
        }

        //Raccourci avec ENTER pour se connecter
        if (Input.GetKeyDown(KeyCode.Return) && ConnexionServeur.activeSelf)
        {
            ConnexionLobby();
        }

        //Raccourci avec ENTER pour cr�er une partie
        if (Input.GetKeyDown(KeyCode.Return) && InterfaceLobby.activeSelf)
        {
            CreerPartie();
        }
    }

    //Fonction appel�e lorsque l'on clique sur le bouton Jouer
    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("Tristan");
    }
}
