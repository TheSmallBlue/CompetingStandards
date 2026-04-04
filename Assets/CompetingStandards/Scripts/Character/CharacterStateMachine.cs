using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards
{
    // -- State Machine --
    
    public partial class Character : MonoBehaviour
    {
        [Header("State Machine")]
        [SerializeField] CSM.StateMachine stateMachine;

        // ---

        void SetUpCharacterSM()
        {
            stateMachine.Initialize(this);
        }

        void UpdateCharacterSM()
        {
            stateMachine.UpdateStateMachine(CSM.StateMachine.UpdateType.Update);
        }
    }
}
