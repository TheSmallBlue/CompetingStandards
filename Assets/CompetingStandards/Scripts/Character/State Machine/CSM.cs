using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [CreateAssetMenu(fileName = "State Machine", menuName = "CompetingStandards/Character State Machine", order = 1)]
    public class StateMachine : ScriptableObject
    {
        [SerializeReference] CSM.State[] states;
        [SerializeReference] CSM.Transition[] transitions;

        // ---

        bool initialized = false;

        int currentStateIndex;

        // ---

        public CSM.State CurrentState => states[currentStateIndex];

        public Character SourceCharacter { get; private set; }

        // ---

        public void Initialize(Character source)
        {
            SourceCharacter = source;

            foreach (var state in states)
            {
                state.Initialize(this);
            }

            initialized = true;
        }

        public void ChangeStateTo(int stateIndex)
        {
            if (!initialized) throw new System.Exception("State Machine not initialized!");

            CurrentState.ExitState();

            currentStateIndex = stateIndex;

            CurrentState.EnterState();
        }

        public void UpdateStateMachine(UpdateType updateType)
        {
            if (!initialized) throw new System.Exception("State Machine not initialized!");

            CurrentState.UpdateState(updateType);

            var activeTransiton = transitions.Where(x => x.FromIndex == currentStateIndex).FirstOrDefault(x => x.CanTransition());
            if (activeTransiton != null)
            {
                ChangeStateTo(activeTransiton.ToIndex);
            }
        }

        // ---

        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}
