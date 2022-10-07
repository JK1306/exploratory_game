using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    public List<CollectableObject> collectables;
    public static MainGameController instance;
    public GameObject inventoryPanel,
                    inventoryObjects,
                    inventory;
    public Text scoreBoard;
    public Button inventoryButton;
    public Button cancelBtn;
    GameObject spawnObject;

    void Start()
    {
        instance = this;
        inventoryButton.onClick.AddListener(ViewInventory);
        cancelBtn.onClick.AddListener(CloseInventory);
    }

    void ViewInventory(){
        inventoryPanel.SetActive(true);
        foreach (var item in collectables)
        {
            if(item.collectionCount > 0){
                spawnObject = Instantiate(inventoryObjects, inventory.transform);
                spawnObject.GetComponent<Image>().sprite = item.collectable;
                spawnObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "x" + item.collectionCount.ToString();
            }
        }
    }

    void CloseInventory(){
        inventoryPanel.SetActive(false);
        for(int i=0; i<inventory.transform.childCount; i++){
            Destroy(inventory.transform.GetChild(i).gameObject);
            Debug.Log(i +" Child Count : "+inventory.transform.childCount);
        }
    }

    public void CollectCollectable(Collectables collectable){
        foreach (var item in collectables)
        {
            if(item.collectableType == collectable){
                item.collectionCount++;
                break;
            }
        }
    }
}

[System.Serializable]
public class CollectableObject{
    public Sprite collectable;
    public Collectables collectableType;
    public int collectionCount;
}

public enum Collectables{
    Apple,
    Bannana,
    Berry,
    Grape,
    Meat,
    Orange,
    Pineapple
}