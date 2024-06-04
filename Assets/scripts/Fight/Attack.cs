using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour,IAttack
{

    
    bool IAttack.finished { get; set; }
    


     IEnumerator IAttack.MakeMove()
    {
        throw new System.NotImplementedException();
    }

    
}
