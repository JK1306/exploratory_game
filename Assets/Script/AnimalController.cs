using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public AnimalType animalCategory;
    public string animalName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public enum AnimalType{
    Carnivores,
    Herbivores,
    Omnivores
}