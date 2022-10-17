using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayOpenBlockerHandler : MonoBehaviour
{
    public float openTime,
                closingTime;
    public float closeStartTime;
    public Transform blocker,
                    logPoint,
                    startPoint,
                    endPoint;
    bool moveUp;
    float elapsedTime;
    void Start()
    {
        moveUp = true;
        elapsedTime = 0;
    }

    void Update()
    {
        if(elapsedTime < closeStartTime) {
            elapsedTime += Time.deltaTime;
            return;
        }
        if(moveUp){
            blocker.position += Vector3.up * (openTime * Time.deltaTime);
            if(Vector3.Distance(startPoint.position, logPoint.position) < 1f){
                moveUp = false; 
            }
        }else
        {
            blocker.position += Vector3.down * (closingTime * Time.deltaTime);
            if(Vector3.Distance(endPoint.position, logPoint.position) < 1f){
                moveUp = true; 
            }
        }
    }
}
