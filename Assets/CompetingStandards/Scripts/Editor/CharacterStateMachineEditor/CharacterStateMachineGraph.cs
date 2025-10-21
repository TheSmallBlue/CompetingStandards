using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterStateMachineGraph : GraphView
{
    public new class UxmlFactory : UxmlFactory<CharacterStateMachineGraph, GraphView.UxmlTraits> { };

    public CharacterStateMachineGraph()
    {
        // Make sure the box grows with the window
        style.flexGrow = 1f;

        // Add the grid background
        Insert(0, new GridBackground());

        // Add the manipulators
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public void CreateNode()
    {
        var newGraphNode = new CharacterStateMachineGraphNode();
        AddElement(newGraphNode);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("New Node", _ => CreateNode());
        evt.menu.AppendSeparator();
        base.BuildContextualMenu(evt);
    }
}
