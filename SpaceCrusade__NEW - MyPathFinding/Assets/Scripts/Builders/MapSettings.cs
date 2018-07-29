using UnityEngine;

public class MapSettings : MonoBehaviour {

	public int numMapPiecesXZ;
	public int numMapPiecesY;
	public int sizeOfMapPieces;
	public int heightOfMapPieces;
	public int totalXZCubes;
	public int totalYCubes;
	public int sizeOfCubes;

	void Awake() {
		totalXZCubes = numMapPiecesXZ * sizeOfMapPieces;
		totalYCubes = numMapPiecesY * heightOfMapPieces;
	}


}
