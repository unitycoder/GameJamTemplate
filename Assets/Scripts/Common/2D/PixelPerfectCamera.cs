using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour {

	public float pixelsToUnits = 100;

	Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void Update () {

		cam.orthographicSize = Screen.height/pixelsToUnits*0.5f;
	}
}
