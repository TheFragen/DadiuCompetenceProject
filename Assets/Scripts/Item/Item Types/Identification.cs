using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : AbstractItem
{
    public override void Use()
    {
        base.Use();
        if(owner.tag == "Player")
        {
            JuiceController.Instance.SelectPlayer(owner);
        }
        
    }
}