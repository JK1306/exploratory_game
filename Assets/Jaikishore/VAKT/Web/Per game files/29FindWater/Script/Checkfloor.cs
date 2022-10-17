using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkfloor : MonoBehaviour
{
    GameObject G_Other;
    bool B_Lerp;
    public GameObject G_Sad, G_Happy;
    bool B_CallOnce;

    public void Start()
    {
        B_CallOnce = true;
        G_Sad.SetActive(true);
        G_Happy.SetActive(false);
    }
    private void Update()
    {
        if(B_Lerp)
        {
           
            this.transform.parent.transform.position = Vector3.Lerp(this.transform.parent.transform.position ,G_Other.transform.GetChild(6).transform.position, 0.05f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Running")
        {
           // FW_PlayerController.Instance.B_Jump = false;
            if (!FW_PlayerController.Instance.B_Jump)
            {
              //  Debug.Log("Runn");
                
                FW_PlayerController.Instance.B_CanRun = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit");
        FW_PlayerController.Instance.B_CanRun = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name=="Off_player")
        {
            if(B_CallOnce)
            {
                B_CallOnce = false;
                FW_PlayerController.Instance.PS_R.gameObject.SetActive(false);
                FW_PlayerController.Instance.PS_L.gameObject.SetActive(false);
                B_Lerp = true;
                this.transform.parent.transform.localScale = new Vector2(0.60f, 0.60f);
                G_Other = collision.gameObject;
                this.transform.parent.gameObject.GetComponent<Animator>().Play("OpenWater");
                Invoke(nameof(WaterFill), 2f);
            }
            
        }


        if (collision.gameObject.name == "Plane")
        {
            if (FW_PlayerController.Instance.B_PlayQAudio)
            {
                FW_PlayerController.Instance.B_PlayQAudio = false;
                if (collision.gameObject.transform.parent.transform.parent.name == "Questions")
                {
                    GameObject Dummy = collision.gameObject.transform.parent.transform.parent.gameObject;
                    Dummy.transform.GetChild(Dummy.transform.childCount - 1).transform.GetChild(0).transform.GetChild(0).GetComponent<AudioSource>().Play();
                    //Debug.Log(Dummy.transform.GetChild(Dummy.transform.childCount - 1).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                }
            }

            //  Debug.Log("Touching Plane");
            if (FW_PlayerController.Instance.B_Jump)
            {
                //  Debug.Log("Jump");
                if (FW_PlayerController.Instance.G_Broke == null)
                {
                    FW_PlayerController.Instance.AS_Land.Play();
                    FW_PlayerController.Instance.G_Broke = collision.gameObject.transform.parent.gameObject;
                    // Debug.Log(G_Broke.transform.parent.name);

                    if (FW_PlayerController.Instance.G_Broke.transform.parent.name == "Questions")
                    {
                        if (FW_PlayerController.Instance.B_JumpOnce)
                        {
                            // FW_Main.Instance.THI_SetQuestion(G_Broke.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);


                            FW_Main.Instance.STR_currentSelectedAnswer = FW_PlayerController.Instance.G_Broke.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

                            if (FW_Main.Instance.STR_currentSelectedAnswer == FW_Main.Instance.STR_currentQuestionAnswer)
                            {
                                FW_PlayerController.Instance.B_JumpOnce = false;
                                FW_Main.Instance.THI_Correct();
                                FW_PlayerController.Instance.falldown();
                            }
                            else
                            {
                                FW_Main.Instance.THI_Wrong();
                                FW_PlayerController.Instance.B_CanRun = true;
                                FW_PlayerController.Instance.Offjump();

                            }


                        }

                        // Debug.Log("Jumping on Question floor");

                    }
                    else
                    {
                        FW_PlayerController.Instance.falldown();
                    }

                }

            }

        }
    }

    void WaterFill()
    {
        G_Sad.SetActive(false);
        G_Happy.SetActive(true);
        B_Lerp = false;
        
        G_Other.GetComponent<Animator>().Play("WaterFlow");
        Invoke(nameof(THI_LVLComplete), 8f);
    }

    void THI_LVLComplete()
    {
        FW_Main.Instance.THI_Levelcompleted();
    }
}
