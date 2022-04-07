using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<Type> : ScriptableObject
{
    private List<Type> items = new List<Type>();

    public void Initialize()
    {
        //Effacer la liste
        items.Clear();
    }

    //Get index of list
    public Type GetItemIndex(int index)
    {
        return items[index];
    }

    public void AddToList(Type thingToAdd)
    {
        //If it doesn't already contain the thing to add
        if (!items.Contains(thingToAdd))
        {
            items.Add(thingToAdd);
        }
    }

    public void RemoveFromList(Type thingToRemove)
    {
        //If the list has the thing we want to remove
        if (items.Contains(thingToRemove))
        {
            items.Remove(thingToRemove);
        }
    }
}
