using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainGameController : MonoBehaviour
{
    public List<CollectableObject> collectables;
    public static MainGameController instance;
    public GameObject inventoryPanel,
                    inventoryObjects,
                    inventory;
    // public Text scoreBoard;
    public Button inventoryButton,
                    feedBtn;
    public Button cancelBtn;
    public AudioClip fruitGainAudio,
                    elephantAudio,
                    monkeyAudio,
                    snakeAudio;
    public bool animalNearby;
    public int successPoint,
                negativePoint;
    public string animalName;
    public AnimalController nearbyAnimalContorller;
    public GameObject nearByAnimal;
    public GameObject monkeyScore,
                        bisonScore,
                        lionessScore;
    GameObject spawnObject;
    GameObject foodToFeed;
    int monkeyScoreVal,
        bisonScoreVal,
        lionessScoreVal;
    int score;

    void Start()
    {
        instance = this;
        successPoint = 5;
        negativePoint = 3;
        score = 0;
        // scoreBoard.text = "Score : "+score.ToString();
    }

    private void OnEnable() {
        inventoryButton.onClick.AddListener(ViewInventory);
        cancelBtn.onClick.AddListener(CloseInventory);
        feedBtn.onClick.AddListener(PerformFeedAnimal);
    }

    private void OnDisable() {
        inventoryButton.onClick.RemoveListener(ViewInventory);
        cancelBtn.onClick.RemoveListener(CloseInventory);
        feedBtn.onClick.RemoveListener(PerformFeedAnimal);
    }

    void PerformFeedAnimal(){

        if(!animalNearby){
            Debug.Log("can't feed food");
            return;
        }

        foodToFeed = inventory.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault().gameObject;
        Debug.Log(foodToFeed.name);
        Debug.Log(foodToFeed.GetComponent<CollectableController>());
        if(foodToFeed.name.ToLower().Contains("apple")){
            FeedFruit(Collectables.Apple);
        }
        if(foodToFeed.name.ToLower().Contains("bannana")){
            FeedFruit(Collectables.Bannana);
        }
        if(foodToFeed.name.ToLower().Contains("berry")){
            FeedFruit(Collectables.Berry);
        }
        if(foodToFeed.name.ToLower().Contains("grape")){
            FeedFruit(Collectables.Grape);
        }
        if(foodToFeed.name.ToLower().Contains("meat")){
            FeedFruit(Collectables.Meat);
        }
        if(foodToFeed.name.ToLower().Contains("orange")){
            FeedFruit(Collectables.Orange);
        }
        if(foodToFeed.name.ToLower().Contains("pineapple")){
            FeedFruit(Collectables.Pineapple);
        }

        CloseInventory();
    }

    void ViewInventory(){
        inventoryPanel.SetActive(true);
        foreach (var item in collectables)
        {
            if(item.collectionCount > 0){
                spawnObject = Instantiate(inventoryObjects, inventory.transform);
                spawnObject.name = item.collectableType.ToString();
                spawnObject.GetComponent<Toggle>().group = inventory.GetComponent<ToggleGroup>();
                spawnObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "x" + item.collectionCount.ToString();
                spawnObject.transform.GetChild(2).gameObject.SetActive(true);
                spawnObject.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = item.collectable;
            }
        }
    }

    void ManageScore(EatableType collectableCategory){
        bool condition1 = (nearbyAnimalContorller.animalCategory == AnimalType.Carnivores && collectableCategory == EatableType.Meat);
        bool condition2 = (nearbyAnimalContorller.animalCategory == AnimalType.Herbivores && (collectableCategory == EatableType.Grass || collectableCategory == EatableType.Fruit));
        bool condition3 = (nearbyAnimalContorller.animalCategory == AnimalType.Omnivores && (collectableCategory == EatableType.Fruit || collectableCategory == EatableType.Meat));

        if(condition1 || condition2 || condition3){
            nearbyAnimalContorller.PlayAnimalSFX();
            nearbyAnimalContorller.AddScore();
            // score += successPoint;
        }else{
            // score -= negativePoint;
        }
        // scoreBoard.text = "Score : "+score.ToString();
    }

    void CloseInventory(){
        inventoryPanel.SetActive(false);
        for(int i=0; i<inventory.transform.childCount; i++){
            Destroy(inventory.transform.GetChild(i).gameObject);
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

    void FeedFruit(Collectables collectable){
        foreach (var item in collectables){
            if(item.collectableType == collectable){
                item.collectionCount--;
                // foodToFeed = 
                ManageScore(item.collectableCategory);
                break;
            }
        }
    }
}

[System.Serializable]
public class CollectableObject{
    public Sprite collectable;
    public Collectables collectableType;
    public EatableType collectableCategory;
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
