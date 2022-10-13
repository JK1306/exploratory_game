using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public EatableType eatableType;
    public Collectables collectableName;
}

public enum EatableType{
    Fruit,
    Grass,
    Meat
}