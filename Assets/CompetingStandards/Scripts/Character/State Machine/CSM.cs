using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [CreateAssetMenu(fileName = "State Machine", menuName = "CompetingStandards/Character State Machine", order = 1)]
    public class StateMachine : ScriptableObject
    {
        [SerializeReference] CSM.State[] states;
        [SerializeReference] CSM.Transition[] transitions;
    }
}
