using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using ForestHabitatGame;

public class MainGameController : MonoBehaviour
{
    public List<CollectableObject> collectables;
    public List<Animal> animals;
    public static MainGameController instance;
    public GameObject inventoryPanel,
                    inventoryObjects,
                    inventory;
    public Text inventoryErrorMsgDisplayPanel;
    public Button inventoryButton,
                    feedBtn;
    public Button cancelBtn;

    public AudioClip fruitGainAudio,
                    gameCompleteAudio;
    public bool animalNearby;
    public AnimalController nearbyAnimalContorller;
    public ParticleSystem playerDeathParticle,
                            gameCompleteParticles;
    public Camera camera_;
    public GameObject lifeLineObject;
    public int playerLifeLine;
    public PlayMode playMode;
    public GameObject gameOver;
    public GameObject androidInput,
                        webGLDemoScreen,
                        androidDemoScreen;
    public int movementDirection;
    GameObject spawnObject;
    GameObject foodToFeed;
    GameObject[] activeAnimals;
    ParticleSystem spawnedParticle;
    float elapsedTime;
    bool gameCompleted;
    bool moveRight,
        moveLeft;
    

    public bool B_production;

    [Header("Screens and UI elements")]
    public GameObject G_Demo;
    bool B_CloseDemo;
    public GameObject G_coverPage;
    public GameObject G_instructionPage;
    public TextMeshProUGUI TEXM_instruction;
    public Text TEX_points;
    public Text TEX_questionCount;
    public TextMeshProUGUI TM_pointFx;
    public string STR_Passage;

    [Header("Values")]
    public string STR_currentQuestionAnswer;
    public string STR_currentSelectedAnswer;
    public int I_currentQuestionCount; // question number current
    public string STR_currentQuestionID;
    public int I_Points;
    public int I_wrongAnsCount;

    [Header("URL")]
    public string URL;
    public string SendValueURL;

    [Header("DB")]
    public List<string> STRL_difficulty;
    public string STR_difficulty;
    public List<int> IL_numbers;
    public int I_correctPoints;
    public int I_wrongPoints;
    public List<string> STRL_instruction;
    public string STR_instruction;
    public string STR_video_link;
    public List<string> STRL_options;
    public List<string> STRL_questions;
    public List<string> STRL_answers;
    public List<string> STRL_quesitonAudios;
    public List<string> STRL_optionAudios;
    public List<string> STRL_instructionAudio;
    public List<string> STRL_questionID;
    public string STR_customizationKey;
    //Dummy values only for helicopter game
    public List<string> STRL_BG_img_link;
    public List<string> STRL_avatar_Color;
    public List<string> STRL_Panel_Img_link;
    public List<string> STRL_Cover_Img_link;
    public List<string> STRL_passageDetail;

    [Header("GAME DATA")]
    public List<string> STRL_gameData;
    public string STR_Data;

    [Header("LEVEL COMPLETE")]
    public GameObject G_levelComplete;

    [Header("AUDIO ASSIGN")]
    public AudioClip[] ACA__questionClips;
    public AudioClip[] ACA_optionClips;
    public AudioClip[] ACA_instructionClips;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }

        if (B_production)
        {
            URL = "https://dlearners.in/template_and_games/Game_template_api-s/game_template_1.php"; // PRODUCTION FETCH DATA
            SendValueURL = "https://dlearners.in/template_and_games/Game_template_api-s/save_child_questions.php"; // PRODUCTION SEND DATA
        }
        else
        {
            URL = "http://20.120.84.12/Test/template_and_games/Game_template_api-s/game_template_1.php"; // UAT FETCH DATA
            SendValueURL = "http://20.120.84.12/Test/template_and_games/Game_template_api-s/save_child_questions.php"; // UAT SEND DATA
        }
    }

    void Start()
    {
        if(MainController.instance.MOBILE){
            androidInput.SetActive(true);
            androidDemoScreen.SetActive(true);
        }else{
            webGLDemoScreen.SetActive(true);
        }

        gameCompleted = false;
        G_levelComplete.SetActive(false);
        G_instructionPage.SetActive(false);

        TEX_points.text = I_Points.ToString();
        STRL_questions = new List<string>();
        STRL_answers = new List<string>();
        STRL_options = new List<string>();
        Invoke("THI_gameData", 1f);
        lifeLineObject.transform.GetChild(0).GetComponent<Text>().text = "x"+playerLifeLine;
        // I_currentQuestionCount = -1;
        // if(playMode == PlayMode.DemoGame){
        camera_.GetComponent<CameraHandler>().B_canfollow = false;
        // }
    }

    private void OnEnable() {
        inventoryButton.onClick.AddListener(ViewInventory);
        cancelBtn.onClick.AddListener(CloseInventory);
        feedBtn.onClick.AddListener(PerformFeedAnimal);
    }

    private void OnDisable() {
        inventoryButton.onClick.RemoveListener(ViewInventory);
        cancelBtn.onClick.RemoveListener(CloseInventory);
        feedBtn.onClick.RemoveListener(PerformFeedAnimal);
    }

    private void Update() {
        elapsedTime += Time.deltaTime;

        if(!gameCompleted && playMode != PlayMode.DemoGame) CheckGameCompleted();
    }

#region TEMPLATE_INTEGRATION
    void THI_gameData()
    {
        // THI_getPreviewData();
        if (MainController.instance.mode == "live")
        {
            StartCoroutine(EN_getValues()); // live game in portal
        }
        if (MainController.instance.mode == "preview")
        {
            // preview data in html game generator

            Debug.Log("PREVIEW MODE RAKESH");
            THI_getPreviewData();
        }
    }

    public void THI_pointFxOn(bool plus)
    {
        if (plus)
        {
            if (I_correctPoints != 1)
            {
                TM_pointFx.text = "+" + I_correctPoints + " points";
            }
            else
            {
                TM_pointFx.text = "+" + I_correctPoints + " point";
            }
        }
        else
        {
            if (I_Points > 0)
            {
                if (I_wrongPoints != 0)
                {
                    if (I_wrongPoints != 1)
                    {
                        TM_pointFx.text = "-" + I_wrongPoints + " points";
                    }
                    else
                    {
                        TM_pointFx.text = "-" + I_wrongPoints + " point";
                    }
                }
            }
        }
        Invoke("THI_pointFxOff", 1f);
    }

    public void THI_pointFxOff()
    {
        TM_pointFx.text = "";
    }

    public IEnumerator IN_CoverImage()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(STRL_Cover_Img_link[0]);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (STRL_Cover_Img_link != null)
            {
                G_coverPage.GetComponent<Image>().sprite = Sprite.Create(downloadedTexture, new Rect(0.0f, 0.0f, downloadedTexture.width, downloadedTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }

        //SPRA_Options

    }

    public IEnumerator EN_getValues()
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", MainController.instance.STR_GameID);
        // Debug.Log("GAME ID : " + MainController.instance.STR_GameID);
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();
        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {

            MyJSON json = new MyJSON();
            //json.Helitemp(www.downloadHandler.text);
            json.Temp_type_2(www.downloadHandler.text, STRL_difficulty, IL_numbers, STRL_questions, STRL_answers, STRL_options, STRL_questionID, STRL_instruction, STRL_quesitonAudios, STRL_optionAudios,
            STRL_instructionAudio, STRL_Cover_Img_link, STRL_passageDetail);
            //        Debug.Log("GAME DATA : " + www.downloadHandler.text);

            STR_difficulty = STRL_difficulty[0];

            STR_instruction = STRL_instruction[0];
            MainController.instance.I_correctPoints = I_correctPoints = IL_numbers[1];
            I_wrongPoints = IL_numbers[2];
            MainController.instance.I_TotalQuestions = STRL_questions.Count;

            if (STRL_passageDetail != null)
            {
                for (int i = 0; i < STRL_questions.Count; i++)
                {
                    STR_Passage = STRL_passageDetail[0];
                }
            }

            StartCoroutine(EN_getAudioClips());
            StartCoroutine(IN_CoverImage());

        }
    }
  
    public IEnumerator EN_getAudioClips()
    {
        ACA__questionClips = new AudioClip[STRL_quesitonAudios.Count];
        ACA_optionClips = new AudioClip[STRL_optionAudios.Count];
        ACA_instructionClips = new AudioClip[STRL_instructionAudio.Count];

        for (int i = 0; i < STRL_quesitonAudios.Count; i++)
        {
            UnityWebRequest www1 = UnityWebRequestMultimedia.GetAudioClip(STRL_quesitonAudios[i], AudioType.MPEG);
            yield return www1.SendWebRequest();
            if (www1.result == UnityWebRequest.Result.ConnectionError || www1.isHttpError || www1.isNetworkError)
            {
                Debug.Log(www1.error);
            }
            else
            {
                ACA__questionClips[i] = DownloadHandlerAudioClip.GetContent(www1);
            }
        }

        for (int i = 0; i < STRL_optionAudios.Count; i++)
        {
            UnityWebRequest www2 = UnityWebRequestMultimedia.GetAudioClip(STRL_optionAudios[i], AudioType.MPEG);
            yield return www2.SendWebRequest();
            if (www2.result == UnityWebRequest.Result.ConnectionError || www2.isHttpError || www2.isNetworkError)
            {
                Debug.Log(www2.error);
            }
            else
            {
                ACA_optionClips[i] = DownloadHandlerAudioClip.GetContent(www2);
            }
        }


        for (int i = 0; i < STRL_instructionAudio.Count; i++)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(STRL_instructionAudio[i], AudioType.MPEG);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {

                ACA_instructionClips[i] = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log("audio clips fetched instruction");

            }
        }
        THI_assignAudioClips();

        // THI_OffDemo();
    }

    void THI_assignAudioClips()
    {
        if (ACA_instructionClips.Length > 0)
        {
            TEXM_instruction.gameObject.AddComponent<AudioSource>();
            TEXM_instruction.gameObject.GetComponent<AudioSource>().playOnAwake = false;
            TEXM_instruction.gameObject.GetComponent<AudioSource>().clip = ACA_instructionClips[0];
            TEXM_instruction.gameObject.AddComponent<Button>();
            TEXM_instruction.gameObject.GetComponent<Button>().onClick.AddListener(THI_playAudio);
        }

    }

    void THI_playAudio()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<AudioSource>().Play();
        Debug.Log("player clicked. so playing audio");
    }

    public void THI_getPreviewData()
    {
        MyJSON json = new MyJSON();
        //  json.Helitemp(MainController.instance.STR_previewJsonAPI);
        json.Temp_type_2(MainController.instance.STR_previewJsonAPI, STRL_difficulty, IL_numbers, STRL_questions, STRL_answers, STRL_options, STRL_questionID, STRL_instruction, STRL_quesitonAudios, STRL_optionAudios,
            STRL_instructionAudio, STRL_Cover_Img_link, STRL_passageDetail);

        STR_difficulty = STRL_difficulty[0];
        STR_instruction = STRL_instruction[0];
        MainController.instance.I_correctPoints = I_correctPoints = IL_numbers[1];
        I_wrongPoints = IL_numbers[2];
        MainController.instance.I_TotalQuestions = STRL_questions.Count;


        StartCoroutine(EN_getAudioClips());
        StartCoroutine(IN_CoverImage());
    }

    public void THI_TrackGameData(string analysis)
    {
        DBmanager TrainSortingDB = new DBmanager();
        TrainSortingDB.question_id = STR_currentQuestionID;
        TrainSortingDB.answer = STR_currentSelectedAnswer;
        TrainSortingDB.analysis = analysis;
        string toJson = JsonUtility.ToJson(TrainSortingDB);
        STRL_gameData.Add(toJson);
        STR_Data = string.Join(",", STRL_gameData);
    }

    void THI_Levelcompleted()
    {
        MainController.instance.I_TotalPoints = I_Points;
        G_levelComplete.SetActive(true);
        StartCoroutine(IN_sendDataToDB());
    }

    public IEnumerator IN_sendDataToDB()
    {
        WWWForm form = new WWWForm();
        form.AddField("child_id", MainController.instance.STR_childID);
        form.AddField("game_id", MainController.instance.STR_GameID);
        form.AddField("game_details", "[" + STR_Data + "]");


        Debug.Log("child id : " + MainController.instance.STR_childID);
        Debug.Log("game_id  : " + MainController.instance.STR_GameID);
        Debug.Log("game_details: " + "[" + STR_Data + "]");

        UnityWebRequest www = UnityWebRequest.Post(SendValueURL, form);
        yield return www.SendWebRequest();
        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Sending data to DB failed : " + www.error);
        }
        else
        {
            MyJSON json = new MyJSON();
            json.THI_onGameComplete(www.downloadHandler.text);

            Debug.Log("Sending data to DB success : " + www.downloadHandler.text);
        }
    }

    public void BUT_playAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BUT_instructionPage()
    { 
        Time.timeScale = 0;
        G_instructionPage.SetActive(true);
        TEXM_instruction.text = STR_instruction;
        TEXM_instruction.GetComponent<AudioSource>().Play();
    }

    public void BUT_closeInstruction()
    {
        Time.timeScale = 1;
        G_instructionPage.SetActive(false);
    }
#endregion TEMPLATE_INTEGRATION

#region GAME_LOGICS

    public void MobileInputHandler(int movementDirection){
        PlayerController.instance.movementDirection = -1f;
    }

    public void AddPoints(){
        I_Points += I_correctPoints;
        TEX_points.text = I_Points.ToString();
        THI_pointFxOn(true);
    }

    public void RemovePoints(){
        I_Points -= I_wrongPoints;
        TEX_points.text = I_Points.ToString();
        THI_pointFxOn(false);
    }

    void PerformFeedAnimal(){
        ForestHabitatGame.DemoController.instance.feedClicked = true;
        // InstructionController Demo.instance.feedClicked
        inventoryErrorMsgDisplayPanel.text = "";
        if(!animalNearby){
            Debug.Log("can't feed food");
            // DemoController.instance.AnimalFeedReset();

            inventoryErrorMsgDisplayPanel.text = "Go Near to animal to Feed";
            return;
        }

        if(!inventory.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault()){
            inventoryErrorMsgDisplayPanel.text = "Select Food to Feed";
            return;
        }

        foodToFeed = inventory.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault().gameObject;
        foreach (var item in collectables)
        {
            if(foodToFeed.name.ToLower().Contains(item.collectableType.ToString().ToLower())){
                item.collectionCount--;
                ManageScore(item.collectableCategory);
                break;
                // FeedFruit(item.collectableType);
            }
        }

        CloseInventory();
    }

    public void CheckGameCompleted(){
        activeAnimals = GameObject.FindGameObjectsWithTag("Animal");
        if(activeAnimals.Length <= 0){
            gameCompleted = true;
            PlayerController.instance.LockInput();
            camera_.GetComponent<AudioSource>().clip = gameCompleteAudio;
            camera_.GetComponent<AudioSource>().Play();
            StartCoroutine(SpawnGameCompleteParticle());
            Debug.Log("Alive Animal Count : "+activeAnimals.Length);
        }
    }

    IEnumerator SpawnGameCompleteParticle(){
        yield return new WaitForSeconds(0.5f);
        spawnedParticle = Instantiate(gameCompleteParticles, PlayerController.instance.gameObject.transform);
        spawnedParticle.Play();
        while(camera_.GetComponent<AudioSource>().isPlaying){
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        THI_Levelcompleted();
    }

    public void ResetScoring(){
        I_Points = 0;
        TEX_points.text = "0";
        foreach (var animal in animals)
        {
            animal.score = 0;
            animal.scoreBoardDisplay.transform.GetChild(1).GetComponent<Text>().text = animal.score.ToString()+"/"+animal.animalCount.ToString();
        }
    }

    void FeedFruit(Collectables collectable){
        foreach (var item in collectables){
            if(item.collectableType == collectable){
                item.collectionCount--;
                ManageScore(item.collectableCategory);
                break;
            }
        }
    }

    void ManageScore(EatableType collectableCategory){
        bool condition1 = (nearbyAnimalContorller.animalCategory == AnimalType.Carnivores && collectableCategory == EatableType.Meat);
        bool condition2 = (nearbyAnimalContorller.animalCategory == AnimalType.Herbivores && (collectableCategory == EatableType.Grass ));
        bool condition3 = (nearbyAnimalContorller.animalCategory == AnimalType.Omnivores && (collectableCategory == EatableType.Fruit));

        StartCoroutine(nearbyAnimalContorller.AnimalFeeded());
        
        if(condition1 || condition2 || condition3){
            foreach (var animal in animals)
            {
                if(animal.animalController.animalName == nearbyAnimalContorller.animalName){
                    animal.score++;
                    animal.scoreBoardDisplay.transform.GetChild(0).gameObject.SetActive(true);
                    animal.scoreBoardDisplay.transform.GetChild(1).GetComponent<Text>().text = animal.score.ToString()+"/"+animal.animalCount.ToString();
                    // animal.scoreBoardDisplay.transform.GetChild(1).GetComponent<Text>().text = "x"+animal.score.ToString();
                }
            }
            STR_currentSelectedAnswer = foodToFeed.gameObject.name;
            THI_TrackGameData("1");
            AddPoints();
        }else{
            STR_currentSelectedAnswer = foodToFeed.gameObject.name;
            THI_TrackGameData("0");
            RemovePoints();
        }
    }

    void ViewInventory(){
        if(inventoryPanel.activeSelf) { return; }
        ForestHabitatGame.DemoController.instance.inventoryOpened = true;
        // InstructionController.instance.inventoryOpened = true;
        inventoryButton.transform.parent.GetComponent<Animator>().enabled = false;
        inventoryPanel.SetActive(true);
        foreach (var item in collectables)
        {
            if(item.collectionCount > 0){
                spawnObject = Instantiate(inventoryObjects, inventory.transform);
                spawnObject.name = item.collectableType.ToString();
                spawnObject.GetComponent<Toggle>().group = inventory.GetComponent<ToggleGroup>();
                spawnObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "x" + item.collectionCount.ToString();
                spawnObject.transform.GetChild(2).gameObject.SetActive(true);
                spawnObject.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = item.collectable;
            }
        }
    }

    void CloseInventory(){
        inventoryPanel.SetActive(false);
        for(int i=0; i<inventory.transform.childCount; i++){
            Destroy(inventory.transform.GetChild(i).gameObject);
        }
    }

    public void LevelRestart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CollectCollectable(Collectables collectable){
        foreach (var item in collectables)
        {
            if(item.collectableType == collectable){
                item.collectionCount++;
                break;
            }
        }
    }

    public void PlayerDeath(Vector3 spawnPosition, Vector3 endPoint, float camerMovementDuration){
        playerLifeLine--;

        // Game Over Scene enabled
        if(playerLifeLine <= 0){
            gameOver.SetActive(true);
            return;
        }

        lifeLineObject.GetComponent<Animator>().Play("New State");
        lifeLineObject.transform.GetChild(0).GetComponent<Text>().text = "x"+playerLifeLine;
        lifeLineObject.GetComponent<Animator>().Play("heart-burst");

        camera_.GetComponent<AudioSource>().clip = PlayerController.instance.deathClip;
        camera_.GetComponent<AudioSource>().Play();
        object[] param = new object[3]{
            new Vector3(spawnPosition.x, spawnPosition.y, camera_.transform.position.z), 
            new Vector3(endPoint.x, endPoint.y, camera_.transform.position.z), 
            camerMovementDuration
        };

        spawnedParticle = Instantiate(playerDeathParticle, spawnPosition, Quaternion.identity);
        spawnedParticle.Play();
        StartCoroutine(nameof(WaitUntilParticleComplete), param);
    }

    IEnumerator WaitUntilParticleComplete(object[] param){
        while(spawnedParticle.isPlaying){
            yield return null;
        }
        StartCoroutine(nameof(MoveCamera), param);
    }

    public IEnumerator MoveCamera(object[] param){
        elapsedTime = 0;
        while(elapsedTime < (float)param[2]){
            camera_.transform.position = Vector3.Lerp((Vector3)param[0], ((Vector3)param[1]), elapsedTime/(float)param[2]);
            yield return null;
        }
        CameraHandler.OBJ_followingCamera.B_canfollow = true;
        PlayerController.instance.SpawnPlayer();
    }

#endregion GAME_LOGICS

}

[System.Serializable]
public class CollectableObject{
    public Sprite collectable;
    public Collectables collectableType;
    public EatableType collectableCategory;
    public int collectionCount;
}

[System.Serializable]
public class Animal{
    public AnimalController animalController;
    public GameObject scoreBoardDisplay;
    public int score;
    public int animalCount;
}

public enum Collectables{
    Apple,
    Bannana,
    Berry,
    Grape,
    Grass,
    Meat,
    Orange,
    Pineapple
}

public enum AnimalType{
    Carnivores,
    Herbivores,
    Omnivores
}

public enum PlayMode{
    DemoGame,
    MainGame
}