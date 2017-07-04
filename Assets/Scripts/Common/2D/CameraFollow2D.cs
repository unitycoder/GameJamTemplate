using UnityEngine;
using System.Collections;

public enum FollowType{
	Strict,
	Smooth
}

public class CameraFollow2D : MonoBehaviour {

	public Transform leader;
	public FollowType followType;

	public float smoothness = 1f;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - leader.position;
	}

	void LateUpdate () {
		updateCameraPosition (followType);
	}

	void updateCameraPosition(FollowType type){
		switch (type) {
		case FollowType.Strict:
			transform.position = leader.position + offset;
			break;
		case FollowType.Smooth:
			transform.position = Vector3.Lerp (transform.position, leader.position + offset, Time.deltaTime * smoothness);
			break;
		}
	}
}
