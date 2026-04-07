using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM.Transitions
{
    [System.Serializable]
    public class CSMTimerTransition : Transition
    {
        [SerializeField] StateMachine.UpdateType updateType;
        [SerializeField] int frameCount;

        // ---
    }
}
