using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an object that 'saves' the values of an inventory in Assets
// Used to create preset inventory layouts for placing lootable objcets in the game world

[CreateAssetMenu(fileName = "New Map", menuName = "Custom/Inventory/Map")]
public class InventoryMap : ScriptableObject
{
    [Header(" - Main - ")]
    [SerializeField] string Type;
    [SerializeField] Vector2 Scale = new Vector2(8, 4);
    [SerializeField] ItemDefinition[] ItemSlots;

    public Inventory ToInventory(){

        if(!ValidScale())
            return null;

        Inventory new_inventory = new Inventory(scale);

        for(int  i = 0; i < ItemSlots.Length; i++){
            Item item = new Item(ItemDefinition[i]);
            new_inventory.Add(item, i);
        }

        if(!new_inventory.FullValidation())
            return null;
        
        return new_inventory
    }

    bool ValidScale(){
        if(ItemSlots.Length != Scale.x * Scale.y){
            Debug.Error("Invalid scale-slot count!");
            return false;
        }
        return true;
    }

    // Getters //

    public string GetType(){return Type;}
    public ItemDefinition[] GetSlots(){return ItemSlots;}
}