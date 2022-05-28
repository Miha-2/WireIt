using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Circuit
{
   public static Dictionary<ComponentObject, Int3> Components = new();

   public static Junction[] Junctions =>
       (from junc in Components.Where(comp => comp.Key is Junction)
           select junc.Key).Cast<Junction>().ToArray();
   
   public static ComponentObject GetComponent(Int3 location)
   {
       if(!Components.ContainsValue(location))
       {
           Debug.LogError("Trying to access non existent component");
           return null;
       }
       
       return Components.First(kvp => kvp.Value.Equals(location)).Key;
   }
   public static ComponentObject GetComponent(Vector3 location) => GetComponent(new Int3(location));
}

public readonly struct Int3
{
    private readonly int iX;
    private readonly int iY;
    private readonly int iZ;

    public Int3(float x, float y, float z)
    {
        iX = Mathf.RoundToInt(x);
        iY = Mathf.RoundToInt(y);
        iZ = Mathf.RoundToInt(z);
    }
    public Int3(Vector3 v3)
    {
        iX = Mathf.RoundToInt(v3.x);
        iY = Mathf.RoundToInt(v3.y);
        iZ = Mathf.RoundToInt(v3.z);
    }

    public Vector3 Vector => new(iX, iY, iZ );

    public bool Equals(Int3 compare) => iX == compare.iX && iY == compare.iY && iZ == compare.iZ;
}

public readonly struct Current
{
    public Current(int index)
    {
        this.Index = index;
        Value = -1;
    }
    public int Index { get; }
    public float Value { get; }
}
