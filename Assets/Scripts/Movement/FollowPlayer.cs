using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField]
	private float damping = 0.2f;
    [SerializeField]
	private GameObject _player;
	private Vector3 _currentVelocity;
	private Vector3 _initialOffset;

	void Start()
	{
		_initialOffset = transform.position;
	}

	void Update()
	{
	    if (_player == null)
	    {
	        return;
	    }
		Vector3 referencePos = _player.transform.position;
		Vector3 cameraPos = transform.position;

		// Damps the camera position
		cameraPos.x = referencePos.x;
		cameraPos.z = referencePos.z + _initialOffset.z;
		Vector3 targetPos = Vector3.SmoothDamp(transform.position, cameraPos,
			ref _currentVelocity, damping);
		transform.position = targetPos;
	}

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }
}