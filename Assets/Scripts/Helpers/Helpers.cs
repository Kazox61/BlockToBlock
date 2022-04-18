using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers {

    private static PointerEventData eventDataCurrentPosition;
    private static List<RaycastResult> results;
    public static bool IsOverUI() {
        eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
