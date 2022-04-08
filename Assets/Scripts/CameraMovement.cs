using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour {
    public Tilemap gridMap;
    public Camera cam;

    public void Cal(bool mobile) {
        gridMap.CompressBounds();
        var bounds = gridMap.localBounds;

        bounds.Expand(2);

        var vertical = bounds.size.y;

        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        if (mobile) {
            horizontal *= 1.5f;
        }

        var size = Mathf.Max(horizontal, vertical) * 0.5f;

        var center = bounds.center + new Vector3(0, 0, -10);

        if (mobile) {
            center = center + new Vector3(horizontal * 0.33f, 0, 0);
        }

        cam.transform.position = center;
        cam.orthographicSize = size;
    }

}
