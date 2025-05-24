using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// Made by SmallBlue, at https://github.com/TheSmallBlue

// Inspired by Facepunch's S&box and their way of requiring components
// Basically, this attribute not only creates the component if its missing,
// But it also automatically sets the variable its applied to! No more GetComponent()s on Awake!

// For this to work, you need a serialized field whose type is a subclass of a Component.
// You can serialized a field by either making it public, or by adding the [SerializeField] attribute!
// Examples:
// [RequireAndAssignComponent, SerializeField] Rigidbody rb;
// [RequireAndAssignComponent] public Rigidbody rb;

// You may also supplement an additional boolean to decide if you want to show the original property field or not!
// Like so:
// [RequireAndAssignComponent(true)] public Rigidbody rb;

// One final note, for this to work correctly you have to focus the gameobject that contains the property once.
// If you don't, the component will not be added.

/// <summary>
/// Automatically assigns a component to a property if it exists, and adds it THEN assigns it to it if it doesn't.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class RequireAndAssignComponent : PropertyAttribute
{
    public bool showProperty;
    public RequireAndAssignComponent(bool shouldShowProperty = false)
    {
        showProperty = shouldShowProperty;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RequireAndAssignComponent))]
public class RequireAndAssignComponentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Make sure the thing that we require is in fact a component. Otherwise, we wouldnt be able to call GetComponent with it!
        var fieldType = fieldInfo.FieldType;
        if(!fieldType.IsSubclassOf(typeof(Component)))
        {
            throw new Exception("The attribute RequireAndAssignComponent is being applied on a non-component field!");
        }

        // Make sure that this proptertydrawer is being drawn on a Monobehaviour. Otherwise we wont be able to add components to it!
        var targetObject = property.serializedObject.targetObject as MonoBehaviour;
        if(targetObject == null)
        {
            throw new Exception("The attribute RequireAndAssignComponent exists on a class that isn't a component!");
        }

        // Try to get the component. If we can't find it, create it.
        var requiredComponent = targetObject.GetComponent(fieldType);
        if(requiredComponent == null)
        {
            requiredComponent = targetObject.gameObject.AddComponent(fieldType);
        }

        // Assing the value of the field to the newly-gotten component!
        property.objectReferenceValue = requiredComponent;
        property.serializedObject.ApplyModifiedProperties();

        // Check if we should draw the property or not.
        if((attribute as RequireAndAssignComponent).showProperty)
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (attribute as RequireAndAssignComponent).showProperty ? base.GetPropertyHeight(property, label) : 0f;
}
#endif
