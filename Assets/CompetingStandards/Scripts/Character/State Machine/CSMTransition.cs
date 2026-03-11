using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [System.Serializable]
    public abstract class Transition
    {
        public CSM.StateMachine SourceMachine { get; private set; }
        public Character SourceCharacter => SourceMachine.SourceCharacter;

        // ---

        public int ToIndex;

        // ---

        protected bool initialized { get; private set; }

        // ---

        public abstract bool CanTransition();

        // ---

        public void Initialize(CSM.StateMachine source)
        {
            SourceMachine = source;

            initialized = true;
        }
    }
}
