using UnityEngine;

public class NodeCover : MonoBehaviour {

    MapNode parentNode;
    GameObject selector;

    void Awake()
    {
        parentNode = transform.parent.GetComponent<MapNode>();
        selector = transform.parent.transform.Find("Select").gameObject;
    }

    void OnMouseDown()
    {
        parentNode.ActivateMapPiece();
    }

    void OnMouseOver()
    {
        selector.SetActive(true);
    }

    void OnMouseExit()
    {
        selector.SetActive(false);
    }
}
