using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitBrain : MonoBehaviour
{
    public readonly Dictionary<Current, List<ComponentObject>> Currents = new();
    public Dictionary<Current, float> Resistances
    {
        get
        {
            return Currents.ToDictionary(pair => pair.Key,
                pair => Currents[pair.Key]
                    .Where(comp => comp is Resistor)
                    .Cast<Resistor>()
                    .Sum(res => res.Resistance));
        }
    }

    private void Awake()
    {
        Circuit.Components = new Dictionary<ComponentObject, Int3>();
        foreach (ComponentObject component in FindObjectsOfType<ComponentObject>())
            Circuit.Components.Add(component, new Int3(component.transform.position));

        foreach (Int3 i3 in Circuit.Components.Values)
        {
            Debug.Log(i3);
        }
    }

    private void Start()
    {
        AssignCurrents();
    }

    private void OnDrawGizmos()
    {
        //Draw Junctions
        Gizmos.color = Color.yellow;
        foreach (Junction junction in Circuit.Junctions)
            Gizmos.DrawSphere(Circuit.Components[junction].Vector, .125f);
    }

    private void AssignCurrents()
    {
        //ToDo: Check if a circuit is complete!!!
        
        if (Circuit.Components.Count == 0)
        {
            Debug.LogError("There are no components to assign values to!");
            return;
        }
        
        foreach (ComponentObject component in Circuit.Components.Keys) component.Current = new Current(-1);

        if (Circuit.Junctions.Length == 0)
        {
            Debug.Log("There are no junctions in this circuit!");
            SetCurrent(new Current(0), Circuit.Components.Keys.ToArray()[0], null);
        }
        
        int currentIndex = 0;
        //If junctions are 0 -> start from component
        foreach (Junction junction in Circuit.Junctions)
        {
            foreach (ComponentObject connection in junction.Connections)
            {
                if(connection is Junction) continue;
                if(connection.Current.Index != -1) continue;
                SetCurrent(new Current(currentIndex), connection, junction);
                currentIndex += 1;
            }
        }
    }

    private void SetCurrent(Current current, ComponentObject component, ComponentObject previousComponent)
    {
        if (component.Current.Index != -1)
        {
            Debug.LogError("Trying to modify already set current!");
            return;
        }
        component.Current = current;
        
        if (Currents.ContainsKey(current))
            Currents[current].Add(component);
        else
            Currents.Add(current, new List<ComponentObject>{component});
        
        foreach (ComponentObject connection in component.Connections)
        {
            if(connection is Junction || connection == previousComponent)
                continue;
            SetCurrent(current, connection, component);
        }
    }
}