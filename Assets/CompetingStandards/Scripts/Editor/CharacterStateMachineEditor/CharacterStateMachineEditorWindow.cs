using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterStateMachineEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("CompetingStandards/Character State Machine Editor")]
    public static void Open()
    {
        // TODO: Load statemachine info into editor

        CharacterStateMachineEditorWindow wnd = GetWindow<CharacterStateMachineEditorWindow>();
        wnd.titleContent = new GUIContent("Character State Machine Editor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        /*
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);
        */

        // Instantiate UXML
        m_VisualTreeAsset.CloneTree(root);
    }
}

/*

public static class StateMachineWindowOpener
{
    [OnOpenAsset()]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);

        if (obj is CompetingStandards.CSM.StateMachine stateMachine)
        {
            CharacterStateMachineEditorWindow.Open(stateMachine);
            return true;
        }

        return false;
    }
}

*/
