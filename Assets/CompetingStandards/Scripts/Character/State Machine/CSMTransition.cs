using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [System.Serializable]
    public abstract class Transition
    {
        [HideInInspector] public int FromIndex;
        [HideInInspector] public int ToIndex;

        // ---

        public abstract bool CanTransition();
    }
}
