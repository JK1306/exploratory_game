using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalController : MonoBehaviour
{
    public AnimalType animalCategory;
    public string animalName;
    public AudioClip animalSFX,
                    disappearSFX;
    public ParticleSystem disappearEffect;
    AudioSource audioSource;
    ParticleSystem spawnedParticle;

    [SerializeField]
    int sfxPlayInterval;
    float sfxPlayed;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sfxPlayed = sfxPlayInterval;
    }

    public void PlayAnimalSFX(){
        audioSource.volume = 1;
        audioSource.clip = animalSFX;
        audioSource.Play();
    }

    public IEnumerator AnimalFeeded(){
        GetComponent<SpriteRenderer>().enabled = false;
        spawnedParticle = Instantiate(disappearEffect, gameObject.transform);
        audioSource.clip = disappearSFX;
        audioSource.Play();
        spawnedParticle.Play();
        yield return StartCoroutine(nameof(StartDestroying));
    }

    public void PlayAnimalHearableSFX(){
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
        if(audioSource.isPlaying || (sfxPlayInterval > sfxPlayed)) { return; }

        audioSource.volume = 0.15f;
        audioSource.clip = animalSFX;
        audioSource.Play();
        sfxPlayed = 0f;
    }

    IEnumerator StartDestroying(){
        while(spawnedParticle.isPlaying){
            yield return null;
        }
        Destroy(gameObject);
    }

    void Update()
    {
        sfxPlayed += Time.deltaTime;
    }
}
