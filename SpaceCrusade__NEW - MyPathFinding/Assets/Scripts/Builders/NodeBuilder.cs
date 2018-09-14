using UnityEngine;

public class NodeBuilder : MonoBehaviour {

    public GameObject _gridObjectPrefab; // Debugging purposes
    public GameObject _worldNodePrefab; // object that shows Map nodes
    public GameObject _mapNodePrefab; // object that shows Map nodes
    public GameObject _connectorNodePrefab; // object that shows Map nodes
    public GameObject _outerNodePrefab; // objects that define the out of bounds areas
    public GameObject _dockingNodePrefab; // node that players ship connect to when docking

    //////////////////////////////////////////

    // node objects are spawned at bottom corner each map piece
    public GameObject InstantiateNodeObject(Vector3Int loc, NodeTypes nodePrefab, Transform parent)
    {
        //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
        GameObject nodeObject = Instantiate(GetNodePrefab(nodePrefab), parent, false);
        nodeObject.transform.position = loc;
        nodeObject.transform.SetParent(parent);
        nodeObject.transform.localScale = new Vector3Int(1, 1, 1);

        return nodeObject;
    }


    private GameObject GetNodePrefab(NodeTypes node)
    {
        switch (node)
        {
            case NodeTypes.GridNode:
                return _gridObjectPrefab;
            case NodeTypes.WorldNode:
                return _worldNodePrefab;
            case NodeTypes.MapNode:
                return _mapNodePrefab;
            case NodeTypes.ConnectorNode:
                return _connectorNodePrefab;
            case NodeTypes.OuterNode:
                return _outerNodePrefab;
            case NodeTypes.DockingNode:
                return _dockingNodePrefab;
            default:
                Debug.Log("OPPSALA WE HAVE AN ISSUE HERE");
                return null;
        }
    }

}
