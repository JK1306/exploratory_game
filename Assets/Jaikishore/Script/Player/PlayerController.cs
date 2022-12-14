using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed, jumpForce;
    public float interactableDistance, hearableDistance;
    public static PlayerController instance;
    public bool inGround,
                // Used by demo script
                inFloatingFloor,
                blockInput,
                isPlayingDemo;
    public float runSFXElapsedTime,
                camerMovementDuration;
    public LayerMask animalLayer,
                    gameEndLayer;
    public ParticleSystem dustParticle,
                            deathParticle;
    public AudioClip jumpStart,
                    jumpEnd,
                    runClip,
                    deathClip,
                    playerRespawnClip;
    public Camera camera_;
    float walkSpeed,
            elapsedTime;
    Animator playerAnimator;
    Transform playerPosition,
                cameraPosition;
    Vector3 playerInitialPosition,
            deathPosition,
            respawnPosition;
    Rigidbody2D rb;
    AudioSource audioSource;
    public float movementDirection;
    public bool jump;
    float movement;
    bool canJump,
        playerDead,
        // reachedEnd,
        playerJumped;
    Collider2D[] animalsNearby,
                gameEndColliders;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        playerInitialPosition = transform.position;
        walkSpeed = movementSpeed;
        instance = this;
        jump = false;
        canJump = true;
        blockInput = false;
        playerDead = false;
        // reachedEnd = false;
        playerJumped = false;
    }

    void Update()
    {

        InputHandler();

        // check animal nearby
        DetectNearbyAnimal();

        if(Input.anyKey) elapsedTime += Time.deltaTime;
    }

    public void SpawnPlayer(){
        if(playerDead){
            GetComponent<SpriteRenderer>().enabled = true;
            audioSource.clip = playerRespawnClip;
            audioSource.Play();
            playerDead = false;
        }
    }

    void DetectNearbyAnimal(){
        animalsNearby = Physics2D.OverlapCircleAll(transform.position, interactableDistance, animalLayer);
        if(animalsNearby.Length > 0){
            foreach (Collider2D animal in animalsNearby) {
                if(animal.gameObject.GetComponent<AnimalController>()){
                    if(!MainGameController.instance.animalNearby){
                        MainGameController.instance.nearbyAnimalContorller = animal.gameObject.GetComponent<AnimalController>();
                        MainGameController.instance.nearbyAnimalContorller.PlayAnimalSFX();
                        MainGameController.instance.inventoryButton.transform.parent.GetComponent<Animator>().enabled = true;
                        // Debug.Log("Animator Enabled : "+MainGameController.instance.inventoryButton.GetComponent<Animator>().enabled);
                        MainGameController.instance.inventoryButton.transform.parent.GetComponent<Animator>().Play("highlight_animation");
                        MainGameController.instance.animalNearby = true;
                    }
                    // MainGameController.instance.animalName = animal.gameObject.name;
                    // MainGameController.instance.nearByAnimal = animal.gameObject;
                    break;
                }
            }
        }else{
            MainGameController.instance.animalNearby = false;
            MainGameController.instance.inventoryButton.transform.parent.GetComponent<Animator>().enabled = false;
            HearAnimalSound();
        }
    }

    void HearAnimalSound(){
        animalsNearby = Physics2D.OverlapCircleAll(transform.position, hearableDistance, animalLayer);
        // Debug.Log("Hearable distance : "+animalsNearby.Length.ToString());
        if(animalsNearby.Length > 0){
            foreach (Collider2D animal in animalsNearby)
            {
                if(animal.gameObject.GetComponent<AnimalController>()){
                    animal.gameObject.GetComponent<AnimalController>().PlayAnimalHearableSFX();
                    break;
                }
            }
        }
    }

    public void InputHandler()
    {
        if(!blockInput) {
            bool verticalInput = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.X) || jump);
            if (verticalInput)
            {
                if(canJump){
                    playerJumped = true;
                    PlayerAnimationHandler(PlayerMovement.Jump);
                    PlayerMovementHandler(PlayerMovement.Jump);
                    audioSource.clip = jumpStart;
                    audioSource.Play();
                    // return;
                }
            }

            bool horizontalInput = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || (movementDirection != 0));
            if (horizontalInput)
            {
                PlayerMovementHandler(PlayerMovement.Run);
                if(!inGround){
                    canJump = false;
                    PlayerAnimationHandler(PlayerMovement.Fall);
                    // return;
                }else{
                    canJump = true;
                    PlayerAnimationHandler(PlayerMovement.Run);
                }
                PlayRunSFX();
            }
        }

        if (!Input.anyKey && canJump && inGround)
        {
            playerAnimator.Play("player_idle");
        }
    }

    void PlayRunSFX(){
        if(!audioSource.isPlaying && canJump && elapsedTime > runSFXElapsedTime){
            audioSource.clip = runClip;
            audioSource.Play();
            elapsedTime = 0;
        }
    }

    void PlayerMovementHandler(PlayerMovement playerMovement){
        playerPosition = gameObject.transform;
        movement = (movementDirection != 0)? movementDirection : Input.GetAxis("Horizontal");
        switch(playerMovement){
            case PlayerMovement.Run:
                playerPosition.position = new Vector3(
                    playerPosition.position.x + (movement * movementSpeed * Time.deltaTime),
                    playerPosition.position.y,
                    playerPosition.position.z
                );

                if(movement > 0 && playerPosition.localScale.x < 0){
                    playerPosition.localScale = new Vector3(
                       Mathf.Abs(playerPosition.localScale.x),
                       playerPosition.localScale.y,
                       playerPosition.localScale.z
                    );
                }
                else if(movement < 0 && playerPosition.localScale.x > 0){
                    playerPosition.localScale = new Vector3(
                       -(playerPosition.localScale.x),
                       playerPosition.localScale.y,
                       playerPosition.localScale.z
                    );
                }

                gameObject.transform.position = playerPosition.position;
                gameObject.transform.localScale = playerPosition.localScale;
                break;

            case PlayerMovement.Jump:
                if(inGround){
                    rb.velocity = Vector2.zero;
                    rb.AddForce( jumpForce * gameObject.transform.up, ForceMode2D.Impulse);
                    canJump = false;
                }
                break;
        }
    }

    void PlayerAnimationHandler(PlayerMovement playerMovement){

        if (playerJumped && playerMovement != PlayerMovement.Jump) return;

        switch(playerMovement){
            case PlayerMovement.Run:
                playerAnimator.Play("player_run_1");
                break;
            case PlayerMovement.Jump:
                playerAnimator.Play("player_jump");
                break;
            case PlayerMovement.Fall:
                playerAnimator.Play("player_fall");
                break;
        }
    }

    public void ResetMovementSpeed(){
        movementSpeed = walkSpeed;
    }

    public void LockInput(){
        blockInput = true;
    }

    public void UnLockInput(){
        blockInput = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {

        if(other.gameObject.tag == "Floor" || other.gameObject.tag == "Blocker" || other.gameObject.tag.ToLower().Contains("floor") || other.gameObject.tag == "GateOpener" ){
            if(!canJump){
                audioSource.clip = jumpEnd;
                audioSource.Play();
            }
            playerJumped = false;
            canJump = true;
        }

        if(other.transform.parent.GetComponent<WayBlockerHandler>()){
            // blockInput = true;
            other.transform.parent.GetComponent<WayBlockerHandler>().StartOpening();
        }

        if(other.gameObject.tag == "Hazard"){
            foreach(ContactPoint2D contact in other.contacts){
                // Debug.Log(contact.collider.gameObject.name+"--> "+contact.otherCollider.gameObject.name);
                // Debug.Log("Player dead");
                deathPosition = transform.position;
                CameraHandler.OBJ_followingCamera.B_canfollow = false;
                playerDead = true;
                transform.position = respawnPosition;
                MainGameController.instance.PlayerDeath(deathPosition, respawnPosition, camerMovementDuration);
                GetComponent<SpriteRenderer>().enabled = false;
                break;
            }
        }

        if(other.gameObject.tag == "MovingFloor"){
            Debug.Log("contact with moving floor");
            transform.parent = other.transform;
            other.transform.parent.GetComponent<MovableBlock>()?.ApplyFloatEffect();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "MovingFloor"){
            // other.transform.parent.GetComponent<MovableBlock>().ResetFloatEffect();
            transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if(other.gameObject.tag == "Fruit" || other.gameObject.tag == "Meat"){
            audioSource.clip = MainGameController.instance.fruitGainAudio;
            audioSource.Play();
            MainGameController.instance.CollectCollectable(other.GetComponent<CollectableController>().collectableName);
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "Collectable"){
            audioSource.clip = MainGameController.instance.fruitGainAudio;
            audioSource.Play();
            Destroy(other.gameObject);
            MainGameController.instance.AddPoints();
        }

        if(other.gameObject.tag == "Respawn"){
            respawnPosition = other.transform.position;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        movementSpeed = walkSpeed;
    }

    #region GIZMOS_LOGIC

    // void OnDrawGizmosSelected(){
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(transform.position, hearableDistance);
    // }

    #endregion

}

public enum PlayerMovement{
    Idle,
    Run,
    Jump,
    Crouch,
    Fall
}