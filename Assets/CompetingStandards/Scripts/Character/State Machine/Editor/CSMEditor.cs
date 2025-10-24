using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            AddItemToSerializedArrayButton(stateArray, typeof(CSM.State), out bool buttonPressed);
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

                // Transitions array
                EditorGUILayout.LabelField("Transitions: ");
                TransitionsArrayField(i); // List of transitions from that state

                // Add transition button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                AddItemToSerializedArrayButton(transitionsArray, typeof(CSM.Transition), out bool buttonPressed);
                if(buttonPressed && transitionsArray.arraySize > 0)
                {
                    ChangeTransitionSource(() => transitionsArray.arraySize - 1, i);
                }
                EditorGUILayout.EndHorizontal();

                // Remove item button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                RemoveItemFromSerializedArrayButton(stateArray, i);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        void TransitionsArrayField(int sourceStateIndex)
        {
            // TODO
            for (int i = 0; i < transitionsArray.arraySize; i++)
            {
                SerializedProperty transition = transitionsArray.GetArrayElementAtIndex(i);

                if (transition.FindPropertyRelative("FromIndex").intValue != sourceStateIndex)
                    continue;

                // Containing box
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), new Color(0.122f, 0.122f, 0.122f, 1.000f));

                // Properties for the selected type
                EditorGUILayout.PropertyField(transition, new GUIContent($"Transition {i}: {GetPropertyTypeNameAsString(transition)}"), true);

                // State to transition to
                EditorGUILayout.LabelField("Transition to:");
                transition.FindPropertyRelative("ToIndex").intValue = EditorGUILayout.Popup(transition.FindPropertyRelative("ToIndex").intValue, GetSerializedArrayTypesAsStrings(stateArray));

                // Remove transition button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                RemoveItemFromSerializedArrayButton(transitionsArray, i);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        // ---

        async void ChangeTransitionSource(Func<int> transitionIndexDelegate, int newValue)
        {
            await Task.Delay(1);

            transitionsArray.GetArrayElementAtIndex(transitionIndexDelegate()).FindPropertyRelative("FromIndex").intValue = newValue;
        }

        // ---

        void AddItemToSerializedArrayButton(SerializedProperty serializedArray, Type itemType, out bool buttonPressed)
        {
            buttonPressed = GUILayout.Button("+");
            if (!buttonPressed) return;

            // Create a dropdown menu with all state types as our items
            var menu = new GenericMenu();
            var states = GetAllInheritingTypesOf(itemType);

            foreach (var stateType in states)
            {
                menu.AddItem(new GUIContent(stateType.Name), false, () => AddItemToSerializedArray(serializedArray, stateType));
            }

            menu.ShowAsContext();
        }

        void RemoveItemFromSerializedArrayButton(SerializedProperty serializedArray, int itemIndex)
        {
            if (!GUILayout.Button("-")) return;

            RemoveItemFromSerializedArray(serializedArray, itemIndex);
        }

        // ---

        void AddItemToSerializedArray(SerializedProperty serializedArray, Type type)
        {
            if (!serializedArray.isArray) return;

            serializedArray.arraySize++;
            serializedArray.GetArrayElementAtIndex(serializedArray.arraySize - 1).managedReferenceValue = Activator.CreateInstance(type);
        }
        
        void RemoveItemFromSerializedArray(SerializedProperty serializedArray, int itemIndex)
        {
            serializedArray.DeleteArrayElementAtIndex(itemIndex);
        }

        // ---

        private string GetPropertyTypeNameAsString(SerializedProperty input)
        {
            var fullName = input.managedReferenceFullTypename;
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }

        string[] GetSerializedArrayTypesAsStrings(SerializedProperty serializedArray)
        {
            List<string> output = new();

            for (int i = 0; i < serializedArray.arraySize; i++)
            {
                output.Add($"{i}: {GetPropertyTypeNameAsString(serializedArray.GetArrayElementAtIndex(i))}");
            }

            return output.ToArray();
        }

        // ---

        List<Type> GetAllInheritingTypesOf(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToList();
        }
    }
}
