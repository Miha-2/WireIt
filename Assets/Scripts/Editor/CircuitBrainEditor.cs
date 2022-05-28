using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CircuitBrain))]
public class CircuitBrainEditor : Editor
{
    private CircuitBrain obj;

    private void OnEnable() => obj = (CircuitBrain)serializedObject.targetObject;

    public int a;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        // Debug.Log(obj.Currents.Count);
        
        foreach (KeyValuePair<Current,List<ComponentObject>> pair in obj.Currents)
        {
            GUIStyle style = new() { richText = true };
            EditorGUILayout.LabelField("<b>Current: " + pair.Key.Index + "</b>", style);
            EditorGUILayout.LabelField("<i>Resistance: " + obj.Resistances[pair.Key] + " Ω</i>", style);
            foreach (ComponentObject o in obj.Currents[pair.Key])
            {
                EditorGUILayout.LabelField("    " + o.name);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
