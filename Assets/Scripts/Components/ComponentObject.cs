using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
// ReSharper disable MergeIntoPattern

[DisallowMultipleComponent]
public class ComponentObject : MonoBehaviour
{
    public Current Current;
    [SerializeField] private bool[] connections = new bool[4];
    
    private int RotationIndex =>
        (int)Mathf.Floor(transform.eulerAngles.y / 90 + .5f);

    private bool[] CorrectedRotations
    {
        get
        {
            bool[] newValues = new bool[4];
            for (int i = 0; i < connections.Length; i++)
                newValues[i] = connections[(i + RotationIndex) % 4];
            return newValues;
        }
    }
    
    [Obsolete]
    private Vector3[] ConnectionVectors
    {
        get
        {
            List<Vector3> vector3s = new();
            if(CorrectedRotations[0])
                vector3s.Add(Vector3.right);
            if(CorrectedRotations[2])
                vector3s.Add(-Vector3.right);
            if(CorrectedRotations[1])
                vector3s.Add(Vector3.forward);
            if(CorrectedRotations[3])
                vector3s.Add(-Vector3.forward);
            return vector3s.ToArray();
        }
    }

    public ComponentObject[] Connections => (from v3 in ConnectionVectors
        where Circuit.Components.ContainsValue(new Int3(transform.position + v3))
        select Circuit.GetComponent(transform.position + v3)).ToArray();

    private Material[] materials;
    private void Awake() => materials = GetComponent<Renderer>().materials;

    private bool isSelected;
    public bool IsSelected
    {
        set
        {
            if(isSelected == value)
                return;

            foreach (Material mat in materials)
                if (mat.HasProperty("_IsSelected"))
                    mat.SetInt("_IsSelected", value ? 1 : 0);

            isSelected = value;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if(CorrectedRotations[0])
            Gizmos.DrawRay(transform.position, Vector3.right);
        if(CorrectedRotations[2])
            Gizmos.DrawRay(transform.position, -Vector3.right);
        if(CorrectedRotations[1])
            Gizmos.DrawRay(transform.position, Vector3.forward);
        if(CorrectedRotations[3])
            Gizmos.DrawRay(transform.position, -Vector3.forward);
        
        foreach (Vector3 v3 in ConnectionVectors)
        {
            Debug.Log("Looking for: " + new Int3(transform.position + v3).Vector);
            Gizmos.color = Circuit.Components.ContainsValue(new Int3(transform.position + v3)) ? Color.green : Color.magenta;
            Gizmos.DrawWireCube(transform.position + v3, Vector3.one);
        }
    }

    private void OnDrawGizmos()
    {
        if(Current.Index == -1) return;
        GUI.color = new Color(1f, 0.62f, 0f);
        Handles.Label(transform.position + Vector3.up*.25f, Current.Index.ToString());
    }

    private void OnDestroy() => Circuit.Components.Remove(this);
}
