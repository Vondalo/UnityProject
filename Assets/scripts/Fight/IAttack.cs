using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAttack
{
    protected bool finished { get; set; }
    public IEnumerator MakeMove();


}
