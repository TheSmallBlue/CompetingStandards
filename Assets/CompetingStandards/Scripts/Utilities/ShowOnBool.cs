using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// Made by SmallBlue, at https://github.com/TheSmallBlue

// Usage:
// [SerializeField] bool booleanField;
// [SerializeField, ShowOnBool("booleanField")] string message = "Hello!";

/// <summary>
/// Takes in a name of a boolean property. If that property is true, shows the applied property on the inspector. If not, it doesn't.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ShowOnBool : PropertyAttribute
{
    public string FieldName;

    public ShowOnBool(string fieldName)
    {
        FieldName = fieldName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowOnBool))]
public class ShowOnBoolDrawer : PropertyDrawer
{
    public bool ShouldShow(SerializedProperty property)
    {
        var att = attribute as ShowOnBool;

        // Look for the boolean value indicated
        var boolProperty = property.serializedObject.FindProperty(att.FieldName);

        if(boolProperty == null) throw new UnityException("Couldn't find boolean property!! Make sure it's serialized!!");
        if(boolProperty.propertyType != SerializedPropertyType.Boolean) throw new UnityException("Indicated property is not a boolean!!");

        return boolProperty.boolValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => ShouldShow(property) ? base.GetPropertyHeight(property, label) : 0f;
}
#endif
