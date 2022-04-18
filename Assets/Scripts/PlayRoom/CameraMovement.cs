using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour {
    public Tilemap gridMap;
    public Camera cam;
    [HideInInspector] public bool takeScreenshotOnNextFrame = false;
    [HideInInspector] public string path;


    public void CalculateCameraZoom() {
        gridMap.CompressBounds();
        var bounds = gridMap.localBounds;

        bounds.Expand(bounds.size.y/2);

        var vertical = bounds.size.y;

        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        horizontal *= 1.5f;
        
        var size = Mathf.Max(horizontal, vertical) * 0.5f;

        var center = bounds.center + new Vector3(0, 0, -10);

        center = center + new Vector3(horizontal * 0.33f, 0, 0);

        cam.transform.position = center;
        cam.orthographicSize = size;
    }

    public void OnPostRender() {
        if (!takeScreenshotOnNextFrame) {
            return;
        }
        takeScreenshotOnNextFrame = false;
        var bounds = gridMap.localBounds;
        var center = cam.WorldToScreenPoint(bounds.center);

        Texture2D renderResult = new Texture2D(500, 500, TextureFormat.ARGB32, false);
        Rect rect = new Rect(center.x -250, center.y - 250, 500, 500);
        renderResult.ReadPixels(rect, 0, 0);

        byte[] byteArray = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, byteArray);
    }
}
