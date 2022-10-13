using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    public float movementSpeed;
    public MovableAxis axis;
    public Transform movablePlatform,
                    rightPoint,
                    leftPoint;
    bool moveRight,
        moveUp;
    void Start()
    {
        moveRight = true;
        moveUp = true;
    }

    void Update()
    {
        if(axis == MovableAxis.Horizontal){
            if(moveRight){
                movablePlatform.position = (movablePlatform.position + (Vector3.right * movementSpeed));
                if(Vector3.Distance(movablePlatform.position, rightPoint.position) < 1f){
                    moveRight = false;
                }
            }else{
                movablePlatform.position = (movablePlatform.position + (Vector3.left * movementSpeed));
                if(Vector3.Distance(movablePlatform.position, leftPoint.position) < 1f){
                    moveRight = true;
                }
            }
        }else{

        }
    }
}

public enum MovableAxis{
    Vertical,
    Horizontal
}