using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [System.Serializable]
    public abstract class Transition : Object
    {
        public int FromIndex => fromIndex;
        public int ToIndex => toIndex;

        // ---

        [SerializeField] protected int fromIndex, toIndex;

        // ---

        public abstract bool CanTransition();
    }
}
