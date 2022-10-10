using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Animal"){ return; }
        PlayerController.instance.movementSpeed = 0;
        Debug.Log("From FrontHandler : "+other.name);
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerController.instance.ResetMovementSpeed();
    }
}
