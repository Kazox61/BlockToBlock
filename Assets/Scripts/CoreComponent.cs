using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoreComponent : MonoBehaviour {

    public List<CoreComponent> components = new List<CoreComponent>();


    public T GetCoreComponent<T>() where T: CoreComponent {
        var component = components.OfType<T>().FirstOrDefault();

        return component;
    }

    public void AddComponent(CoreComponent component) {
        if (!components.Contains(component)) {
            components.Add(component);
        }
    }
}
