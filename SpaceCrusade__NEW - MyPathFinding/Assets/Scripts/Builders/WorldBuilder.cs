using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{

    MapSettings _mapSettings;

    public GameObject _spawnPrefab; // object that runs around world space creating the locations

    public GameObject _worldMapNodePrefab; // object that shows Map nodes
    public GameObject _worldConnectorNodePrefab; // object that shows Map nodes

    private GameObject _spawn;

    private List<Vector3> _worldMapNodes = new List<Vector3>();
    private Dictionary<Vector3, int> _worldConnectorNodes = new Dictionary< Vector3, int>();

    int worldSizeXZ;
    int worldSizeY;
    int numMapPiecesXZ;
    int numMapPiecesY;
    int sizeOfMapConnectorsXYZ;
    int sizeOfMapPiecesXZ;
    int sizeOfMapPiecesY;
    int sizeOfCubes;

    int connectorCount = 0;
    List<int> _corners = new List<int>();

    void Awake()
    {
        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public List<Vector3> GetWorldMapNodes()
    {
        return _worldMapNodes;
    }

    public Dictionary<Vector3, int> GetWorldConnectorNodes()
    {
        return _worldConnectorNodes;
    }

    public void BuildWorldGrid(float waitTime)
    {
        worldSizeXZ = _mapSettings.worldSizeXZ;
        worldSizeY = _mapSettings.worldSizeY;
        numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        numMapPiecesY = _mapSettings.numMapPiecesY;
        sizeOfMapConnectorsXYZ = _mapSettings.sizeOfMapConnectorsXYZ;
        sizeOfMapPiecesXZ = _mapSettings.sizeOfMapPiecesXZ;
        sizeOfMapPiecesY = _mapSettings.sizeOfMapPiecesY;
        sizeOfCubes = _mapSettings.sizeOfCubes;

        GetCornerConnectors();

        // SPAWN //
        _spawn = Instantiate(_spawnPrefab, transform, false);


        // these are the bottom left corner axis for EACH map node
        int startGridLocX = 0;
        int startGridLocY = 0; // starting height of each new layer, 0, 10, 20. etc
        int startGridLocZ = 0;

        // Load each Y layer of grids in a loop, not nessacary but just did it this way for some reason
        for (int worldLayer = 0; worldLayer < _mapSettings.worldSizeY; worldLayer++)
        {
            // build map Layer grid locations
            BuildWorldLocations(startGridLocX, startGridLocY, startGridLocZ, 0.001f);

            startGridLocX = 0;
            startGridLocY += _mapSettings.numMapPiecesY * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY) + 10;
            startGridLocZ = 0;

            //yield return new WaitForSeconds(waitTime);
        }

        //Debug.Log("connectorCount: " + connectorCount);
    }


    private void BuildWorldLocations(int startX, int startY, int startZ, float waitTime)
    {
        int gridLocX = startX;
        int gridLocY = startY;
        int gridLocZ = startZ;

        int mapPadding = 2; // 2 = around whole map not just bottom and left side

        int finishX = worldSizeXZ * (numMapPiecesXZ + (sizeOfMapConnectorsXYZ * mapPadding)) - (worldSizeXZ - 1);
        int finishZ = worldSizeXZ * (numMapPiecesXZ + (sizeOfMapConnectorsXYZ * mapPadding)) - (worldSizeXZ - 1);
        //int finishY = worldSizeY * (numMapPiecesY);

        float spawnPosX = startX;
        float spawnPosZ = startZ;
        float spawnPosY = startY;

        int nodeCount = 0;

        for (int z = startZ; z < finishZ; z++)
        {

            spawnPosX = startX;
            _spawn.transform.localPosition = new Vector3(spawnPosX, spawnPosY, spawnPosZ);

            gridLocX = startX;

            for (int x = startX; x < finishX; x++)
            {

                // Create location positions
                ///////////////////////////////
                // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                Vector3 gridLoc = new Vector3(gridLocX, gridLocY, gridLocZ);

                // node objects are spawned at bottom corner each map piece
                MakeWorldNodeObject(gridLocX, gridLocY, gridLocZ, startX, startY, startZ, finishX, finishZ, nodeCount);

                spawnPosX += sizeOfMapPiecesXZ;
                _spawn.transform.localPosition = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
                gridLocX += sizeOfMapPiecesXZ;
                nodeCount += 1;

            }
            spawnPosZ += sizeOfMapPiecesXZ;
            gridLocZ += sizeOfMapPiecesXZ;
        }
            // NEED TO SORT OUT THE CONNETIONS ONTOP OF THE WORLD LAYER HERE
            // int height = (_mapSettings.numMapPiecesY * _mapSettings.sizeOfMapPiecesY) + (_mapSettings.numMapPiecesY * _mapSettings.sizeOfMapVentsY);
            // gridLocY += height;
            // spawnPosY += height;
            //MakeWorldNodeObject(gridLocX, gridLocY, gridLocZ, startY, finishX, finishZ);

    }


    // node objects are spawned at bottom corner each map piece
    private void MakeWorldNodeObject(int gridLocX, int gridLocY, int gridLocZ, int startX, int startY, int startZ, int finishX, int finishZ, int nodeCount)
    {
        //////////////////////////////////////////

        int mapNodeOffset = 2; // change this to move Map node
        int mapNodePos = worldSizeXZ * (numMapPiecesXZ + sizeOfMapConnectorsXYZ) + mapNodeOffset; //

        int multiplier = ((numMapPiecesXZ + sizeOfMapConnectorsXYZ) * sizeOfMapPiecesXZ);

        // Map Nodes
        if (nodeCount == mapNodePos) // finds first node and then places all other nodes from start one here
        {
            int x = gridLocX;
            int y = gridLocY;
            int z = gridLocZ;

            for (int i = 0; i < worldSizeXZ; i++)
            {
                for (int j = 0; j < worldSizeXZ; j++)
                {

                    // Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    Vector3 pos = new Vector3(x, y, z);

                    GameObject nodeObject = Instantiate(_worldMapNodePrefab, _spawn.transform, false);
                    nodeObject.transform.position = pos;
                    nodeObject.transform.SetParent(this.gameObject.transform);
                    nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                    _worldMapNodes.Add(pos);

                    x += multiplier;
                }
                x = gridLocX;
                z += multiplier;
            }

        }

        // connector Nodes
        else if (gridLocX % multiplier == 0)
        {
            Vector3 pos = new Vector3(gridLocX, gridLocY, gridLocZ);

            if (!_worldConnectorNodes.ContainsKey(pos))
            {
                if (RemoveCornerConnectors())
                {
                    GameObject nodeObject = Instantiate(_worldConnectorNodePrefab, _spawn.transform, false);
                    nodeObject.transform.position = pos;
                    nodeObject.transform.SetParent(this.gameObject.transform);
                    nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                    _worldConnectorNodes.Add(pos, 1);
                }
            }
        }
        else if (gridLocZ % multiplier == 0)
        {
            Vector3 pos = new Vector3(gridLocX, gridLocY, gridLocZ);

            if (!_worldConnectorNodes.ContainsKey(pos))
            {
                if (RemoveCornerConnectors())
                {
                    GameObject nodeObject = Instantiate(_worldConnectorNodePrefab, _spawn.transform, false);
                    nodeObject.transform.position = pos;
                    nodeObject.transform.SetParent(this.gameObject.transform);
                    nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                    _worldConnectorNodes.Add(pos, 0);
                }
            }
        }
        /////////////////////////////////////////////
    }

    private void GetCornerConnectors()
    {
        _corners = new List<int>();

        int jumpRow = numMapPiecesXZ * worldSizeXZ;
        int jumpCol = numMapPiecesXZ + 1;

        int cornerCount = 0;

        for (int y = 0; y < (worldSizeY); y++)
        {
            for (int z = 0; z < (worldSizeXZ + 1); z++)
            {
                for (int x = 0; x < (worldSizeXZ + 1); x++)
                {
                    _corners.Add(cornerCount);
                    cornerCount += jumpCol;
                }
                cornerCount += jumpRow;
            }
            cornerCount -= jumpCol;
            cornerCount -= jumpRow;
            cornerCount++;
        }
    }


    private bool RemoveCornerConnectors()
    {
        foreach(int i in _corners)
        {
            if(connectorCount == i)
            {
                //Debug.Log("not letting i: " + i);
                connectorCount++;
                return false;
            }
        }

        connectorCount++;
        return true;
    }
}
