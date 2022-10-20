using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    public float movementSpeed;
    public MovableAxis axis;
    public bool applyFloat,
                applyMovable;
    public bool printDebug;
    public Transform movablePlatform,
                    rightPoint,
                    leftPoint;
    [SerializeField] float floatEffectFrequency,
                            floatEffectAmplitude,
                            floatEffectTime;
    bool moveRight,
        moveUp;
    float bufferVal;
    Vector3 tempPos, posOffset;

    void Start()
    {
        moveRight = true;
        moveUp = true;
        bufferVal = floatEffectAmplitude;
        posOffset = movablePlatform.transform.position;
    }

    void Update()
    {
        if(applyMovable){
            BlockMovement();
        }
    }

    public void ApplyFloatEffect(){
        bufferVal = floatEffectAmplitude;
        if(applyFloat) StartCoroutine(StartFloatEffect());
    }

    // public void ResetFloatEffect(){
    //     bufferVal = floatEffectAmplitude;
    // }

    void BlockMovement(){
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
        } 
        else {
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

    IEnumerator StartFloatEffect(){
        while(bufferVal >= 0){
            FloatEffect();
            bufferVal -= (Time.deltaTime * floatEffectTime);
            yield return null;
        }
    }

    void FloatEffect(){
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatEffectFrequency) * bufferVal;
        movablePlatform.transform.position = tempPos;
    }
}

public enum MovableAxis{
    Vertical,
    Horizontal
}