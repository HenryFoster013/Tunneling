using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemUtils{

    public class ItemManager{

        public ItemManager() { }

        public WorldItem SpawnWorldItem(ItemDefinition item_definition, Vector3 position, Quaternion rotation, Vector3 velocity){
            return SpawnWorldItem(new ItemInstance(item_definition), position, rotation, velocity);}
        public WorldItem SpawnWorldItem(ItemInstance item_instance, Vector3 position, Quaternion rotation, Vector3 velocity){
            GameObject holder = GameObject.Instantiate(item_instance.GetPrefab());
            holder.transform.position = position;
            holder.transform.rotation = rotation;
            WorldItem world_item = holder.GetComponent<WorldItem>();
            world_item.LoadInstance(item_instance);
            world_item.SetVelocity(velocity);
            return world_item;
        }

        public ItemInstance PickupWorldItem(WorldItem world_item){
            ItemInstance item_instance = world_item.PickUp();
            Object.Destroy(world_item.gameObject);
            return item_instance;
        }
    }

    public class ItemInstance{
        
        public ItemDefinition Definition {private set; get;}

        public int stored_int;
        
        public ItemInstance(ItemDefinition desc){
            Definition = desc;
        }

        // Definition Interactions //

        public string Name(){return Definition.GetName();}
        public string GetTypeDef(){return Definition.GetTypeDef();}
        public SoundEffect GetPickupSound(){return Definition.GetPickupSound();}
        public SoundEffect GetCollisionSound(){return Definition.GetCollisionSound();}
        public GameObject GetPrefab(){return Definition.GetPrefab();}
    }
}