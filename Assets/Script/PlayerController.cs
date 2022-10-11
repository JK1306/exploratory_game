using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed, jumpForce;
    public float interactableDistance, hearableDistance;
    public static PlayerController instance;
    public bool inGround;
    public LayerMask animalLayer;
    public ParticleSystem dustParticle;
    public AudioClip jumpStart,
                    jumpEnd,
                    runClip;
    float walkSpeed;
    Animator playerAnimator;
    Transform playerPosition;
    Rigidbody2D rb;
    AudioSource audioSource;
    bool canJump; 
    Collider2D[] animalsNearby;


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        walkSpeed = movementSpeed;
        canJump = true;
        instance = this;
    }

    void Update()
    {
        InputHandler();

        // check animal nearby
        DetectNearbyAnimal();
    }

    void DetectNearbyAnimal(){
        animalsNearby = Physics2D.OverlapCircleAll(transform.position, interactableDistance, animalLayer);
        if(animalsNearby.Length > 0){
            foreach (Collider2D animal in animalsNearby) {
                if(animal.gameObject.GetComponent<AnimalController>()){
                    MainGameController.instance.nearbyAnimalContorller = animal.gameObject.GetComponent<AnimalController>();
                    MainGameController.instance.animalNearby = true;
                    // MainGameController.instance.animalName = animal.gameObject.name;
                    // MainGameController.instance.nearByAnimal = animal.gameObject;
                    break;
                }
            }
        }else{
            MainGameController.instance.animalNearby = false;
            HearAnimalSound();
        }
    }

    void HearAnimalSound(){
        animalsNearby = Physics2D.OverlapCircleAll(transform.position, hearableDistance, animalLayer);
        Debug.Log("Hearable distance : "+animalsNearby.Length.ToString());
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
        if(!audioSource.isPlaying && canJump){
            audioSource.clip = runClip;
            audioSource.Play();
        }
    }

    private void InputHandler()
    {
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

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Floor" || other.gameObject.tag == "Blocker" || other.gameObject.tag.ToLower().Contains("floor")){
            if(!canJump){
                audioSource.clip = jumpEnd;
                audioSource.Play();
            }

            canJump = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "MovingFloor"){
            transform.position = new Vector3(
                other.transform.position.x,
                transform.position.y,
                transform.position.z
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // if(other.gameObject.tag == "Floor" && !inGround) movementSpeed = 0;

        // Debug.Log("Name : "+other.gameObject.name);
        // Debug.Log("Tag : "+other.gameObject.tag);
        if(other.gameObject.tag == "Fruit" || other.gameObject.tag == "Meat"){

            audioSource.clip = MainGameController.instance.fruitGainAudio;
            audioSource.Play();
            Debug.Log("It came here");

            if(other.gameObject.name.Contains("apple")){
                Debug.Log("Its apple");
                MainGameController.instance.CollectCollectable(Collectables.Apple);
            }
            if(other.gameObject.name.Contains("bannana")){
                Debug.Log("Its bannana");
                MainGameController.instance.CollectCollectable(Collectables.Bannana);
            }
            if(other.gameObject.name.Contains("berry")){
                Debug.Log("Its berry");
                MainGameController.instance.CollectCollectable(Collectables.Berry);
            }
            if(other.gameObject.name.Contains("grape")){
                Debug.Log("Its grape");
                MainGameController.instance.CollectCollectable(Collectables.Grape);
            }
            if(other.gameObject.name.Contains("meat")){
                Debug.Log("Its meat");
                MainGameController.instance.CollectCollectable(Collectables.Meat);
            }
            if(other.gameObject.name.Contains("orange")){
                Debug.Log("Its orange");
                MainGameController.instance.CollectCollectable(Collectables.Orange);
            }
            if(other.gameObject.name.Contains("pineapple")){
                Debug.Log("Its pineapple");
                MainGameController.instance.CollectCollectable(Collectables.Pineapple);
            }
            Destroy(other.gameObject);
        }

        // if(other.gameObject.tag == "Animal"){
        //     Debug.Log("Animal near me");
        // }
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