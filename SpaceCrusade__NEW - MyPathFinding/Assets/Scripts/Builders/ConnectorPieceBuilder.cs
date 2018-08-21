﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorPieceBuilder : MonoBehaviour {

    LocationManager _locationManager;
    CubeBuilder _cubeBuilder;
    MapSettings _mapSettings;


    public List<int[,]> floors = new List<int[,]>();
    public List<int[,]> vents = new List<int[,]>();

    private bool loadVents = false;

    private int[] _playerShipPlacements; // int array which the index is the NodeCount and the contents is that nodes rotation

    void Awake()
    {
        _locationManager = transform.parent.GetComponent<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    // Return the node number that can used in the shipPlacement node list to attach ship to
    public int GetConnectorNodeForShipPlacement(int playerID)
    {
        int numMapPiecesXZ = _mapSettings.numMapPiecesXZ;

        List<int> entranceList = new List<int>();

        int bottomCount = (int)Mathf.Floor(numMapPiecesXZ / 3);
        for(int i = 0; i < bottomCount; i++)
        {
            entranceList.Add(0);
        }
        int topCount = (int)Mathf.Floor(numMapPiecesXZ / 3);
        for (int i = 0; i < topCount; i++)
        {
            entranceList.Add(1);
        }
        int leftCount = (int)Mathf.Floor(numMapPiecesXZ / 3);
        for (int i = 0; i < leftCount; i++)
        {
            entranceList.Add(2);
        }
        int rightCount = (int)Mathf.Floor(numMapPiecesXZ / 3);
        for (int i = 0; i < rightCount; i++)
        {
            entranceList.Add(3);
        }

        int result = bottomCount + topCount + leftCount + rightCount;

        /*
        List<int> matchs = new List<int>();

        for(int i = 0; i < _playerShipPlacements.Length; i++)
        {
            if (_playerShipPlacements[i] == playerID)
            {
                matchs.Add(i);
            }
        }

        int winner = matchs[Random.Range(0, matchs.Count)];

        switch (playerID)
        {
            case 0:
                return = winner + 1;
                break;
            case 1:
                nodeCountToReturn = winner + 1;
                break;

            case 2:

                break;

            case 3:

                break;

            default

            break;
        }
        */
        return 0; 
    }


    public void AttachConnectorPieceToMapNode(List<Vector3> nodes)
    {
        StartCoroutine(BuildConnectorsByIEnum(nodes, 0.001f));
    }


    private IEnumerator BuildConnectorsByIEnum(List<Vector3> nodes, float waitTime)
    {

        int numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        int sizeOfMapPieces = _mapSettings.sizeOfMapPieces;

        _playerShipPlacements = new int[nodes.Count];

        Vector3 GridLoc;

        List<int[,]> layers = new List<int[,]>();
        int[,] floor;

        /*
        int entranceSpaceBetween = 7;
        int bottomCount = (int)Mathf.Floor(_mapSettings.numMapPiecesXZ / entranceSpaceBetween);
        int topCount = (int)Mathf.Floor(_mapSettings.numMapPiecesXZ / entranceSpaceBetween);
        int leftCount = (int)Mathf.Floor(_mapSettings.numMapPiecesXZ / entranceSpaceBetween);
        int rightCount = (int)Mathf.Floor(_mapSettings.numMapPiecesXZ / entranceSpaceBetween);
        */

        int sizeSquared = (numMapPiecesXZ * 4) + 4; // calculating area count for area AROUND mainShip
        int nodeCount = 0;
        int layerCount = -1;


        for (int j = 0; j < nodes.Count; j++)
        {
            int posX = (int)nodes[j].x;
            int posY = (int)nodes[j].y;
            int posZ = (int)nodes[j].z;

            int modulusResult = nodeCount % sizeSquared;

            if (modulusResult == 0)
            { // clever way to figure out each increase in Layer
                layerCount += 1;
            }

           // if (j % _mapSettings.numMapPiecesXZ == 4) // trying to put entrances in middle of chunks of 7
           // {



            // Working Out Corners with padding
            bool corner = false;
            if (modulusResult == 0 || modulusResult == 1 || 
                modulusResult == (numMapPiecesXZ + 2) || // Bottom left corner

                modulusResult == (numMapPiecesXZ + 1) || modulusResult == (numMapPiecesXZ + 1) - 1 ||
                modulusResult == (numMapPiecesXZ + 3) || // Bottom Right Corner 

                modulusResult == (sizeSquared - 1) - (numMapPiecesXZ + 1) || modulusResult == (sizeSquared - 1) - (numMapPiecesXZ + 1) + 1 ||
                modulusResult == (sizeSquared - 1) - (numMapPiecesXZ + 1) - 2 || // Top Left Corner

                modulusResult == sizeSquared - 1 || modulusResult == sizeSquared - 2 || 
                modulusResult == (sizeSquared - 1) - (numMapPiecesXZ + 1) - 1) // Top Right Corner
            {
                corner = true;
            }

                if (!corner)
                {
                    int mapPieceType = (layerCount % 2 == 0) ? 0 : 1;
                    int mapPiece = 0; //Map pieces
                    int rotation = 1;

                    // Rotation calculation (NOTE: THESE COULD ALL BE WRONG!!!)
                    if (modulusResult >= 0 && modulusResult <= (numMapPiecesXZ + 1)) // Bottom
                    {
                        rotation = 0;
                    }
                    else if (modulusResult >= (sizeSquared - 1) - (numMapPiecesXZ + 1) && modulusResult <= sizeSquared - 1) // Top
                    {
                        rotation = 2;
                    }
                    else if (nodeCount % numMapPiecesXZ == 0) // Left
                    {
                        rotation = 1;
                    }
                    else // right
                    {
                        rotation = 3;
                    }

                    // To store placement to give to PlayerShip (array index(Node Index): rotation)
                    _playerShipPlacements[j] = rotation;


                    layers = GetConnectorPiece(mapPieceType, mapPiece);
                    int rotations = rotation;

                    int objectsCountX = posX;
                    int objectsCountY = posY;
                    int objectsCountZ = posZ;

                    for (int y = 0; y < layers.Count; y++)
                    {

                        objectsCountX = posX;
                        objectsCountZ = posZ;

                        floor = layers[y];

                        for (int r = 0; r < rotations; r++)
                        {
                            floor = TransposeArray(floor, sizeOfMapPieces - 1);
                        }

                        for (int z = 0; z < floor.GetLength(0); z++)
                        {

                            objectsCountX = posX;

                            for (int x = 0; x < floor.GetLength(1); x++)
                            {

                                int cubeType = floor[z, x];
                                GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);

                                _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount); // Create the cube

                                objectsCountX += 1;
                            }
                            objectsCountZ += 1;
                        }
                        objectsCountY += 1;
                    }
                }
          //  }
            nodeCount += 1;

            yield return new WaitForSeconds(waitTime);
        }

        //_gameManager.MapsFinishedLoading ();
    }

    // Get map by type and piece
    private List<int[,]> GetConnectorPiece(int type, int map)
    {
        switch (type)
        {
            case 0: // Floor
                BaseConnectorPiece connectPiece = null;
                switch (map)
                {
                    case 0:
                        connectPiece = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    case 1:
                        connectPiece = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    default:
                        break;
                }
                return connectPiece.floors;

            case 1:
                BaseConnectorPiece ventPiece = null;
                switch (map)
                {
                    case 0:
                        ventPiece = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    case 1:
                        ventPiece = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    default:
                        break;
                }
                return ventPiece.floors;

            default:
                Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                return null;
        }
    }





    private int[,] TransposeArray(int[,] array, int size)
    {

        int[,] ret = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                ret[i, j] = array[size - j - 1, i];
            }
        }

        return ret;
    }
}
