using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    public float movementSpeed;
    public MovableAxis axis;
    public bool printDebug;
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
                movablePlatform.position = (movablePlatform.position + ((Vector3.right * movementSpeed) * Time.deltaTime));
                if(Vector3.Distance(movablePlatform.position, rightPoint.position) < 1f){
                    moveRight = false;
                }
            }else{
                movablePlatform.position = (movablePlatform.position + ((Vector3.left * movementSpeed) * Time.deltaTime));
                if(Vector3.Distance(movablePlatform.position, leftPoint.position) < 1f){
                    moveRight = true;
                }
            }
        }else{
            if(moveUp){
                movablePlatform.position = (movablePlatform.position + ((Vector3.up * movementSpeed) * Time.deltaTime));
                if(Vector3.Distance(movablePlatform.position, rightPoint.position) < 1f){
                    moveUp = false;
                }
            }else{
                movablePlatform.position = (movablePlatform.position + ((Vector3.down * movementSpeed) * Time.deltaTime));
                if(Vector3.Distance(movablePlatform.position, leftPoint.position) < 1f){
                    moveUp = true;
                }
            }
        }
    }
}

public enum MovableAxis{
    Vertical,
    Horizontal
}