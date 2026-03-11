using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// TODO: Add support for UnityEngine.Object

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class SubclassPicker : PropertyAttribute
{
    
}

[CustomPropertyDrawer(typeof(SubclassPicker), true)]
public class SubclassPickerDrawer : PropertyDrawer
{
    // We need to cache our types, retrieving them each frame is too costly
    private Dictionary<Type, Type[]> typesCache = new();

    // ---

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect popupPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        DrawSubclassPopup(popupPos, property);

        if(property.managedReferenceValue != null)
        {
            SerializedProperty firstProperty = property.Copy();
            bool hasProperties = firstProperty.NextVisible(true);

            Rect nextPropertyPos = new Rect
            (
                position.x, 
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 
                position.width, 
                hasProperties ? EditorGUI.GetPropertyHeight(firstProperty, true) : 0f
            );

            foreach (var childProperty in GetChildPropertiesOf(property))
            {
                EditorGUI.PropertyField(nextPropertyPos, childProperty, true);

                nextPropertyPos.y += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue != null)
        {
            foreach (var childProperty in GetChildPropertiesOf(property))
            {
                height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        return height;
    }

    // ---

    // Unity uses a linked list for child properties of a property, by turning it into a lazy builder we can use them more easily.
    IEnumerable<SerializedProperty> GetChildPropertiesOf(SerializedProperty parentProperty)
    {
        
        SerializedProperty propertyIterator = parentProperty.Copy(); // A copy of the property, we'll iterate through this
        SerializedProperty finalProperty = propertyIterator.GetEndProperty(); // The final property inside our property, will mark our end
        propertyIterator.NextVisible(true); // Get the first property

        // Loop through properties until we've reached the final one
        while (!SerializedProperty.EqualContents(propertyIterator, finalProperty))
        {
            yield return propertyIterator;

            propertyIterator.NextVisible(false);
        }
    }

    void DrawSubclassPopup(Rect position, SerializedProperty property)
    {
        Type fieldType = fieldInfo.FieldType;

        if (!typesCache.TryGetValue(fieldType, out Type[] inheritingTypes))
        {
            inheritingTypes = GetAllInheritingTypesOf(fieldType);
            typesCache[fieldType] = inheritingTypes;
        }

        Type currentType = property.managedReferenceValue?.GetType();
        int currentTypeIndex = Array.IndexOf(inheritingTypes, currentType);

        int newTypeIndex = EditorGUI.Popup(position, currentTypeIndex, inheritingTypes.Select(t => t.Name).ToArray());

        if (newTypeIndex != currentTypeIndex)
        {
            Type newType = inheritingTypes[newTypeIndex];
            object typeInstance = Activator.CreateInstance(newType);
            property.managedReferenceValue = typeInstance;
        }
    }

    Type[] GetAllInheritingTypesOf(Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .ToArray();
    }
}
