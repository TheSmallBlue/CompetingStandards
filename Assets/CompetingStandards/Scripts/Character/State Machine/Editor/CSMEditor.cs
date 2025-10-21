using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [CustomEditor(typeof(CSM.StateMachine))]
    public class CSMEditor : Editor
    {
        SerializedProperty stateArray;
        SerializedProperty transitionsArray;

        // ---

        private void OnEnable()
        {
            stateArray = serializedObject.FindProperty("states");
            transitionsArray = serializedObject.FindProperty("transitions");
        }

        public override void OnInspectorGUI()
        {
            BuildStateArray();
        }

        // ---

        void BuildStateArray()
        {
            // List Background
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), new Color(0.275f, 0.275f, 0.275f, 1.000f));
            if (stateArray.arraySize > 0)
                StateArrayField(); // List of states
            else
                EditorGUILayout.LabelField("Empty!");
            EditorGUILayout.EndVertical();

            // Array size field, "Add State" button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            AddStateButton();
            EditorGUILayout.EndHorizontal();

            // Save changes
            serializedObject.ApplyModifiedProperties();
        }

        void StateArrayField()
        {
            for (int i = 0; i < stateArray.arraySize; i++)
            {
                SerializedProperty state = stateArray.GetArrayElementAtIndex(i);

                // Containing box
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), new Color(0.192f, 0.192f, 0.192f, 1.000f));

                // Properties for the selected type
                EditorGUILayout.PropertyField(state, new GUIContent($"State {i}: {GetPropertyTypeNameAsString(state)}"), true);

                EditorGUILayout.LabelField("Transitions: ");
                TransitionsArrayField(); // List of transitions from that state

                // Remove item button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                RemoveStateButton(i);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        void TransitionsArrayField()
        {
            // TODO
        }

        // ---

        void AddStateButton()
        {
            if (!GUILayout.Button("+")) return;

            // Create a dropdown menu with all state types as our items
            var menu = new GenericMenu();
            var states = GetAllStateTypes();

            foreach (var stateType in states)
            {
                menu.AddItem(new GUIContent(stateType.Name), false, () => AddState(stateType));
            }

            menu.ShowAsContext();
        }

        void RemoveStateButton(int stateIndex)
        {
            if (!GUILayout.Button("-")) return;

            RemoveState(stateIndex);
        }

        // ---

        void AddState(Type type)
        {

            stateArray.arraySize++;
            stateArray.GetArrayElementAtIndex(stateArray.arraySize - 1).managedReferenceValue = Activator.CreateInstance(type);
        }

        void RemoveState(int index)
        {
            stateArray.DeleteArrayElementAtIndex(index);
        }

        // ---

        private string GetPropertyTypeNameAsString(SerializedProperty input)
        {
            var fullName = input.managedReferenceFullTypename;
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }

        // ---

        private List<Type> GetAllStateTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(CSM.State).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToList();
        }

        private List<string> GetAllStateTypesString()
        {
            return GetAllStateTypes().Select(x => x.Name).ToList();
        }
    }
}
