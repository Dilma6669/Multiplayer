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
        float buildTime = 0.1f;

        yield return new WaitForSeconds(buildTime);

        _worldBuilder.BuildWorldGrid(buildTime);

        yield return new WaitForSeconds(buildTime);


        
        List<Vector3> worldMapNodes = _worldBuilder.GetWorldMapNodes();

        int total = (_mapSettings.worldSizeXZ * _mapSettings.worldSizeXZ) - 1;
        int layerCount = -1;

        // Build the Map Pieces
        for (int i = 0; i < worldMapNodes.Count; i++)
        {
            Vector3 mapNode = worldMapNodes[i];

            // Give us a list of Locations
            _gridBuilder.BuildLocationGrid(mapNode, buildTime, 0); // 0 = map

            List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();

            _mapPieceBuilder.AttachMapPieceToMapNode(mapPieceNodes, layerCount, buildTime);

            if (i % total == 0 && i != 0 && i != (worldMapNodes.Count - 2))
            {
                //Debug.Log("fucken count <<<<<: " + layerCount);
                layerCount += (_mapSettings.numMapPiecesY * 2) + 2; // +2 for connectors going up floor AND roof
            }

            yield return new WaitForSeconds(buildTime);
        }



        ////

        int rowNoMap = (_mapSettings.worldSizeXZ * _mapSettings.numMapPiecesXZ);
        int rowYesMap = (_mapSettings.worldSizeXZ + 1);

        int startX = (int)Mathf.Ceil(_mapSettings.numMapPiecesXZ / 2);
        int spaceBetweenX = _mapSettings.numMapPiecesXZ;

        int startZ = (_mapSettings.worldSizeXZ * _mapSettings.numMapPiecesXZ) + _mapSettings.numMapPiecesXZ;
        int spaceBetweenZ = _mapSettings.numMapPiecesXZ;

        int connectorPosX = (int)Mathf.Ceil(_mapSettings.numMapPiecesXZ / 2) - 2;
        int connectorPosY = (int)Mathf.Ceil(_mapSettings.numMapPiecesY / 2) - 2;

        //////

        Dictionary<Vector3, int> _connectPieceNodes = _worldBuilder.GetWorldConnectorNodes();

        int[] code = new int[] { 0, 4, 12, 24, 40 };

        total = (code[_mapSettings.worldSizeXZ]*_mapSettings.numMapPiecesXZ) - 1;

        layerCount = -1;

        // Build Connector Pieces
        int j = 0;
        foreach (KeyValuePair<Vector3, int> node in _connectPieceNodes )
        {
          
           _gridBuilder.BuildLocationGrid(node.Key, buildTime, 1); // 1 = connectors

            List<Vector3> connectorPieceNodes = _gridBuilder.GetGridNodePositions();

           _connectorPieceBuilder.AttachConnectorPieceToMapNode (connectorPieceNodes, node.Value, layerCount, buildTime);

            //Debug.Log("fucken count <<<<<: " + count);
            if (j % total == 0 && j != 0 && j != (_connectPieceNodes.Count - 2))
            {
                layerCount += (_mapSettings.numMapPiecesY * 2) + 2; // +2 for connectors going up floor AND roof
            }

            j += 1;

            yield return new WaitForSeconds(buildTime);

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
