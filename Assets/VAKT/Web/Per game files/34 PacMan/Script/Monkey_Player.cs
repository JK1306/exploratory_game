using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey_Player : MonoBehaviour
{
    public static Monkey_Player Instance;
    public bool[] B_Directions;

    public float movementspeed;
    public Sprite SPR_MouthOpen, SPR_Normal,SPR_Sad;
    void Start()
    {
        Instance = this;
        B_Directions = new bool[4];
    }
    public void PUB_Directionselect(int count)
    {
        OFFBool();
        B_Directions[count] = true;
    }

    // Update is called once per frame
    void Update()
    {
      
        if (B_Directions[0])
        {
            transform.Translate(Vector3.left * movementspeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, -180, 0);
            Debug.LogWarning("Rotate0 "+ transform.rotation);
        }
        else
        if (B_Directions[1])
        {
            transform.Translate(Vector3.left * movementspeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0,0);
            Debug.LogError("Rotate1 " + transform.rotation);
        }
        else
        if (B_Directions[2])
        {
            transform.Translate(Vector3.left * movementspeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, -90);
          //  Debug.Log("Rotate2 " + transform.rotation);
        }
        else
        if (B_Directions[3])
        {
            transform.Translate(Vector3.left * movementspeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, -270);
           // Debug.Log("Rotate3 " + transform.rotation);
        }
       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject!=null)
        {
            Debug.Log("Collision = " + collision.gameObject.name);
            if (collision.gameObject.transform.parent.name=="Enemy")
            {
                OFFBool();
                this.transform.GetChild(0).GetComponent<Animator>().enabled = true;
                this.transform.GetChild(0).GetComponent<Animator>().Play("MonkeySad");
               
               
           }
            if (collision.gameObject.name == "maze-1")
            {
                OFFBool();
            }

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
       /* if (collision.gameObject.name == "maze-1")
        {
            OFFBool();
        }*/
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject!=null)
        {
            Debug.Log(collision.gameObject.name);
           

            if (collision.gameObject.transform.parent.transform.parent.name == "Content")
            {
                OFFBool();
                Pac_Monkey_Main.Instance.THI_Check(collision.gameObject.name);
            }
            if (collision.gameObject.transform.parent.name == "Dots")
            {
                collision.gameObject.SetActive(false);
                Pac_Monkey_Main.Instance.THI_Collect();
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SPR_MouthOpen;
                Invoke(nameof(Normal), 0.3f);
            }
            if (collision.gameObject.transform.parent.name == "PowerBanana")
            {
                collision.gameObject.SetActive(false);
                Pac_Monkey_Main.Instance.THI_MonkeyPower();
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SPR_MouthOpen;
                Invoke(nameof(Normal), 0.3f);
            }
        }
    }
    void OFFBool()
    {
        for (int i = 0; i < B_Directions.Length; i++)
        {
            B_Directions[i] = false;
        }
    }
    void Normal()
    {
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SPR_Normal;
    }
}
