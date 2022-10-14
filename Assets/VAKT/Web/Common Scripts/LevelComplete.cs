using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class LevelComplete : MonoBehaviour
{
    public GameObject G_star1;
    public GameObject G_star2;
    public GameObject G_star3;
    public Text TEX_finalPoints;
    public int I_2star;
    public int I_3star;
    public GameObject G_replayButtonMob, G_replayButtonWeb, G_nextButtonMob, G_nextButtonWeb;



    void Start()
    {
        THI_platformChanges();
        THI_requiredScore();
        THI_showScore();
    }



    void THI_platformChanges()
    {
#if UNITY_ANDROID || UNITY_IOS
G_replayButtonMob.SetActive(true);
G_replayButtonWeb.SetActive(false);
G_nextButtonMob.SetActive(true);
G_nextButtonWeb.SetActive(false);



#elif UNITY_WEBGL
        G_replayButtonMob.SetActive(false);
        G_replayButtonWeb.SetActive(true);
        G_nextButtonMob.SetActive(false);
        G_nextButtonWeb.SetActive(true);
#endif
    }



    void THI_requiredScore()
    {
        int totalObtainablePoints = MainController.instance.I_TotalQuestions * MainController.instance.I_correctPoints;
        // Debug.Log("Max points : " + totalObtainablePoints);
        // Debug.Log("Total obtainable points : " + totalObtainablePoints);
        I_2star = totalObtainablePoints / 3;
        I_3star = totalObtainablePoints / 2;
    }



    void THI_showScore()
    {
        TEX_finalPoints.text = MainController.instance.I_TotalPoints.ToString();
        // Debug.Log("Points got : " + MainController.instance.I_TotalPoints);



        if (MainController.instance.I_TotalPoints > 0 && MainController.instance.I_TotalPoints < I_2star) // 1 star
        {
            G_star1.SetActive(true);
        }
        if (MainController.instance.I_TotalPoints >= I_2star && MainController.instance.I_TotalPoints < I_3star) // 2 star
        {
            G_star1.SetActive(true);
            G_star2.SetActive(true);
        }
        if (MainController.instance.I_TotalPoints >= I_3star) // 3 star
        {
            G_star1.SetActive(true);
            G_star2.SetActive(true);
            G_star3.SetActive(true);
        }
    }



    public void BUT_replayWeb() // mobile or web gl
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BUT_replayMob() // mobile or web gl
    {
       // StartCoroutine(VAKT_controller.instance.EN_loadBundle());
    }



    public void BUT_nextMobile() // mobile only
    {
        // VAKT_controller.instance.GA_pages[5].SetActive(true);
        Screen.orientation = ScreenOrientation.Portrait;
      //  VAKT_controller.instance.BUT_gameLWSintroBack();
      //  Destroy(VAKT_controller.instance.G_currentActivity);
    }



    public void BUT_nextWeb() // web only
    {
        Application.ExternalEval("closeApplication()");
    }
}