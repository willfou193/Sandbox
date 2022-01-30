using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventaire;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int Y_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    private void Start() {
        CreateDisplay();
    }
    private void Update() {
        UpdateDisplay();
    }

    public void CreateDisplay(){
        for(int i = 0; i < inventaire.Container.Count; i++){
            var obj = Instantiate(inventaire.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventaire.Container[i].amount.ToString("n0");
            itemsDisplayed.Add(inventaire.Container[i], obj);
        }
    }
    public void UpdateDisplay(){
         for(int i = 0; i < inventaire.Container.Count; i++){
             if(itemsDisplayed.ContainsKey(inventaire.Container[i])){
                 itemsDisplayed[inventaire.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventaire.Container[i].amount.ToString("n0");
             }else{
                 var obj = Instantiate(inventaire.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
                 obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                 obj.GetComponentInChildren<TextMeshProUGUI>().text = inventaire.Container[i].amount.ToString("n0");
                 itemsDisplayed.Add(inventaire.Container[i], obj);
             }
         }
    }
    public Vector3 GetPosition(int i){
        return new Vector3(X_START+(X_SPACE_BETWEEN_ITEM *(i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEM * (i/NUMBER_OF_COLUMN)), 0f);
    }
}
