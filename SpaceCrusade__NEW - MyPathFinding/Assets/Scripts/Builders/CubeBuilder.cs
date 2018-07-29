using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBuilder : MonoBehaviour {

	public static CubeBuilder instance = null;

	PanelBuilder _panelBuilder;
	ObjectBuilder _objectBuilder;

	private int rotationY = 0;

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_panelBuilder = GetComponentInChildren<PanelBuilder> ();
		if(_panelBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_objectBuilder = GetComponentInChildren<ObjectBuilder> ();
		if(_objectBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}
		

	public void CreateCubeObject(ref CubeLocationScript oldCubeScript, int cubeType, int rotations, int layerCount){

		rotationY = (rotations * -90) % 360;

		GameObject cubeObject = oldCubeScript.gameObject; // empty cube
		CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript> ();
		cubeObject.transform.eulerAngles = new Vector3 (0, rotationY, 0);
		cubeObject.gameObject.layer = LayerMask.NameToLayer ("Floor" + layerCount.ToString ());

		cubeScript.cubeAngle = (int)rotationY;

		switch (cubeType) {
		case 00:
			break;
		case 01:
			_panelBuilder.CreatePanelForCube ("Floor", cubeObject.transform, layerCount, 0, rotations);
			break;
		case 02:
			_panelBuilder.CreatePanelForCube ("Wall", cubeObject.transform, layerCount, 90, rotations); // Down
			break;
		case 03:
			_panelBuilder.CreatePanelForCube ("Wall", cubeObject.transform, layerCount, 0, rotations); // across
			break;
		case 04:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 90, rotations);
			break;
		case 05:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 270, rotations); 
			break;
		case 06:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 180, rotations);
			break;
		case 07:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 0, rotations);
			break;
		case 08:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 90, rotations);
			break;
		case 09:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 270, rotations); 
			break;
		case 10:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 180, rotations);
			break;
		case 11:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 0, rotations);
			break;
		}
	}

}
