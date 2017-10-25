using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State { 
    
    public IdleState(AI ai) : base(ai)
    {
    }

    public override void Tick()
    {
        _ai.transform.Rotate(new Vector3(0,5,0));
    }
}
