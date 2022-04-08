using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFinishedTrigger : MonoBehaviour {
    public GameController gameController;


    public void AnimationFinished() {
        gameController.StateMachine.CurrentState.isAnimationFinished = true;
    }
}
