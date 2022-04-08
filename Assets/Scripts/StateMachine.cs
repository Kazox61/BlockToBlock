using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine { 
    public StateBase CurrentState { get; private set; }

    public void TryEnterState(StateBase state) {
        CurrentState?.OnExit();
        CurrentState = state;
        CurrentState?.OnEnter();
    }

}
