using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RuntimeAnchorBaseSO<T> : ScriptableObject where T : class {

    public UnityAction<T> OnEventRaised;
    public bool IsSet { get; private set; }

    private T item;

    public T Item {
        get => item;
        set {
            item = value;
            RaiseEvent(item);
            IsSet = item != null;
        }
    }

    private void OnDisable() {
        item = null;
        IsSet = false;
    }

    public void RaiseEvent(T data) {
        if (OnEventRaised != null) OnEventRaised.Invoke(data);
    }
}

