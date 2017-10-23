using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : AbstractItem {
    public override void Use()
    {
        base.Use();
        if (owner.tag == "Player")
        {
            TeleportManager.Instance.SetPlayerInUse(owner);
        }
        else
        {
            TeleportManager.Instance.AITeleport(owner);
        }
        
    }
}
