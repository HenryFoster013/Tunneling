using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Inventory/Item")]
public class ItemDescription : ScriptableObject
{
    [Header(" - Main - ")]
    [SerializeField] string Name;
    [SerializeField] string Description;

    [Header(" - Tags - ")]
    [SerializeField] bool CanCarry;
    [SerializeField] bool CanEquip;

    [Header(" - 2D - ")]
    [SerializeField] GameObject PovModel;

    [Header(" - 3D - ")]
    [SerializeField] GameObject Model;
    [SerializeField] bool _Rigidbody;
    [SerializeField] float Weight;
    [SerializeField] float Bounciness;
    [SerializeField] SoundEffect CollisionSound;

    // Getters //

    public string GetName(){return Name;}
    public string GetDescription(){return Description;}
    public bool Carriable(){return CanCarry;}
    public bool Equipable(){return CanEquip;}
    public GameObject GetPovModel(){return PovModel;}
    public GameObject GetModel(){return Model;}
    public bool IsRigidbody(){return _Rigidbody;}
    public float GetWeight(){return Weight;}
    public float GetBounciness(){return Bounciness;}
    public SoundEffect GetCollisionSound(){return CollisionSound;}
}