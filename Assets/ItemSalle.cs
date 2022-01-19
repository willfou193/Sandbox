using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ItemSalle : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI nomSalle;
    GestionConnexion gestionConnexion;

    private void Start()
    {
        gestionConnexion = FindObjectOfType<GestionConnexion>();
    }
    public void determinerNomSalle(string _nomSalle)
    {
        nomSalle.text = _nomSalle;
    }

    public void clickSalle()
    {
        gestionConnexion.JoindreSalle(nomSalle.text);
    }
}
