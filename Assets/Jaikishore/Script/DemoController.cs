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
                    rightMoveInstruction,
                    instructionPanel;
    public AudioClip instructionAudio;
    public float practiceTime;
    public DemoClass currentDemo;
    public bool inventoryOpened,
                animalFeedReset,
                feedClicked;
    public PlayerController player;
    float time;
    bool foodCollected,
        animalFeed,
        foodClicked,
        playerInputDisabled,
        demoCompleted,
        playerBlockedInput,
        demoAudioPlayed;
    [SerializeField] int index;

    void Start()
    {
        time = 0;
        instance = this;
        animalFeed = false;
        feedClicked = false;
        foodClicked = false;
        demoCompleted = false;
        foodCollected = false;
        inventoryOpened = false;
        playerInputDisabled = false;
        demoAudioPlayed = false;
        playerBlockedInput = false;
        // index = 0;
        currentDemo = demoList[index];
    }

    // private void OnEnable() {
    //     player.LockInput();
    //     Debug.Log("Input Locked");
    // }

    // Update is called once per frame
    void Update()
    {
        if(currentDemo.demo == Demo.None){ return; }
        // Lock Play Input in Demo while starting the game input 
        if(!playerInputDisabled){
            player.LockInput();
            playerInputDisabled = true;
        }

        if(currentDemo.demoPlayMenuObject.activeSelf && !demoAudioPlayed && !player.blockInput){
            CameraHandler.OBJ_followingCamera.gameObject.GetComponent<AudioSource>().clip = currentDemo.demoInstructionAudio;
            CameraHandler.OBJ_followingCamera.gameObject.GetComponent<AudioSource>().Play();
            demoAudioPlayed = true;
        }

        if(EventSystem.current.currentSelectedGameObject){
            if(EventSystem.current.currentSelectedGameObject.name == "CoverPageStart" && !playerBlockedInput){
                player.UnLockInput();
                playerBlockedInput = true;
            }
        }

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
        if(player.blockInput) return;
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
        if(player.inFloatingFloor){
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
        if(currentDemo.demo != Demo.None){
            if(currentDemo.demoPlayMenuObject.transform.childCount >= 4) currentDemo.demoPlayMenuObject.transform.GetChild(4).gameObject.SetActive(false);
        }
    }

    // public void AnimalFeedReset(){
    //     foodClicked = false;
    //     feedClicked = false;
    //     inventoryOpened = false;
    //     currentDemo.demoPlayMenuObject.transform.GetChild(3).gameObject.SetActive(false);
    //     currentDemo.demoPlayMenuObject.transform.GetChild(4).gameObject.SetActive(true);
    // }

    void PlayAnimalFeedDemo(){
        if(!MainGameController.instance.nearbyAnimalContorller){
            animalFeed = true;
        }else{
            if(currentDemo.demoPlayMenuObject.transform.GetChild(0).gameObject.activeSelf){
                player.LockInput();
                currentDemo.demoPlayMenuObject.transform.GetChild(0).gameObject.SetActive(false);
            }

            if(feedClicked){
                currentDemo.demoPlayMenuObject.transform.GetChild(3).gameObject.SetActive(false);
                Invoke(nameof(StartMainGame), 1f);
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

    public void PlayInstructionAudio(){
        CameraHandler.OBJ_followingCamera.gameObject.GetComponent<AudioSource>().clip = instructionAudio;
        CameraHandler.OBJ_followingCamera.gameObject.GetComponent<AudioSource>().Play();
    }

    public void StopInstructionAudio(){
        CameraHandler.OBJ_followingCamera.gameObject.GetComponent<AudioSource>().Stop();
    }

    void DisableMainMenuObjects(){
        foreach (var item in disableGameObjects)
        {
            item.SetActive(false);
        }
    }

    void EnableMainMenuObjects(){
        foreach (var item in disableGameObjects)
        {
            item.SetActive(true);
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
        demoAudioPlayed = false;
    }

    public void StartMainGame(){
        demoCompleted = true;
        instructionPanel.SetActive(true);

        PlayInstructionAudio();

        demoMenu.SetActive(false);
        demoPlay.SetActive(false);
        mainMenu.SetActive(true);
        mainGame.SetActive(true);
        EnableMainMenuObjects();
        MainGameController.instance.playMode = PlayMode.MainGame;
        MainGameController.instance.ResetScoring();
        CameraHandler.OBJ_followingCamera.B_canfollow = true;
        currentDemo = demoList[demoList.Count - 1];
    }
}

[System.Serializable]
public class DemoClass{
    public Demo demo,
                nextDemo;
    public GameObject demoPlayGameObject,
                        demoPlayMenuObject;
    public AudioClip demoInstructionAudio;
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