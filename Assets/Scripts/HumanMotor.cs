using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

public class HumanMotor : MonoBehaviour
{
	public void Move(Vector2 direction)
	{
		Vector3 reference = transform.position;
		reference += direction.To3DXZ(0);
		transform.position = reference;
	}
}
