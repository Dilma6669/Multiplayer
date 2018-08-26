using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LocationManager : MonoBehaviour {

    GameManager _gameManager;

    WorldBuilder _worldBuilder;
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


        _worldBuilder = GetComponentInChildren<WorldBuilder>();
        if (_worldBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

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

        StartCoroutine(BuildGridEnumerator());
    }

    // this is make the game actually start at startup and not wait loading
    private IEnumerator BuildGridEnumerator()
    {
        yield return new WaitForSeconds(1.0f);

        float buildTime = 0.1f;

        _worldBuilder.BuildWorldGrid(buildTime);

        List<Vector3> worldMapNodes = _worldBuilder.GetWorldMapNodes();


        // Build the Map Pieces
        for (int i = 0; i < worldMapNodes.Count; i++)
        {
            Vector3 mapNode = worldMapNodes[i];

            // Give us a list of Locations
            _gridBuilder.BuildLocationGrid(mapNode, buildTime, 0); // 0 = map

            //Debug.Log("_LocationLookup.Count: " + _LocationLookup.Count);

              List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();

            //  yield return new WaitForSeconds(1.0f);

             _mapPieceBuilder.AttachMapPieceToMapNode(mapPieceNodes, buildTime);

            //yield return new WaitForSeconds(buildTime);

        }




        Dictionary<Vector3, int> _connectPieceNodes = _worldBuilder.GetWorldConnectorNodes();

        Debug.Log("_connectPieceNodes.Count: " + _connectPieceNodes.Count);

        // Build Connector Pieces
        foreach (KeyValuePair<Vector3, int> node in _connectPieceNodes )
        {
            _gridBuilder.BuildLocationGrid(node.Key, buildTime, 1); // 1 = connectors

            List<Vector3> connectorPieceNodes = _gridBuilder.GetGridNodePositions();

            _connectorPieceBuilder.AttachConnectorPieceToMapNode (connectorPieceNodes, node.Value, buildTime);

            //yield return new WaitForSeconds(buildTime);
        }


        _LocationLookup = _gridBuilder.GetGridLocations();


        //   List<Vector3> connectPieceNodes = _gridBuilder.GetConnectNodes();
        //  _connectorPieceBuilder.AttachConnectorPieceToMapNode (connectPieceNodes);

        //  List<Vector3> shipPieceNodes = _gridBuilder.GetShipNodes();
        //_playerShipBuilder.AttachShipPieceToMapNode (shipPieceNodes);

        //	SetCubeNeighbours ();

        yield return null;
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
