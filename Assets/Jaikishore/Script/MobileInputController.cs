using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool movePlayer;
    public MovementType movementType;
    public float movementDirection;
    private void Awake() {
        movePlayer = false;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(eventData.selectedObject.gameObject.CompareTag("GameController")){
            if(movementType == MovementType.Horizontal){
                PlayerController.instance.movementDirection = movementDirection;
            }
            if(movementType == MovementType.Vertical){
                PlayerController.instance.jump = true;
            }
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(eventData.selectedObject.gameObject.CompareTag("GameController")){
            if(movementType == MovementType.Horizontal){
                PlayerController.instance.movementDirection = 0;
            }
            if(movementType == MovementType.Vertical){
                PlayerController.instance.jump = false;
            }
        }
    }
}

public enum MovementType{
    Horizontal,
    Vertical
}