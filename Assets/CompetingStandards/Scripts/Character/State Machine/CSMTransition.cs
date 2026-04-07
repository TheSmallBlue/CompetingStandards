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

        protected bool initialized { get; private set; }

        // ---

        public bool CanTransition(StateMachine.UpdateType updateType)
        {
            if (!initialized) throw new System.Exception("State not initialized!");

            switch (updateType)
            {
                case StateMachine.UpdateType.FixedUpdate:
                    return CanTransitionFixedUpdate();
                
                default:
                    return CanTransitionUpdate();
            }
        }

        protected virtual bool CanTransitionUpdate() => false;
        protected virtual bool CanTransitionFixedUpdate() => false;

        // ---

        public virtual void Initialize(CSM.StateMachine source)
        {
            SourceMachine = source;

            initialized = true;
        }

        // ---

        public virtual void OnTransitionedInto() {}
    }
}
