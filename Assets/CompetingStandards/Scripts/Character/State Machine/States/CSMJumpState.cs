using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompetingStandards.CSM;

namespace CompetingStandards.CSM.States
{
    [System.Serializable]
    public class CharacterStateJump : CompetingStandards.CSM.State
    {
        protected override void OnStateUpdate()
        {
            Debug.Log("Jump state");
        }
    }
}