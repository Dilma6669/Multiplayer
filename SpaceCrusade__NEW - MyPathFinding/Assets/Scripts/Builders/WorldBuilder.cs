﻿using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    MapSettings _mapSettings;
    NodeBuilder _nodeBuilder;

    private List<Vector3Int> worldVects;
    private Dictionary<List<Vector3Int>, int> connectorVectsAndRotations;
    private List<Vector3Int> outerVects;
    private List<Vector3Int> dockingVects;

    private List<WorldNode> worldNodes;
    private Dictionary<WorldNode, List<MapNode>> worldNodeAndWrapperNodes;
    private List<MapNode> connectorNodes;
    private List<MapNode> outerNodes;
    private List<MapNode> dockingNodes;


    private int lowestYpos = 10000;
    private int highestYpos = 0;

    Dictionary<Vector3, int[]> NEW_WORLD_GRID = new Dictionary<Vector3, int[]>();

    void Awake()
    {
        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = transform.parent.GetComponentInChildren<NodeBuilder>();
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public List<WorldNode> GetWorldNodes() { return worldNodes; }
    public Dictionary<WorldNode, List<MapNode>> GetWorldAndWrapperNodes() { return worldNodeAndWrapperNodes; }
    public List<MapNode> GetConnectorNodesAndRotations() { return connectorNodes; }
    public List<MapNode> GetOuterNodes() { return outerNodes; }
    public List<MapNode> GetDockingNodes() { return dockingNodes; }



    public void BuildWorldNodes(float waitTime)
    {
        List<List<Vector3Int>> container = GetWorld_Outer_DockingVects();
        worldVects = container[0];
        outerVects = container[1];
        dockingVects = container[2];
        outerNodes = CreateOuterNodes(outerVects);

        worldNodes = CreateWorldNodes(worldVects);
        worldNodeAndWrapperNodes = CreateMapNodes(worldNodes);

        connectorVectsAndRotations = GetConnectorVects(worldNodes);
        connectorNodes = CreateConnectorNodes(connectorVectsAndRotations);

        dockingNodes = CreateDockingNodes(dockingVects);

    }

    ////////////////////////////////////////////
    // Get the Vects //////////////////////////
    ////////////////////////////////////////////


    // Get World Outer Docking Vects ////////////////////////////////////////////
    private List<List<Vector3Int>> GetWorld_Outer_DockingVects()
    {
        List<List<Vector3Int>> container = new List<List<Vector3Int>>();

        List<Vector3Int> worldVects = new List<Vector3Int>();
        List<Vector3Int> outerVects = new List<Vector3Int>();
        List<Vector3Int> dockingVects = new List<Vector3Int>();

        int countY = _mapSettings.worldPadding; // First node position
        int countZ = _mapSettings.worldPadding;
        int countX = _mapSettings.worldPadding;

        int nodeDistanceXZ = _mapSettings.worldNodeDistanceXZ + 1; // + 1 cause distance is only space IN-between nodes
        int nodeDistanceY = _mapSettings.worldNodeDistanceY + 1;

        int worldSizeY = _mapSettings.worldSizeY;
        int worldSizeZ = _mapSettings.worldSizeZ + 2; // +2 for Outer/Ship Zones
        int worldSizeX = _mapSettings.worldSizeX + 2; // +2 for Outer/Ship Zones

        //int centralOuterNodeX = 2;
        int dockingNodeX = 3;

        for (int y = 0; y < worldSizeY; y++)
        {
            for (int z = 0; z < worldSizeZ; z++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    //Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    int resultY = countY * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
                    int resultZ = countZ * _mapSettings.sizeOfMapPiecesXZ;
                    int resultX = countX * _mapSettings.sizeOfMapPiecesXZ;


                    if ((x == 0) || (z == 0) || (x == (worldSizeX - 1)) || (z == (worldSizeZ - 1)))
                    {
                        /*// Get outer Zone central node
                        if (z == (worldSizeX - 2) && x == centralOuterNodeX && y == 0)
                        {
                            outerVects.Add(new Vector3Int(resultX, resultY, resultZ));
                        } */

                        // Get docking lines
                        if (z == 0 && x == dockingNodeX)
                       {
                           dockingVects.Add(new Vector3Int(resultX, resultY, resultZ));
                       }
                   }
                   else // All central map nodes
                   {
                       worldVects.Add(new Vector3Int(resultX, resultY, resultZ));
                   }
                   countX += nodeDistanceXZ;
               }
               countX = _mapSettings.worldPadding;
               countZ += nodeDistanceXZ;
           }
           countX = _mapSettings.worldPadding;
           countZ = _mapSettings.worldPadding;
           countY += nodeDistanceY;
       }

       container.Add(worldVects);
       container.Add(outerVects);
       container.Add(dockingVects);

       return container;
   }
    ////////////////////////////////////////////////////////////////////////////

    // Get Map Vects ////////////////////////////////////////////////////////////
    private List<Vector3Int> GetMapVects(WorldNode nodeScript)
    {
        Vector3Int loc = nodeScript.nodeLocation;
        int size = nodeScript.nodeSize;

        List<Vector3Int> nodeVects = new List<Vector3Int>();

        int multiplierXZ = (int)Mathf.Floor(size / 2) * _mapSettings.sizeOfMapPiecesXZ;
        int multiplierY = (int)Mathf.Floor(size / 2) * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);

        int countY = loc.y - multiplierY;
        int countZ = loc.z - multiplierXZ;
        int countX = loc.x - multiplierXZ;

        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    nodeVects.Add(new Vector3Int(countX, countY, countZ));
                    countX += _mapSettings.sizeOfMapPiecesXZ;
                }
                countX = loc.x - multiplierXZ;
                countZ += _mapSettings.sizeOfMapPiecesXZ;
            }
            countX = loc.x - multiplierXZ;
            countZ = loc.z - multiplierXZ;
            countY += (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
        }

        return nodeVects;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get Connector Vects /////////////////////////////////////////////////////
    private Dictionary<List<Vector3Int>, int> GetConnectorVects(List<WorldNode> worldNodes)
    {
        Dictionary<List<Vector3Int>, int> connectorVectsAndRotations = new Dictionary<List<Vector3Int>, int>();

        foreach (WorldNode worldNode in worldNodes)
        {
            int nodeCount = worldNode.worldNodeCount;
            int[] neighbours = worldNode.neighbours;

            if (!worldNode.connected && worldNode.nodeSize > 0)
            {
                foreach (int neigh in neighbours)
                {
                    if (neigh > 0 && neigh < (_mapSettings.worldSizeX * _mapSettings.worldSizeZ) * _mapSettings.worldSizeY)
                    {
                        WorldNode neighbour = worldNodes[neigh];
                        if (!neighbour.connected && neighbour.nodeSize > 0)
                        {
                            //Debug.Log("Vector3 worldNode: " + worldNode.nodeLocation);
                            KeyValuePair<List<Vector3Int>, int> vectorAndRot = GetVectsAndRotation(worldNode, neighbour, neigh);

                            connectorVectsAndRotations.Add(vectorAndRot.Key, vectorAndRot.Value);
                        }
                    }
                }
                worldNode.connected = true;
            }
        }
        return connectorVectsAndRotations;
    }
    public KeyValuePair<List<Vector3Int>, int> GetVectsAndRotation(WorldNode node0, WorldNode node1, int neighCount)
    {
        List<Vector3Int> connectionVects = new List<Vector3Int>();

        bool initialSmaller = false;
        WorldNode smallerNode = null;
        WorldNode biggerNode = null;

        int widthOfConnects = 0;
        int lengthOfConnects = 0;

        if (node0.nodeSize == node1.nodeSize)
        {
            smallerNode = node1;
            biggerNode = node0;
        }
        else if (node0.nodeSize < node1.nodeSize)
        {
            initialSmaller = true;
            smallerNode = node0;
            biggerNode = node1;
        }
        else if (node0.nodeSize > node1.nodeSize)
        {
            initialSmaller = false;
            smallerNode = node1;
            biggerNode = node0;
        }
        else
        {
            Debug.LogError("Something went wrong here");
        }

        int smallLength = (int)Mathf.Floor(smallerNode.nodeSize / 2); // 0
        int bigLength = (int)Mathf.Floor(biggerNode.nodeSize / 2); // 1

        widthOfConnects = smallerNode.nodeSize;
        lengthOfConnects = _mapSettings.worldNodeDistanceXZ - (bigLength + smallLength); // 3

        int rotation = -1;

        int distance = (initialSmaller) ? smallLength : bigLength;
        distance += 1;

        for (int i = 0; i < lengthOfConnects; i++)
        {
            Vector3Int finalVect;
            Vector3Int direction; // this is to seperate what axis x,y,z neighbour is

            if (initialSmaller)
            {
                direction = (biggerNode.nodeLocation - smallerNode.nodeLocation);
            }
            else
            {
                direction = (smallerNode.nodeLocation - biggerNode.nodeLocation);
            }

            finalVect = node0.nodeLocation;

            if (direction.x != 0 && direction.y == 0 && direction.z == 0)
            {
                if (direction.x > 0)
                {
                    rotation = 1;
                    finalVect = new Vector3Int(finalVect.x + (distance * _mapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
                }
                else if (direction.x < 0)
                {
                    rotation = 3;
                    finalVect = new Vector3Int(finalVect.x - (distance * _mapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
                }
            }
            else if (direction.x == 0 && direction.y != 0 && direction.z == 0)
            {
                if (direction.y > 0)
                {
                    rotation = 4;
                    finalVect = new Vector3Int(finalVect.x, finalVect.y + (distance * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY)), finalVect.z);
                }
                else if (direction.y < 0)
                {
                    rotation = 4;
                    finalVect = new Vector3Int(finalVect.x, finalVect.y - (distance * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY)), finalVect.z);
                }
            }
            else if (direction.x == 0 && direction.y == 0 && direction.z != 0)
            {
                if (direction.z > 0)
                {
                    rotation = 0;
                    finalVect = new Vector3Int(finalVect.x, finalVect.y, finalVect.z + (distance * _mapSettings.sizeOfMapPiecesXZ));
                }
                else if (direction.z < 0)
                {
                    rotation = 2;
                    finalVect = new Vector3Int(finalVect.x, finalVect.y, finalVect.z - (distance * _mapSettings.sizeOfMapPiecesXZ));
                }
            }
            else
            {
                Debug.LogError("SOMETHING WRONG HERE direction: " + direction);
                Debug.LogFormat("initialSmaller: {0} -node0: {1} -node1: {2}", initialSmaller, node0.nodeLocation, node1.nodeLocation);
            }

            distance += 1;
            connectionVects.Add(finalVect);
        }
        return new KeyValuePair<List<Vector3Int>, int>(connectionVects, rotation);
    }
    ////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////
    // Create the Nodes from the Vects ///////
    ////////////////////////////////////////////


    // Create World Nodes ///////////////////////////////////////////////////
    private List<WorldNode> CreateWorldNodes(List<Vector3Int> nodeVects)
    {
        int[,] nodeGrid = new int[_mapSettings.worldSizeX, _mapSettings.worldSizeZ];

        // build inital map Node
        List<WorldNode> worldNodes = new List<WorldNode>();
        bool left = true;
        bool right = false;
        bool front = false;
        bool back = true;

        int rowMultipler = _mapSettings.worldSizeX;
        int colMultiplier = _mapSettings.worldSizeZ;

        int totalMultiplier = _mapSettings.worldSizeX * _mapSettings.worldSizeZ;

        int layer = 1;
        int count = 1;
        foreach (Vector3Int vect in nodeVects)
        {
            right = (count % rowMultipler == 0) ? true : false;
            left = (count == 1 || ((count - 1) % rowMultipler == 0)) ? true : false;

            front = ((count + _mapSettings.worldSizeX) > (totalMultiplier * layer) && count <= (totalMultiplier * layer)) ? true : false;
            back = (count >= ((totalMultiplier + 1) * (layer - 1)) && count <= (totalMultiplier * (layer - 1)) + _mapSettings.worldSizeX) ? true : false;

            int rotation = 0;
            WorldNode nodeScript = CreateNode<WorldNode>(vect, rotation, NodeTypes.WorldNode);
            int randSize = _mapSettings.getRandomMapSize;
            //Debug.LogError("Random.Range(0, sizes.Length): " + Random.Range(0, sizes.Length));
            nodeScript.nodeSize = randSize;
            nodeScript.worldNodeCount = (count - 1);
            nodeScript.neighbours = GetWorldNodeNeighbours((count - 1), left, right, front, back);
            worldNodes.Add(nodeScript);

            if (count % totalMultiplier == 0)
            {
                layer++;
            }
            count++;
        }
        return worldNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Map Nodes ////////////////////////////////////////////////////////
    private Dictionary<WorldNode, List<MapNode>> CreateMapNodes(List<WorldNode> worldNodes)
    {
        // Wrap map Nodes around around Initial
        Dictionary<WorldNode, List<MapNode>> worldNodeAndWrapperNodes = new Dictionary<WorldNode, List<MapNode>>();

        foreach (WorldNode worldNode in worldNodes)
        {
            List<Vector3Int> mapVects = GetMapVects(worldNode);
            List<MapNode> mapNodes = new List<MapNode>();

            foreach (Vector3Int vect in mapVects)
            {
                int rotation = 0;
                mapNodes.Add(CreateNode<MapNode>(vect, rotation, NodeTypes.MapNode));

                if (vect.y <= lowestYpos) // this is find lowest point to make LayerCounts
                {
                    lowestYpos = vect.y;
                }
                if (vect.y >= highestYpos) // this is find highest point to make LayerCounts
                {
                    highestYpos = vect.y;
                }
            }
            worldNodeAndWrapperNodes.Add(worldNode, mapNodes);
        }

        // figure out LayerCount (DONT LIKE THIS) basicly have to re-run whats just happened to get count
        foreach (WorldNode worldNode in worldNodeAndWrapperNodes.Keys)
        {
            List<MapNode> wrapperNodes = worldNodeAndWrapperNodes[worldNode];
            foreach (MapNode nodeScript in wrapperNodes)
            {
                nodeScript.nodeLayerCount = GetLayerCountForNodePos(nodeScript.nodeLocation);
            }
        }
        return worldNodeAndWrapperNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Connector Nodes ////////////////////////////////////////////////
    private List<MapNode> CreateConnectorNodes(Dictionary<List<Vector3Int>, int> connectorVects)
   {
       List<MapNode> connectorNodes = new List<MapNode>();

        foreach (List<Vector3Int> vectors in connectorVects.Keys)
        {
            int rotation = connectorVects[vectors];

            foreach (Vector3Int vect in vectors)
            {
                connectorNodes.Add(CreateNode<MapNode>(vect, rotation, NodeTypes.ConnectorNode));
            }
        }
       return connectorNodes;
   }
    ////////////////////////////////////////////////////////////////////////////

    // Create Outer Nodes /////////////////////////////////////////////////////
    private List<MapNode> CreateOuterNodes(List<Vector3Int> outerVects)
    {
        List<MapNode> outerNodes = new List<MapNode>();
        foreach (Vector3Int vect in outerVects)
        {
            int rotation = 0;
            outerNodes.Add(CreateNode<MapNode>(vect, rotation, NodeTypes.OuterNode));
        }
        return outerNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Docking Nodes /////////////////////////////////////////////////////
    private List<MapNode> CreateDockingNodes(List<Vector3Int> dockingVects)
    {
        List<MapNode> dockingNodes = new List<MapNode>();
        foreach (Vector3Int vect in dockingVects)
        {
            int rotation = 0;
            dockingNodes.Add(CreateNode<MapNode>(vect, rotation, NodeTypes.DockingNode));
        }
        return dockingNodes;
    }
    ////////////////////////////////////////////////////////////////////////////



    ////////////////////////////////////////////
    // Helper Functions ////////////////////////
    ////////////////////////////////////////////


    // Create Generic Node /////////////////////////////////////////////////////
    private T CreateNode<T>(Vector3Int vect, int rotation, NodeTypes nodeType) where T : BaseNode
    {
        //Debug.Log("Vector3 (gridLoc): x: " + vect.x + " y: " + vect.y + " z: " + vect.z);
        GameObject node = _nodeBuilder.InstantiateNodeObject(vect, nodeType, this.transform);
        T nodeScript = node.GetComponent<T>();
        nodeScript.nodeLocation = vect;
        nodeScript.nodeRotation = rotation;
        nodeScript.nodeLayerCount = GetLayerCountForNodePos(vect);
        return nodeScript;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get Layer Count for Node Position ////////////////////////////////////////
    private int GetLayerCountForNodePos(Vector3Int nodePos)
    {
        int yPos = nodePos.y;
        int currHeight = lowestYpos;
        int mapHeight = (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
        for (int layer = 0; layer < 50; layer += 2) // needs to be 2 coz 2 layers needed each map piece floor and roof
        {
            if (yPos == currHeight)
            {
                return layer;
            }
            currHeight += mapHeight;
        }
        return 0;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get World Node Neighbours ///////////////////////////////////////////////
    private int[] GetWorldNodeNeighbours(int count, bool left, bool right, bool front, bool back)
    {
        int[] neighbours = new int[6];

        neighbours[0] = (left) ? -1 : count - 1;                             //(x - 1, y, z)
        neighbours[1] = (right) ? -1 : count + 1;                            //(x + 1, y, z)
        neighbours[2] = (front) ? -1 : count + _mapSettings.worldSizeX;      //(x, y, z + 1)
        neighbours[3] = (back) ? -1 : count - _mapSettings.worldSizeX;       //(x, y, z - 1)
        neighbours[4] = count - (_mapSettings.worldSizeX * _mapSettings.worldSizeZ);  //(x, y - 1, z)
        neighbours[5] = count + (_mapSettings.worldSizeX * _mapSettings.worldSizeZ);  //(x, y + 1, z)

        return neighbours;
    }
    ////////////////////////////////////////////////////////////////////////////


    /*
    private bool AddValueToWorldGridLoc(Vector3 loc, int[] values)
    {
        if (!NEW_WORLD_GRID.ContainsKey(loc))
        {
            NEW_WORLD_GRID[loc] = values;
            return true;
        }
        else
        {
            Debug.LogFormat("ISSUE HERE GRID LOCATION ALREADY ASSIGNED!!! Vector: {0}", loc);
            return false;
        }
    }
    */

}
