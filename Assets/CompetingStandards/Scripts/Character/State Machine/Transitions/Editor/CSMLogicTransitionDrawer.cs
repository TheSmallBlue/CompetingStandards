using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CompetingStandards.CSM.Transitions
{
    //[CustomPropertyDrawer(typeof(CSMLogicTransition))]
    public class CSMLogicTransitionDrawer : PropertyDrawer
    {
        // TODO: Transitions list

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.LabelField(position, "Hi :3");
            base.OnGUI(position, property, label);
        }
    }
}
