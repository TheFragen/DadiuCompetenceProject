using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 649
[RequireComponent(typeof(HumanMotor))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float _movementDebounce;
    [SerializeField]
    private bool _hoveringUi;

    private Camera _camera;
	private HumanMotor _motor;
    private float _lastClickTime = 0;

    void Start ()
	{
		_camera = Camera.main;
		_motor = GetComponent<HumanMotor>();
	}
	
	void Update ()
	{
		if (Input.GetMouseButton(0) && Time.time > _lastClickTime + _movementDebounce)
		{
            OnMouseClick();
		    _lastClickTime = Time.time;
		}
	}

    public void SetHovering()
    {
        _hoveringUi = !_hoveringUi;
    }

    void OnMouseClick()
	{
	    if (_hoveringUi)
	    {
	        return;
	    }
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
