using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    [Header("Player Options")]
    public GameObject player;
    public float playerMovementSpeed, jumpForce;
    Animator playerAnimator;
    Transform playerTranform;
    Rigidbody2D rb2d;
    bool canJump;
    
    void Start()
    {
        canJump = true;
        playerAnimator = player.GetComponent<Animator>();
        rb2d = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputHandler();
    }

#region PLAYER_CONTROLLER
    void PlayerInputHandler(){
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) && canJump){
            PlayerMovementHandler(PlayerMovementState.Jump);
            PlayerAnimationHandler(PlayerMovementState.Jump);
            canJump = false;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)){
            PlayerMovementHandler(PlayerMovementState.Run);
            PlayerAnimationHandler(PlayerMovementState.Run);
        }

        if(!Input.anyKey && canJump){
            playerAnimator.Play("player_idle");
        }
    }

    void PlayerMovementHandler(PlayerMovementState playerState){
        switch(playerState){
            case PlayerMovementState.Run:
                playerTranform = player.transform;
                playerTranform.position = new Vector3(
                    (playerTranform.position.x + (Input.GetAxis("Horizontal") * playerMovementSpeed) * Time.deltaTime),
                    playerTranform.position.y,
                    playerTranform.position.z
                );
                playerTranform.position = playerTranform.position;
                Debug.Log(playerTranform.position);
                break;
            
            case PlayerMovementState.Jump:
                rb2d.AddForce((Vector2.up * (jumpForce * Time.deltaTime * 100)), ForceMode2D.Impulse);
                break;
        }
    }

    void PlayerAnimationHandler(PlayerMovementState playerMovementState){
        switch(playerMovementState){
            case PlayerMovementState.Run:
                playerAnimator.Play("player_run");
                break;
            case PlayerMovementState.Jump:
                playerAnimator.Play("player_jump");
                break;
        }
    }
#endregion

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Floor"){
            canJump = true;
        }
    }

}

public enum PlayerMovementState{
    Idle,
    Run,
    Jump,
    Slide
}