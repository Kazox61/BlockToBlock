using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeAnchorBaseSO<T> : ScriptableObject where T : class {

    public bool IsSet { get; private set; }

    private T item;

    public T Item {
        get => item;
        set {
            item = value;
            IsSet = item != null;
        }
    }

    private void OnDisable() {
        item = null;
        IsSet = false;
    }
}

