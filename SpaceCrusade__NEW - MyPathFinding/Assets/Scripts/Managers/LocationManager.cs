using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LocationManager : NetworkBehaviour {

    GameManager _gameManager;

	GridBuilder _gridBuilder;
	MapPieceBuilder _mapPieceBuilder;
    ConnectorPieceBuilder _connectorPieceBuilder;
    PlayerShipBuilder _playerShipBuilder;

    CubeConnections _cubeConnections;

	MapSettings _mapSettings;


	public Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();

	void Awake() {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }



        _gridBuilder = GetComponentInChildren<GridBuilder> ();
		if(_gridBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapPieceBuilder = GetComponentInChildren<MapPieceBuilder> ();
		if(_mapPieceBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _connectorPieceBuilder = GetComponentInChildren<ConnectorPieceBuilder>();
        if (_connectorPieceBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!");}

        _playerShipBuilder = GetComponentInChildren<PlayerShipBuilder>();
        if (_playerShipBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }



        _cubeConnections = GetComponent<CubeConnections> ();
		if(_cubeConnections == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}

	}

	public void BuildMapForClient () {

		_gridBuilder.InitialiseGridManager ();
		_gridBuilder.StartGridBuilding();
		_LocationLookup = _gridBuilder.GetGridLocations ();

        List<Vector3> mapPieceNodes = _gridBuilder.GetMapNodes ();
        _mapPieceBuilder.AttachMapPieceToMapNode (mapPieceNodes);

        List<Vector3> connectPieceNodes = _gridBuilder.GetConnectNodes();
        _connectorPieceBuilder.AttachConnectorPieceToMapNode (connectPieceNodes);

        List<Vector3> shipPieceNodes = _gridBuilder.GetShipNodes();
        //_playerShipBuilder.AttachShipPieceToMapNode (shipPieceNodes);

        //	SetCubeNeighbours ();
    }




    public bool CheckIfLocationExists(Vector3 loc) {

		if (_LocationLookup.ContainsKey (loc)) {
			return true;
		}
		return false;
	}


	public CubeLocationScript GetLocationScript(Vector3 loc) {

		if (CheckIfLocationExists(loc)) {
			return _LocationLookup[loc];
		}
		//Debug.LogError ("Returning a null Location for: " + loc);
		return null;
	}

	public CubeLocationScript CheckIfCanMoveToCube(Vector3 loc) {

		CubeLocationScript cubeScript = GetLocationScript(loc);

		if (cubeScript != null) {
			if (cubeScript._cubeOccupied) {
				cubeScript = null;
			}
		}
		return cubeScript;
	}


	public void SetCubeNeighbours() {

		_cubeConnections.SetCubeNeighbours (_LocationLookup);

	}
}
