using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : AbstractItem
{
    public override void Use()
    {
        base.Use();
        GameManager.Instance.IncreaseAction(owner, 2);
        JuiceController.Instance.SetItemUsedText(itemName +" used.");
    }
}