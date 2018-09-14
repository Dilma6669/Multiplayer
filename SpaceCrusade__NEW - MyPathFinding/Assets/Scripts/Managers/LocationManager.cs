﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LocationManager : MonoBehaviour {

    GameManager _gameManager;

    WorldBuilder _worldBuilder;
	GridBuilder _gridBuilder;
	MapPieceBuilder _mapPieceBuilder;
    OuterZoneBuilder _outerZoneBuilder;
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

        _outerZoneBuilder = GetComponentInChildren<OuterZoneBuilder>();
        if (_outerZoneBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

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

        // Get the World Nodes
        _worldBuilder.BuildWorldNodes(buildTime);

        // Get the Map Nodes around the World Nodes
        Dictionary<WorldNode, List<MapNode>> worldAndMapNodes = _worldBuilder.GetWorldAndWrapperNodes();


        // World Nodes and Maps
        foreach (WorldNode worldNode in worldAndMapNodes.Keys)
        {
            List<MapNode> wrapperNodes = worldAndMapNodes[worldNode];

            int mapSize = worldNode.nodeSize;

            foreach (MapNode mapNode in wrapperNodes)
            {
                Vector3 nodeVect = mapNode.nodeLocation;

                int layerCount = mapNode.nodeLayerCount;
                /*
                _gridBuilder.BuildLocationGrid(nodeVect, mapSize);

                List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();

                _mapPieceBuilder.AttachMapPieceToMapNode(mapPieceNodes, layerCount, mapSize, 0); // 0 = mapPieces 1 = Roofs
                */
               // yield return new WaitForSeconds(buildTime);
            }
        }
        
        // Connectors
        List<MapNode> connectorNodes = _worldBuilder.GetConnectorNodesAndRotations();

        foreach (MapNode connectorNode in connectorNodes)
        {
            Vector3 nodeVect = connectorNode.nodeLocation;

            int layerCount = connectorNode.nodeLayerCount;

            int mapSize = 1; // this is for Each connector

            int rotation = connectorNode.nodeRotation;

            if (rotation == 4) // for the connectors going up
            {
                connectorNode.gameObject.transform.Find("ConnectorNodeCover").transform.localScale = new Vector3Int(8, 8, 8); 
            }

            connectorNode.gameObject.transform.localEulerAngles = new Vector3Int(0, rotation * 90, 0);

            /*
            _gridBuilder.BuildLocationGrid(nodeVect, mapSize);

            List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();

            _mapPieceBuilder.AttachMapPieceToMapNode(mapPieceNodes, layerCount, mapSize, 2, rotation); // 2= connectors, 3 = roofs
            */
           // yield return new WaitForSeconds(buildTime);
        }



        /*
        // Outer Zones
        List<WorldNode> outerNodes = _worldBuilder.GetOuterNodes();
        foreach (WorldNode outerNode in outerNodes)
        {
            Vector3 nodeVect = outerNode.nodeLocation;
            _outerZoneBuilder.CreateOuterZoneForNode(outerNode);
            //yield return new WaitForSeconds(buildTime);
        }

        // Docking Zones
        List<MapNode> dockingNodes = _worldBuilder.GetDockingNodes();

        foreach (MapNode dockingNode in dockingNodes)
        {
            Vector3 nodeVect = dockingNode.nodeLocation;
            int layerCount = dockingNode.nodeLayerCount;
            int mapSize = 1; // this is for Each docking tile
            _gridBuilder.BuildLocationGrid(nodeVect, mapSize);
            List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();
            _mapPieceBuilder.AttachMapPieceToMapNode(mapPieceNodes, layerCount, mapSize, 2); // 2= connectors, 3 = roofs
            yield return new WaitForSeconds(buildTime);
        }
        */

        yield return new WaitForSeconds(buildTime);

        Debug.Log("FINSIHED LOADING!!!!!!!!");
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
