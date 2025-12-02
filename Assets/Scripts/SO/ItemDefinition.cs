using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Inventory/Item")]
public class ItemDefinition : ScriptableObject
{
    [Header(" - Main - ")]
    [SerializeField] string Name;
    [SerializeField] string Type;
    [Header(" - Extras - ")]
    [SerializeField] SoundEffect PickupSound;
    [SerializeField] SoundEffect CollisionSound;
    [SerializeField] GameObject Model;

    // Getters //

    public string GetName(){return Name;}
    public string GetTypeDef(){return Type.ToUpper();}
    public GameObject GetPrefab(){return Model;}
    public SoundEffect GetPickupSound(){return PickupSound;}
    public SoundEffect GetCollisionSound(){return CollisionSound;}
}