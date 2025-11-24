using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Custom/Inventory/Map")]
public class InventoryMap : ScriptableObject
{
    [Header(" - Main - ")]
    [SerializeField] string Type;
    [SerializeField] ItemDefinition[] ItemSlots;

    // Getters //

    public string GetType(){return Type;}
    public ItemDefinition[] GetSlots(){return ItemSlots;}
}