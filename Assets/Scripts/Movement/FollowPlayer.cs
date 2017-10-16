using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField]
	private float damping = 0.2f;
	[SerializeField]
	private GameObject player;
	private Vector3 currentVelocity;
	private Vector3 initialOffset;

	void Start()
	{
		initialOffset = transform.position;
	}

	void Update()
	{
		Vector3 referencePos = player.transform.position;
		Vector3 cameraPos = transform.position;

		// Damps the camera position
		cameraPos.x = referencePos.x;
		cameraPos.z = referencePos.z + initialOffset.z;
		Vector3 targetPos = Vector3.SmoothDamp(transform.position, cameraPos,
			ref currentVelocity, damping);
		transform.position = targetPos;
	}
}