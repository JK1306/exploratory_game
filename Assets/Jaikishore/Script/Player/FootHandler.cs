using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootHandler : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerController.instance.inFloatingFloor = true;
    }

    private void OnTriggerStay2D(Collider2D other) {
        PlayerController.instance.inGround = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerController.instance.inGround = false;
        PlayerController.instance.inFloatingFloor = false;
    }
}
