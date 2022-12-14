using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WayBlockerHandler : MonoBehaviour
{
    public GameObject blocker,
                    blockOpener;
    public Camera camera_;
    public PlayerController player;

    [Header("Block Opener")]
    public Transform startingPoint; 
    public Transform endingPoint;
    public Transform logPoint;

    [Header("Block")]
    public Transform startPoint;
    public Transform endPoint;
    public AudioClip woodScrapSound,
                    leafMovementSFX;
    public float movementSpeed,
                blockerMovementSpeed;
    float standingPosition;
    [SerializeField]
    bool canMoveDown,
            gateOpened,
            closeProgram,
            audioPlayed;
    Vector3 cameraPosition,
            blockerPosition,
            tmpPosition;
    float time;
    [SerializeField]
    float camerMovementDuration;
    AudioSource audioSource;

    private void Start() {
        time = 0;
        gateOpened = false;
        closeProgram = false;
        audioPlayed = false;
        // duration = 9f;
        audioSource = GetComponent<AudioSource>();
        cameraPosition = camera_.transform.position;

        blockerPosition = blocker.transform.position;
        blockerPosition.z = camera_.transform.position.z;
    }

    public void StartOpening(){
        canMoveDown = true;
    }

    private void Update() {
        if(closeProgram) { return; }

        if(!canMoveDown){
            cameraPosition = camera_.transform.position;
            return; 
        }

        if(Math.Round((endingPoint.position - logPoint.position).sqrMagnitude, 2) > 0.07f){
            if(!audioPlayed) {
                audioSource.clip = woodScrapSound;
                PlayAudio();
                audioPlayed=true;
            }
            OpenerMovement();
        }else{
            if(audioPlayed){
                StopAudio();
                audioPlayed=false;
            }
            CameraHandler.OBJ_followingCamera.B_canfollow = false;
            PlayerController.instance.LockInput();
            CameraMovement();
        }
    }

    void CameraMovement(){
        if(time > camerMovementDuration){
            if(gateOpened) {

                if(audioPlayed){
                    StopAudio();
                    audioPlayed=false;
                }

                CameraHandler.OBJ_followingCamera.B_canfollow = true;
                PlayerController.instance.UnLockInput();
                closeProgram = true;
            }else{
                if(!audioPlayed){
                    audioSource.clip = leafMovementSFX;
                    PlayAudio();
                    audioPlayed=true;
                }
                BlockerMovement();
            }
            return;
        }
        camera_.transform.position = Vector3.Lerp(cameraPosition, blockerPosition, time/camerMovementDuration);
        time += Time.deltaTime;
    }

    void OpenerMovement(){
        blockOpener.transform.position = new Vector3(
            blockOpener.transform.position.x,
            blockOpener.transform.position.y - (movementSpeed * Time.deltaTime),
            blockOpener.transform.position.z
        );
    }

    void BlockerMovement(){
        if(Math.Round((endPoint.position - startPoint.position).sqrMagnitude, 2) < 0.07f){
            tmpPosition = cameraPosition;
            cameraPosition = blockerPosition;
            blockerPosition = tmpPosition;
            time = 0f;
            gateOpened = true;
            return;
        }
        blocker.transform.position = new Vector3(
            blocker.transform.position.x,
            blocker.transform.position.y + (blockerMovementSpeed * Time.deltaTime),
            blocker.transform.position.z
        );
    }

    void PlayAudio(){
        audioSource.Play();
    }

    void StopAudio(){
        audioSource.Stop();
    }

}
