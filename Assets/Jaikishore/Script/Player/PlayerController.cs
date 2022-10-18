using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed, jumpForce;
    public float interactableDistance, hearableDistance;
    public static PlayerController instance;
    public bool inGround;
    public float runSFXElapsedTime,
                camerMovementDuration;
    public LayerMask animalLayer;
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
    bool canJump,
        playerDead,
        blockInput;
    Collider2D[] animalsNearby;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        playerInitialPosition = transform.position;
        walkSpeed = movementSpeed;
        canJump = true;
        instance = this;
        blockInput = false;
        playerDead = false;
        // time = 0;
    }

    void Update()
    {
        InputHandler();

        // check animal nearby
        DetectNearbyAnimal();

        elapsedTime += Time.deltaTime;
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

    void PlayRunSFX(){
        if(!audioSource.isPlaying && canJump && elapsedTime > runSFXElapsedTime){
            audioSource.clip = runClip;
            audioSource.Play();
            elapsedTime = 0;
        }
    }

    private void InputHandler()
    {
        if(!blockInput) {
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)))
            {
                if(canJump){
                    PlayerAnimationHandler(PlayerMovement.Jump);
                    PlayerMovementHandler(PlayerMovement.Jump);
                    audioSource.clip = jumpStart;
                    audioSource.Play();
                    return;
                }
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                PlayerAnimationHandler(PlayerMovement.Run);
                PlayerMovementHandler(PlayerMovement.Run);
                PlayRunSFX();
            }
        }

        if (!Input.anyKey && canJump)
        {
            playerAnimator.Play("player_idle");
        }
    }

    void PlayerMovementHandler(PlayerMovement playerMovement){
        playerPosition = gameObject.transform;
        switch(playerMovement){
            case PlayerMovement.Run:
                playerPosition.position = new Vector3(
                    playerPosition.position.x + ((Input.GetAxis("Horizontal") * movementSpeed) * Time.deltaTime),
                    playerPosition.position.y,
                    playerPosition.position.z
                );

                if(Input.GetAxis("Horizontal") > 0 && playerPosition.localScale.x < 0){
                    playerPosition.localScale = new Vector3(
                       Mathf.Abs(playerPosition.localScale.x),
                       playerPosition.localScale.y,
                       playerPosition.localScale.z
                    );
                }
                else if(Input.GetAxis("Horizontal") < 0 && playerPosition.localScale.x > 0){
                    playerPosition.localScale = new Vector3(
                       -(playerPosition.localScale.x),
                       playerPosition.localScale.y,
                       playerPosition.localScale.z
                    );
                }

                dustParticle.Play();
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

        if (!canJump) return;

        switch(playerMovement){
            case PlayerMovement.Run:
                playerAnimator.Play("player_run_1");
                break;
            case PlayerMovement.Jump:
                playerAnimator.Play("player_jump");
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

    }

    private void OnCollisionStay2D(Collision2D other) {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "MovingFloor" && !playerDead){
            transform.position = new Vector3(
                other.transform.position.x,
                transform.position.y,
                transform.position.z
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // if(other.gameObject.tag == "Floor" && !inGround) movementSpeed = 0;

        if(other.gameObject.tag == "Fruit" || other.gameObject.tag == "Meat"){

            audioSource.clip = MainGameController.instance.fruitGainAudio;
            audioSource.Play();
            
            if(other.gameObject.name.Contains("apple")){
                // Debug.Log("Its apple");
                MainGameController.instance.CollectCollectable(Collectables.Apple);
            }
            if(other.gameObject.name.Contains("bannana")){
                // Debug.Log("Its bannana");
                MainGameController.instance.CollectCollectable(Collectables.Bannana);
            }
            if(other.gameObject.name.Contains("berry")){
                // Debug.Log("Its berry");
                MainGameController.instance.CollectCollectable(Collectables.Berry);
            }
            if(other.gameObject.name.Contains("grape")){
                // Debug.Log("Its grape");
                MainGameController.instance.CollectCollectable(Collectables.Grape);
            }
            if(other.gameObject.name.Contains("meat")){
                // Debug.Log("Its meat");
                MainGameController.instance.CollectCollectable(Collectables.Meat);
            }
            if(other.gameObject.name.Contains("orange")){
                // Debug.Log("Its orange");
                MainGameController.instance.CollectCollectable(Collectables.Orange);
            }
            if(other.gameObject.name.Contains("pineapple")){
                // Debug.Log("Its pineapple");
                MainGameController.instance.CollectCollectable(Collectables.Pineapple);
            }
            Destroy(other.gameObject);
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
    Crouch
}