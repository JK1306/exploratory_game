using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick_clone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.name=="Plane" || collision.collider.name == "Off_player")
        {
            Destroy(this.gameObject);
        }
    }
}
