using UnityEngine;

[ExecuteInEditMode]
public class ScaleCamera : MonoBehaviour {

	public int targetWidth = 640;
	public float pixelsToUnits = 100;

	Camera cam;
	int orthoHeight;

	void Start()
	{
		cam = Camera.main;
	}

	void Update() 
	{
		orthoHeight = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
		cam.orthographicSize = orthoHeight/pixelsToUnits*0.5f;
	}
}
