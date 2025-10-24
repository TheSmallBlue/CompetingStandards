using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [System.Serializable]
    public abstract class State
    {
        public CSM.StateMachine SourceMachine { get; private set; }
        public Character SoureCharacter => SourceMachine.SourceCharacter;

        // ---

        protected bool initialized { get; private set; }

        // ---

        protected virtual void OnStateEnter() { }
        protected virtual void OnStateExit() { }

        protected virtual void OnStateUpdate() { }
        protected virtual void OnStateFixedUpdate() { }

        // ---

        public void Initialize(CSM.StateMachine source)
        {
            SourceMachine = source;

            initialized = true;
        }

        // ---

        public void EnterState()
        {
            if (!initialized) throw new System.Exception("State not initialized!");

            OnStateEnter();
        }

        public void UpdateState(StateMachine.UpdateType updateType)
        {
            if (!initialized) throw new System.Exception("State not initialized!");

            switch (updateType)
            {
                case StateMachine.UpdateType.Update:
                    OnStateUpdate();
                    break;

                case StateMachine.UpdateType.FixedUpdate:
                    OnStateFixedUpdate();
                    break;
            }
        }

        public void ExitState()
        {
            if (!initialized) throw new System.Exception("State not initialized!");

            OnStateExit();
        }

    }
}
