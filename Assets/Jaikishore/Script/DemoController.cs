using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    public GameObject demoMenu,
                        demoPlay,
                        mainMenu,
                        mainGame;
    public List<GameObject> disableGameObjects;
    public List<DemoClass> demoList;
    public GameObject mainGameObject;
    public GameObject buttonPressHighlighter;
    public GameObject leftMoveInstruction,
                    rightMoveInstruction;
    public float practiceTime;
    public DemoClass currentDemo;
    float time;
    [SerializeField] int index;

    void Start()
    {
        time = 0;
        // index = 0;
        currentDemo = demoList[index];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey){
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
                    GetFood();
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
            DisableCurrentDemo();
            currentDemo = demoList[++index];
            ChangeDemo();
        }
    }

    void GetFood(){
        foreach(var food in MainGameController.instance.collectables){
            if(food.collectionCount > 0){
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
            currentDemo = demoList[++index];

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