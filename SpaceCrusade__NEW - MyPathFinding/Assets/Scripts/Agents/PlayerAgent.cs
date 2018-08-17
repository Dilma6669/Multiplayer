using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour {

	LocationManager _locationManager;

	MapSettings _mapSettings;

    SyncedVars _syncedVars;

    PlayerManager _playerManager;
	UIManager _uiManager;

	[HideInInspector]
	CameraAgent _cameraAgent;
    //
    //	[HideInInspector]
    //	public UnitsAgent _unitsAgent;


    public int _playerUniqueID = 0;
    public int _totalPlayers = -1;
    public int _seed = -1;


    Text totalPlayerText;
    Text playerNumText;
    Text seedNumText;


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
        playerNumText = _uiManager.transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        seedNumText = _uiManager.transform.FindDeepChild("SeedNum").GetComponent<Text>();

    }

    // Need this Start()
    void Start()
    {
        Debug.Log("A network Player object has been created");
        CreatePlayerAgent();
    }

    // DONT FUCKING TOUCH THIS FUNCTION
    void CreatePlayerAgent() {

        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        transform.SetParent (_playerManager.transform);
		_uiManager.GetComponent<Canvas>().enabled = true;

        _seed = _syncedVars.GlobalSeed;
        seedNumText.text = _seed.ToString();
        Random.InitState(_seed);

        _syncedVars.CmdTellServerToUpdatePlayerCount();

        if (isLocalPlayer)
        {
            _playerUniqueID = _syncedVars.PlayerCount;
            playerNumText.text = _playerUniqueID.ToString();
            ContinuePlayerSetUp();
        }
    }


    public void UpdatePlayerCount(int count)
    {
        _totalPlayers = count;
        totalPlayerText.text = _totalPlayers.ToString();
    }



    void ContinuePlayerSetUp()
	{
	    _cameraAgent.SetUpCameraAndLayers (_playerUniqueID);

        _locationManager.BuildMapForClient();

    }
	
}
