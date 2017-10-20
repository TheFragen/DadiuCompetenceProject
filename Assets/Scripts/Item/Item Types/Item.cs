using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using Gamelogic.Extensions;
using UnityEngine;

public class Item : AbstractItem
{
    public override void Use()
    {
        base.Use();
        owner.transform.RotateAroundZ(10);
    }
}