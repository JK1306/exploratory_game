using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public static MainController instance;


    [Header("PLATFORM")]
    public bool WEB;
    public bool MOBILE;

    [Header("MANAGERS")]
    public GameObject G_GameID;
    public GameObject G_GameManager;

    [Header("OBJECTS")]
    public GameObject G_coverPageStart;
    public Image IM_loading;
    public float startLoad;
    public float maxLoad;
    public GameObject G_coverPage;
    public bool B_enteredGame;

    [Header("ID")]
    public string STR_IDjson;
    public string STR_childID;
    public string STR_GameID;
    public bool called;
    public string STR_responseSerial;

    [Header("SCORE")]
    public int I_TotalPoints;
    public int I_TotalQuestions;
    public int I_correctPoints;

    [Header("MODE")]
    public string mode;


    [Header("PREVIEW MODE")]
    public string STR_previewJsonAPI;

    [Header("INITIALIZE")]
    public int I_loadTime;


    void Awake()
    {
        instance = this;

        G_coverPageStart.GetComponent<Button>().interactable = false;
        B_enteredGame = false;


        maxLoad = 100f;

#if UNITY_ANDROID || UNITY_IOS
        MOBILE = true;
        WEB = false;
     //   Debug.Log("MOBILE");
#elif UNITY_WEBGL
        MOBILE = false;
        WEB = true;
        //  Debug.Log("WEB");
#endif


        //if (MOBILE)
        //{
        //    STR_GameID = GameManager.instance.STR_selectedGameID;
        //    G_GameManager.SetActive(false);
        //    G_GameID.SetActive(false);
        //}
        if (WEB)
        {
            G_GameManager.SetActive(false);
            G_GameID.SetActive(true);


           // testing
           STR_childID = "336";
           mode = "live";
        //    mode = "preview";

            // Live ID's
            //  STR_GameID = "413"; //train sorting
            //  STR_GameID = "382"; //fish sorting
            //   STR_GameID = "404"; //fruit ninja
            //   STR_GameID = "322"; //Domino arrange
            //  STR_GameID = "378"; //robot runner
            //  STR_GameID = "348"; //fish nemo
            //  STR_GameID = "364"; //Cake Baking ,robot runner
            //  STR_GameID = "332"; //Farm harvest ,desert car racing
            //  STR_GameID = "387"; //sling shot
            //  STR_GameID = "378"; //robot runner
            //  STR_GameID = "405"; // misfit
            //  STR_GameID = "415"; // Hill climb

            //  STR_GameID = "435"; // HangMan
            //  STR_GameID = "430"; // Snake
            //  STR_GameID = "440"; // direction
            //  STR_GameID = "401"; // boardgame
            //  STR_GameID = "426"; // ending blends

            //Live ID's

            // STR_GameID = "133"; //  camel game
            //   STR_GameID = "106"; // caterpillar game, stack game
            // STR_GameID = "2";   // crate game,bird game
            // STR_GameID = "10";  // helicopter game
            // STR_GameID = "13";  // squirrel game,fireman game
            // STR_GameID = "69";  // white dummy game 1 (img ques / img ans)  santa game
            //  STR_GameID = "70";  // white dummy game 2 (img ques / text ans)   cake baking , robot runner , river rafting
            // STR_GameID = "147"; // sorting game
            // STR_GameID = "191"; // hangman
            // STR_GameID = "192"; // Audio click
            // STR_GameID = "175"; // worksheet image drag and drop

            // STR_GameID = "193";  // direction game
            // STR_GameID = "195";  // farm harvesting , desert car racing
            // STR_GameID = "204";  // Audio Question text option sorting

            // STR_GameID = "143";   // text text hillclimbracing, football // bike racing     snake

            //  STR_GameID = "203";  // Word Search
            //  STR_GameID = "205";   // true or false
            //  STR_GameID = "206";   // find the water --> text text 4 options
            //  STR_GameID = "207";   // popcorn
            //  STR_GameID = "350";   // Board game
            //    STR_GameID = "354";   // bridge crossing
            //    STR_GameID = "303";   // Readcomp

        }
    }


    private void Start()
    {
        I_loadTime = Random.Range(15, 51);
     //   Debug.Log("Load Time :  " + I_loadTime);
    }

    private void Update()
    {
        if (WEB)
        {
            if (STR_GameID != "" && STR_childID!="" && !called) // live
            {
                called = true;
                G_GameManager.SetActive(true);
            }
            if(STR_GameID==""&& STR_childID == "" && !called && mode=="preview") // preview
            {
                called = true;
                G_GameManager.SetActive(true);
               // G_coverPage.SetActive(false);
            }
        }
        if(MOBILE)
        {
            if (STR_GameID != "" && STR_childID != "" && !called)
            {
                called = true;
                G_GameManager.SetActive(true);
            }
        }


        if (called && startLoad<100f)
        {
            startLoad = startLoad + I_loadTime * Time.deltaTime;
            IM_loading.fillAmount = startLoad / maxLoad;
            if(startLoad>= maxLoad)
            {
                G_coverPageStart.GetComponent<Button>().interactable = true;
                G_coverPageStart.GetComponent<Animator>().enabled = true;
                IM_loading.transform.parent.GetChild(1).gameObject.SetActive(true);
            }
        }
        if(!G_coverPage.activeInHierarchy && !B_enteredGame) // player has entered the game
        {
            B_enteredGame = true;

            /* if (HelicopterGameManager.instance!=null)
             {
                HelicopterGameManager.instance.AS_helicopter.Play();   // helicopter game
             }
            if (HelicopterGameManager.instance!=null)
            {
                HelicopterGameManager.instance.AS_helicopter.Play();   // helicopter game
            }*/
        }
    }
}
