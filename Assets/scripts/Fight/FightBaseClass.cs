using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightBaseClass : MonoBehaviour
{
    public bool finished { get; protected set; }
    protected Attack[] attacks;
  

    
    
    protected IEnumerator BotMoveRandom()
    {
        Attack attack = attacks[Random.Range(0, attacks.Length)];
        yield return attack;
    }
}
