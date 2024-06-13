using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character Selection/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public GameObject characterPrefab;
}
