using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FW_PlayerController : MonoBehaviour
{
    public static FW_PlayerController Instance;
    Rigidbody2D RB2D;
    public float F_jumpForce;
    public float F_MovementSpeed;
    public Vector2 V2_direction;
    public bool B_Jump,B_MoveRight,B_MoveLeft,B_CanRun;
    public GameObject G_Broke;
    GameObject G_Question;
    public AudioSource AS_jump, AS_Land, AS_Running;
    public bool B_PlayQAudio;
    public bool B_JumpOnce;
    int k;
    public GameObject G_BrickClone;
    public ParticleSystem PS_R,PS_L;
  //  AnimationCurve curve = new AnimationCurve();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        RB2D = this.GetComponent<Rigidbody2D>();
        V2_direction.x = 1;
        B_Jump = false;
        PS_R.gameObject.SetActive(true);
        AS_Running.Play();
      
        // B_JumpOnce = true;
        // B_CanRun = true;
    }

    // Update is called once per frame
    void Update()
    {
       // if(B_Jump)
       // {
           // this.GetComponent<Animator>().Play("Jump");
           // RB2D.velocity = Vector2.up * F_jumpForce;
           // RB2D.AddForce(new Vector2(0f, F_MovementSpeed * F_jumpForce), ForceMode2D.Impulse);
           // B_Jump = false;
       // }

        if(!B_Jump && B_CanRun)
        {
            this.GetComponent<Animator>().Play("Run");

            this.transform.Translate(V2_direction * F_MovementSpeed * Time.deltaTime);

            // RB2D.MovePosition(RB2D.position + F_MovementSpeed * V2_direction * Time.fixedDeltaTime);
           

            if (B_MoveLeft) { this.transform.localScale = new Vector2(-0.6f, 0.6f);  }
            if (B_MoveRight) {  this.transform.localScale = new Vector2(0.6f, 0.6f); }
        }

    }

    public void Jumping()
    {
        if (B_CanRun)
        {
            this.GetComponent<CapsuleCollider2D>().isTrigger = true;
            B_Jump = true;
            AS_Running.Stop();
            AS_jump.Play();
         
            RB2D.AddForce(new Vector2(0f, F_MovementSpeed * F_jumpForce), ForceMode2D.Impulse);
       
            this.GetComponent<Animator>().Play("Jump");
            B_MoveLeft = false;
            B_MoveRight = false;
            B_CanRun = false;
            PS_R.gameObject.SetActive(false);
            PS_L.gameObject.SetActive(false);
            // Debug.Log("Above Invoke");
            Invoke(nameof(Offjump), 1.5f);
        B_JumpOnce = true;
        }
       //  Debug.Log("OFFJump calling");
    }
   public void Offjump()
    {
        this.GetComponent<CapsuleCollider2D>().isTrigger = false;
        if(G_Broke!=null)
        {
            G_Broke.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.red;
            G_Broke = null;
        }
      //  PS_Smoke.Play();
        // B_JumpOnce=true;
        // B_CanRun = true;
        B_Jump = false;
       // Debug.Log("Collider == false");
       

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       // Debug.Log(collision.gameObject.name);
        if(collision.gameObject.name=="RW")
        {
            PS_L.gameObject.SetActive(true);
           // PS_L.Play();
            PS_R.gameObject.SetActive(false);
            V2_direction.x = -1;
            B_MoveLeft = true;
            B_MoveRight = false;
        }
        if (collision.gameObject.name == "LW")
        {
            PS_R.gameObject.SetActive(true);
            PS_L.gameObject.SetActive(false); //PS_R.Play();
            V2_direction.x = 1;
            B_MoveLeft = false;
            B_MoveRight = true;
        }
        /* if (collision.gameObject.name == "Plane")
         {
             if (B_PlayQAudio)
             {
                 B_PlayQAudio = false;
                 if (collision.gameObject.transform.parent.transform.parent.name == "Questions")
                 {
                     GameObject Dummy = collision.gameObject.transform.parent.transform.parent.gameObject;
                     Dummy.transform.GetChild(Dummy.transform.childCount - 1).transform.GetChild(0).transform.GetChild(0).GetComponent<AudioSource>().Play();
                     Debug.Log(Dummy.transform.GetChild(Dummy.transform.childCount - 1).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                 }
             }

             //  Debug.Log("Touching Plane");
             if (B_Jump)
             {
                 //  Debug.Log("Jump");
                 if (G_Broke == null)
                 {
                     AS_Land.Play();
                     G_Broke = collision.gameObject.transform.parent.gameObject;
                     // Debug.Log(G_Broke.transform.parent.name);

                     if (G_Broke.transform.parent.name == "Questions")
                     {
                         if (B_JumpOnce)
                         {
                            // FW_Main.Instance.THI_SetQuestion(G_Broke.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);


                             FW_Main.Instance.STR_currentSelectedAnswer = G_Broke.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

                             if (FW_Main.Instance.STR_currentSelectedAnswer == FW_Main.Instance.STR_currentQuestionAnswer)
                             {
                                 B_JumpOnce = false;
                                 FW_Main.Instance.THI_Correct();
                                 falldown();
                             }
                             else
                             {
                                 FW_Main.Instance.THI_Wrong();
                                 B_CanRun = true;
                                 Offjump();

                             }


                         }

                         // Debug.Log("Jumping on Question floor");

                     }
                     else
                     {
                         falldown();
                     }

                 }

             }

         }*/
    }

    public void falldown()
    {
       // Debug.Log("Falling");
        k = 2;
       /* for (int i = 0; i < k; i++)
        {
            G_Broke.transform.GetChild(i).GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            G_Broke.transform.GetChild(i).GetComponent<Collider2D>().enabled = true;
        }*/
        
        AS_Running.Play();
        GameObject Dummy = Instantiate(G_BrickClone);
        Dummy.transform.position = G_Broke.transform.GetChild(1).transform.position;
        Dummy.GetComponent<SpriteRenderer>().color = G_Broke.transform.GetChild(1).GetComponent<SpriteRenderer>().color;

        Destroy(G_Broke);
        //G_Broke = null;
        B_PlayQAudio = true;
    }
   
}
