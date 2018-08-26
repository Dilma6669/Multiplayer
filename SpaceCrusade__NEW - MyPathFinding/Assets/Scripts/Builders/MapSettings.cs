using UnityEngine;

public class MapSettings : MonoBehaviour {

    public int worldSizeXZ; // 2
    public int worldSizeY; // 1

    public int worldType; // 0 = square, 1 = Line, 2 = tower

	public int numMapPiecesXZ; // 5
	public int numMapPiecesY; // 3
	public int sizeOfMapPiecesXZ; // 24
	public int sizeOfMapPiecesY; // 6
    public int sizeOfMapVentsY; // 2
    public int sizeOfMapConnectorsXYZ; // 1

    public int sizeOfCubes; // 1

    [HideInInspector]
	public int totalXZCubes; // 5 * 24
    [HideInInspector]
	public int totalYCubes; // 3 * 6




	void Awake() {
		totalXZCubes = numMapPiecesXZ * sizeOfMapPiecesXZ;
		totalYCubes = numMapPiecesY * sizeOfMapPiecesY;
	}


}
