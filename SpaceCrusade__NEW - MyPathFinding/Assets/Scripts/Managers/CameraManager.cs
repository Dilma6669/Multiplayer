using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	// Layer INfo
    [HideInInspector]
	public int maxLayer = 5; // This needs to change with the amout of y levels, basicly level*2 because of vents layer ontop of layer
    [HideInInspector]
    public int minLayer = 0;
    [HideInInspector]
    public int startLayer = 0;

	void Awake() {

	}


	public KeyValuePair<Vector3, Vector3> GetCameraStartPosition(int playerID) {

        Debug.Log("Finding Camera for player: " + playerID);

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
