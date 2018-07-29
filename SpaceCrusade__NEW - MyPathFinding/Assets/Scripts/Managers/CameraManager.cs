using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

//	private static CameraManager instance = null;

	// Layer INfo
	public int maxLayer = 3; // This needs to change with the amout of y levels, basicly level*2 because of vents layer ontop of layer
	public int minLayer = 0;
	public int startLayer = 1;

	void Awake() {

//		if (instance == null)
//			instance = this;
//		else if (instance != this) {
//			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
//			Destroy (gameObject);
//		}
	}


	public KeyValuePair<Vector3, Vector3> GetCameraStartPosition(int playerID) {

		List<KeyValuePair<Vector3, Vector3>> cameraPositions = new List<KeyValuePair<Vector3, Vector3>> ();

		KeyValuePair<Vector3, Vector3> cam0 = new KeyValuePair<Vector3, Vector3> (new Vector3 (11, 9, -10), new Vector3 (26, 0, 0));
		KeyValuePair<Vector3, Vector3> cam1 = new KeyValuePair<Vector3, Vector3> (new Vector3 (11, 9, 34), new Vector3 (26, 180, 0));
		KeyValuePair<Vector3, Vector3> cam2 = new KeyValuePair<Vector3, Vector3> (new Vector3 (31, 9, 11.5f), new Vector3 (26, 267, 0));
		KeyValuePair<Vector3, Vector3> cam3 = new KeyValuePair<Vector3, Vector3> (new Vector3 (-9, 9, 11), new Vector3 (26, 90, 0));

		cameraPositions.Add (cam0);
		cameraPositions.Add (cam1);
		cameraPositions.Add (cam2);
		cameraPositions.Add (cam3);

		return cameraPositions [playerID];
	}
}
