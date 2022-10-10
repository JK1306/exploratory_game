using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    public float movementSpeed;
    public Transform movablePlatform,
                    rightPoint,
                    leftPoint;
    bool moveRight;
    void Start()
    {
        moveRight = true;
    }

    void Update()
    {
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
    }
}
