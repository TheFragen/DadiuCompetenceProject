using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : AbstractItem
{
    public override void Use()
    {
        base.Use();
        JuiceController.Instance.SelectPlayer(owner);
    }
}