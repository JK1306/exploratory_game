using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed, jumpForce;
    float walkSpeed;
    Animator playerAnimator;
    Transform playerPosition;
    Rigidbody2D rb;
    bool canJump, inGround;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        walkSpeed = movementSpeed;
        rb = GetComponent<Rigidbody2D>();
        canJump = true;
    }

    void Update()
    {
        InputHandler();
    }

    private void InputHandler()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)))
        {
            if(canJump){
                PlayerAnimationHandler(PlayerMovement.Jump);
                PlayerMovementHandler(PlayerMovement.Jump);
                return;
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            PlayerAnimationHandler(PlayerMovement.Run);
            PlayerMovementHandler(PlayerMovement.Run);
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

                gameObject.transform.position = playerPosition.position;
                gameObject.transform.localScale = playerPosition.localScale;
                break;

            case PlayerMovement.Jump:
                rb.velocity = Vector2.zero;
                rb.AddForce( jumpForce * gameObject.transform.up, ForceMode2D.Impulse);
                canJump = false;
                break;
        }
    }

    void PlayerAnimationHandler(PlayerMovement playerMovement){

        if (!canJump) return;

        switch(playerMovement){
            case PlayerMovement.Run:
                playerAnimator.Play("player_run");
                break;
            case PlayerMovement.Jump:
                playerAnimator.Play("player_jump");
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Floor"){
            canJump = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("0---> "+other.bounds);
        if(other.gameObject.tag == "Floor") movementSpeed = 0;

        Debug.Log("Name : "+other.gameObject.name);
        Debug.Log("Tag : "+other.gameObject.tag);
        if(other.gameObject.tag == "Fruit" || other.gameObject.tag == "Meat"){
            if(other.gameObject.name.Contains("apple")){
                MainGameController.instance.CollectCollectable(Collectables.Apple);
            }
            if(other.gameObject.name.Contains("bannana")){
                MainGameController.instance.CollectCollectable(Collectables.Bannana);
            }
            if(other.gameObject.name.Contains("berry")){
                MainGameController.instance.CollectCollectable(Collectables.Berry);
            }
            if(other.gameObject.name.Contains("grape")){
                MainGameController.instance.CollectCollectable(Collectables.Grape);
            }
            if(other.gameObject.name.Contains("meat")){
                MainGameController.instance.CollectCollectable(Collectables.Meat);
            }
            if(other.gameObject.name.Contains("orange")){
                MainGameController.instance.CollectCollectable(Collectables.Orange);
            }
            if(other.gameObject.name.Contains("pineapple")){
                MainGameController.instance.CollectCollectable(Collectables.Pineapple);
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        movementSpeed = walkSpeed;
        Debug.Log(other.gameObject.name);
    }
}

public enum PlayerMovement{
    Idle,
    Run,
    Jump,
    Crouch
}