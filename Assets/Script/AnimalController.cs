using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalController : MonoBehaviour
{
    public AnimalType animalCategory;
    public string animalName;
    public AudioClip animalSFX;
    public ParticleSystem disappearEffect;
    public GameObject scoreBoardList;
    public Color color;
    AudioSource audioSource;
    [SerializeField]
    int sfxPlayInterval;
    float sfxPlayed;
    int score;

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

    public void PlayAnimalHearableSFX(){
        if(audioSource.isPlaying || (sfxPlayInterval > sfxPlayed)) { return; }

        audioSource.volume = 0.15f;
        audioSource.clip = animalSFX;
        audioSource.Play();
        sfxPlayed = 0f;
    }

    public void AddScore(){
        // scoreBoardList.transform.GetChild(score).gameObject.GetComponent<Image>().color = color;
        score++;
    }

    void Update()
    {
        sfxPlayed += Time.deltaTime;
    }
}

public enum AnimalType{
    Carnivores,
    Herbivores,
    Omnivores
}