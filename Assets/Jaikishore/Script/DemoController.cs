using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class DemoController : MonoBehaviour
{
    public GameObject demoMenu,
                        demoPlay,
                        mainMenu,
                        mainGame;
    public static DemoController instance;
    public Button inventoryButton;
    public List<GameObject> disableGameObjects;
    public List<DemoClass> demoList;
    public GameObject mainGameObject;
    public GameObject buttonPressHighlighter;
    public GameObject leftMoveInstruction,
                    rightMoveInstruction;
    public float practiceTime;
    public DemoClass currentDemo;
    public bool inventoryOpened,
                animalFeedReset,
                feedClicked;
    float time;
    bool foodCollected,
        animalFeed,
        foodClicked;
    [SerializeField] int index;

    void Start()
    {
        time = 0;
        instance = this;
        animalFeed = false;
        feedClicked = false;
        foodClicked = false;
        foodCollected = false;
        inventoryOpened = false;
        // index = 0;
        currentDemo = demoList[index];
        // PlayerController.instance.isPlayingDemo = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey || animalFeed){
            switch(currentDemo.demo){
                case Demo.RighMove:
                    PlayMovementDemo(KeyCode.RightArrow);
                    break;
                case Demo.LeftMove:
                    PlayMovementDemo(KeyCode.LeftArrow);
                    break;
                case Demo.Jump:
                    PlayMovementDemo(KeyCode.Space);
                    break;
                case Demo.JumpThroughFloor:
                    JumpThroughFloor();
                    break;
                case Demo.CollectFood:
                    PlayGetFoodDemo();
                    break;
                case Demo.AnimalFeed:
                    PlayAnimalFeedDemo();
                    break;
            }
        }
    }

    void PlayMovementDemo(KeyCode keyPressed){
        if(currentDemo.demoPlayGameObject) currentDemo.demoPlayGameObject.SetActive(true);
        if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(true);

        if(Input.GetKey(keyPressed)){
            time += Time.deltaTime;
            if(keyPressed == KeyCode.Space) time += 0.1f;
        }

        if(time > practiceTime){
            time = 0;

            if(currentDemo.demoPlayGameObject) currentDemo.demoPlayGameObject.SetActive(false);
            if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(false);

            ChangeDemo();
        }
    }

    void JumpThroughFloor(){
        if(PlayerController.instance.inFloatingFloor){
            // DisableCurrentDemo();
            if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(false);
            Debug.Log("Landed in Floating floor ---> ");
            // currentDemo = demoList[++index];
            ChangeDemo();
        }
    }

    void PlayGetFoodDemo(){
        if(!foodCollected){
            GetFood();
        }else{
            if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(false);

            if(!mainMenu.activeSelf){
                mainMenu.SetActive(true);
                DisableMainMenuObjects();
                ChangeDemo();
            }
        }
    }

    public void InventoryCancelBtn(){
        currentDemo.demoPlayMenuObject.transform.GetChild(4).gameObject.SetActive(false);
    }

    public void AnimalFeedReset(){
        foodClicked = false;
        feedClicked = false;
        inventoryOpened = false;
        currentDemo.demoPlayMenuObject.transform.GetChild(3).gameObject.SetActive(false);
        currentDemo.demoPlayMenuObject.transform.GetChild(4).gameObject.SetActive(true);
    }

    void PlayAnimalFeedDemo(){
        if(Input.anyKey && time < practiceTime){
            animalFeed = true;
            time += (Time.deltaTime);
        }else{
            if(currentDemo.demoPlayMenuObject.transform.GetChild(0).gameObject.activeSelf){
                currentDemo.demoPlayMenuObject.transform.GetChild(0).gameObject.SetActive(false);
            }

            if(feedClicked){
                currentDemo.demoPlayMenuObject.transform.GetChild(3).gameObject.SetActive(false);
            }

            if(Input.GetMouseButtonDown(0)){
                if(MainGameController.instance.inventory.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault()){
                    foodClicked = true;
                    currentDemo.demoPlayMenuObject.transform.GetChild(2).gameObject.SetActive(false);
                    currentDemo.demoPlayMenuObject.transform.GetChild(3).gameObject.SetActive(true);
                    return;
                }
            }

            if(foodClicked) return;

            if(inventoryOpened){
                currentDemo.demoPlayMenuObject.transform.GetChild(1).gameObject.SetActive(false);
                currentDemo.demoPlayMenuObject.transform.GetChild(2).gameObject.SetActive(true);
                return;
            }

            if(MainGameController.instance.nearbyAnimalContorller && !currentDemo.demoPlayMenuObject.transform.GetChild(1).gameObject.activeSelf){
                currentDemo.demoPlayMenuObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    void DisableMainMenuObjects(){
        foreach (var item in disableGameObjects)
        {
            item.SetActive(false);
        }
    }

    void GetFood(){
        foreach(var food in MainGameController.instance.collectables){
            if(food.collectionCount > 0){
                foodCollected = true;
                Debug.Log(food.collectableType);
                currentDemo.demoPlayGameObject.SetActive(false);
                break;
            }
        }
    }

    void DisableCurrentDemo(){
        if(currentDemo.demoPlayGameObject) currentDemo.demoPlayGameObject.SetActive(false);
        if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(false);
    }

    void ChangeDemo(){
        if((index + 1) >= demoList.Count) {
            // Reached Last demo
            Debug.Log("Reached Last demo");
            return;
        }
        currentDemo = demoList[++index];
        Debug.Log("Index : "+index);
        if(currentDemo.demoPlayGameObject) currentDemo.demoPlayGameObject.SetActive(true);
        if(currentDemo.demoPlayMenuObject) currentDemo.demoPlayMenuObject.SetActive(true);
    }

    public void StartMainGame(){
        demoMenu.SetActive(false);
        demoPlay.SetActive(false);
        mainMenu.SetActive(true);
        mainGame.SetActive(true);
        CameraHandler.OBJ_followingCamera.B_canfollow = true;
    }
}

[System.Serializable]
public class DemoClass{
    public Demo demo,
                nextDemo;
    public GameObject demoPlayGameObject,
                        demoPlayMenuObject;
}

public enum Demo{
    None,
    LeftMove,
    RighMove,
    Jump,
    JumpThroughFloor,
    CollectFood,
    AnimalFeed,
    PlayerDeath
}