using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : NetworkManager {

	CameraManager _cameraManager;

	PlayerAgent _playerAgent;

	LocationManager _locationManager;

	private static NetWorkManager instance = null;

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_cameraManager = FindObjectOfType<CameraManager> ();

		_locationManager = FindObjectOfType<LocationManager> ();

	}

	public override void OnStartHost()
	{
		Debug.Log("OnStartHost");
		_playerAgent = FindObjectOfType<PlayerAgent> ();
		Invoke("CreateRules", 1);
	}

	private void CreateRules()
	{
		_playerAgent = FindObjectOfType<PlayerAgent> ();
		_playerAgent.CmdGetMapRulesFromServer();
	}


	public override void OnStartClient(NetworkClient client)
	{
		Debug.Log("OnStartClient");
	}


	public override void OnServerConnect(NetworkConnection Conn)
	{
		if (Conn.hostId >= 0)
		{
			Debug.Log("New Player has joined");
		}
	}


}
