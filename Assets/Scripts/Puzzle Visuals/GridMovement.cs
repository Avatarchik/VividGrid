using UnityEngine;
using System.Collections;

public class GridMovement : MonoBehaviour {

    public Texture2D heightMap;
    public float heightmapStrength = 5;

    public int vertsX = 11;
    public int vertsY = 11;
    private int curVertsX = 0;
    private int curVertsY = 0;

    public float speed = 1;
    private float xOffset = 0;
    private float yOffset = 0;

    private Vector3[] verticesStarPos;
    private Mesh mesh;

	void Start(){
		mesh = GetComponent<MeshFilter>().mesh;
 		verticesStarPos = mesh.vertices;
	}

	 void Update() {
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        if (vertices.Length != vertsX* vertsY){
        	print("Incorrect Vert Numbers");
        }
        while (i < vertices.Length) {
        	if (curVertsX > vertsX){
        		curVertsY +=1;
        		curVertsX = 1;
        	}

            float height = heightMap.GetPixel(curVertsX + (int)xOffset,curVertsY + (int)yOffset).grayscale;
            float zPos = verticesStarPos[i].z + ((height * heightmapStrength)-(0.5f * heightmapStrength));
            float yPos = verticesStarPos[i].y + ((height * heightmapStrength)-(0.5f * heightmapStrength));
            float xPos = verticesStarPos[i].x;

            // float randAngle = Random.Range(0.0f, 360.0f);
            // float randDist = Random.Range(0.0f,1.0f);

            // float xDelta = Mathf.Cos(randAngle) * randDist;
            // float zDelta = Mathf.Sin(randAngle) * randDist;

            // float xPos = verticesStarPos[i].x + xDelta;
            // float yPos = verticesStarPos[i].y;
            // float zPos = verticesStarPos[i].z + zDelta;

            vertices[i] = Vector3.Lerp(vertices[i], new Vector3(xPos,yPos,zPos),speed * Time.deltaTime);
            curVertsX++;
            i++;
        }
        
        xOffset += (speed * Time.deltaTime);
        curVertsX = 0;
        curVertsY = 0;
        
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}