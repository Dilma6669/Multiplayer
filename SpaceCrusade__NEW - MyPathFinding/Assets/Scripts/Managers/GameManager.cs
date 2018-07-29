using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static GameManager instance = null;


	[HideInInspector]
	public LocationManager _locationManager;
	[HideInInspector]
	public CameraManager _cameraManager;
	[HideInInspector]
	public UnitsManager _unitsManager;
//	public UIManager _uiManager;
//	public NetworkManager _networkManager;




	void Awake() {

		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_locationManager = GetComponentInChildren<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}


		_cameraManager = GetComponentInChildren<CameraManager> ();
		if(_cameraManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_unitsManager = GetComponentInChildren<UnitsManager> ();
		if(_unitsManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


	void Start() {

	//	_locationManager.BuildMap ();

		//_unitsManager.LoadPlayersUnitAgents();
		//_cameraManager.LoadPlayersCameraAgents();
			
	}

	// this is because maps are built in an IEnumerator now
	public void MapsFinishedLoading() {

//		_cubeManager.SetUpPanelCubeScripts ();
//
//		_cubeManager.SetCubeNeighbours ();
//
//		_unitManager.LoadPlayers ();

		//	_cubeManager.GetCubeConnections ();

		//		_uiManager.PutFloorsIntoLists ();
		//		_uiManager.PutCeilingsIntoLists ();

	}

}

