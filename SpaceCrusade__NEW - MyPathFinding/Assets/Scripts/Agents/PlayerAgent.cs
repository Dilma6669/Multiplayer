using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour {

	LocationManager _locationManager;

	MapSettings _mapSettings;

	PlayerManager _playerManager;
	UIManager _uiManager;

	[HideInInspector]
	CameraAgent _cameraAgent;
//
//	[HideInInspector]
//	public UnitsAgent _unitsAgent;



	[SyncVar]
	public string SERVER_mapRules = "";

	[SyncVar]
	public string mapRules = "";


	[SyncVar]
	public int SERVER_totalPlayers = -1;

	public int _totalPlayers = -1;
	public int _playerUniqueID = -1;

	Text totalPlayerText;
	Text playerNUmText;


	// Use this for initialization
	void Awake () {

		_playerManager = FindObjectOfType<PlayerManager> ();
		if(_playerManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_uiManager = FindObjectOfType<UIManager> ();
		if(_uiManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cameraAgent = GetComponent<CameraAgent> ();
		if(_cameraAgent == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = FindObjectOfType<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}

//		_unitsAgent = GetComponentInChildren<UnitsAgent> ();
//		if(_unitsAgent == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_locationManager = FindObjectOfType<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		//_cameraAgent._playerAgent = this;
		//_cameraAgent._camera = GetComponent<Camera> ();
		//_unitsAgent._playerAgent = this;
	
		totalPlayerText = _uiManager.transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
		playerNUmText = _uiManager.transform.FindDeepChild("PlayerNum").GetComponent<Text>();

	}

	void Start()
	{
		Debug.Log ("A network Player object has been created");
		CreatePlayerAgent ();
	}


	void CreatePlayerAgent() {

		transform.SetParent (_playerManager.transform);
		_uiManager.GetComponent<Canvas>().enabled = true;

		CmdGetPlayerIDFromServer(); 

	}

		
	[Command] //Commands - which are called from the client and run on the server;
	void CmdGetPlayerIDFromServer() {
		SERVER_totalPlayers = GetConnectionCount();
		RpcUpdatePlayerIDAtClient();
	}
	[ClientRpc] //ClientRpc calls - which are called on the server and run on clients
	void RpcUpdatePlayerIDAtClient() {
		UpdatePlayerID ();
	}
	void UpdatePlayerID(){
		_totalPlayers = SERVER_totalPlayers;
		totalPlayerText.text = _totalPlayers.ToString ();

		if (isLocalPlayer) {
			int ID = SERVER_totalPlayers - 1;
			if (ID <= 0) {
				ID = 0;
			}
			_playerUniqueID = ID;
			playerNUmText.text = _playerUniqueID.ToString ();

			ContinuePlayerSetUp (); 

		}

		UpdateMapRules ();

		_locationManager.BuildMapForClient (mapRules);
	}



	void ContinuePlayerSetUp()
	{
			_cameraAgent.SetUpCameraAndLayers (_playerUniqueID);

	}
		

	[Command] //Commands - which are called from the client and run on the server;
	public void CmdGetMapRulesFromServer() {

		Debug.Log ("CALCULATING RULES<<");
		//string rules = _locationManager.BuildMapForHost ();
		SERVER_mapRules = _locationManager.BuildMapForHost ();

		RpcUpdateMapRulesAtClient ();
	}

	[ClientRpc] //ClientRpc calls - which are called on the server and run on clients
	void RpcUpdateMapRulesAtClient() {
		UpdateMapRules ();
	}
	void UpdateMapRules(){
		mapRules = SERVER_mapRules;
		Debug.Log ("mapRules: " + mapRules);
	}
//
//

	// Network
	int GetConnectionCount()
	{
		int count = 0;
		foreach (NetworkConnection con in NetworkServer.connections)
		{
			if (con != null)
				count++;
		}
		return count;
	}
		
}
