using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.gameObject.name+" on enter : "+other.gameObject.GetComponent<BoxCollider2D>().enabled, other.gameObject);
        if(other.gameObject.GetComponent<BoxCollider2D>().enabled){
            other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log(other.gameObject.name+" on exit : "+other.gameObject.GetComponent<BoxCollider2D>().enabled, other.gameObject);

        if(!other.gameObject.GetComponent<BoxCollider2D>().enabled){
            other.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
