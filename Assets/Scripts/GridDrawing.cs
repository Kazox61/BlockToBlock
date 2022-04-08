using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawing : MonoBehaviour {
    public Vector2 size;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uv = new List<Vector2>();

    private List<int> triangles = new List<int>();
    public Color gridColor;

    public void Start() {
        CreateGrid(new Vector2(38, 8));
    }

    public void CreateGrid(Vector2 size) {
        for (int i = -Mathf.FloorToInt(size.x / 2); i <= Mathf.CeilToInt(size.x / 2); i++) {

            AddLineV(i, -Mathf.FloorToInt(size.x / 2), Mathf.CeilToInt(size.x / 2));
            AddLineH(i, -Mathf.FloorToInt(size.x / 2), Mathf.CeilToInt(size.x / 2));
        }


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;


        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = gridColor;
    }

    public void AddLineH(float positionX, float startPoint, float endPoint) {
        vertices.Add(new Vector3(positionX - 0.02f, startPoint));
        vertices.Add(new Vector3(positionX - 0.02f, endPoint));
        vertices.Add(new Vector3(positionX + 0.02f, endPoint));
        vertices.Add(new Vector3(positionX + 0.02f, startPoint));

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }

    public void AddLineV(float positionY, float startPoint, float endPoint) {
        vertices.Add(new Vector3(startPoint, positionY - 0.02f));
        vertices.Add(new Vector3(startPoint, positionY + 0.02f));
        vertices.Add(new Vector3(endPoint, positionY + 0.02f));
        vertices.Add(new Vector3(endPoint, positionY - 0.02f));

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }

}
