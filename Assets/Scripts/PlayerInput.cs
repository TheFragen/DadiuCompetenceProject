﻿using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

[RequireComponent(typeof(HumanMotor))]
public class PlayerInput : MonoBehaviour
{
	private Camera _camera;
	private HumanMotor _motor;

	void Start ()
	{
		_camera = Camera.main;
		_motor = GetComponent<HumanMotor>();
	}
	
	void Update ()
	{
		if (Input.GetMouseButton(0))
		{
			OnMouseClick();
		}	
	}

	void OnMouseClick()
	{
		Vector3 mousePos = Input.mousePosition;
		Vector3 playerPosScreenSpace = _camera.WorldToScreenPoint(transform.position);

		Vector3 direction = mousePos - playerPosScreenSpace;
		Vector2 normalizedDirection = direction.normalized.To2DXY();
		
		if (normalizedDirection.x > .8)
		{
			_motor.Move(Vector2.right);
		}
		if (normalizedDirection.x < -.8) {
			_motor.Move(Vector2.left);
		}
		if (normalizedDirection.y > .8) {
			_motor.Move(Vector2.up);
		}
		if (normalizedDirection.y < -.8) {
			_motor.Move(Vector2.down);
		}
	}
}