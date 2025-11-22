using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryUtils{

    public class ItemInstance{
        
        public ItemDescription description {private set; get;}
        
        public ItemInstance(ItemDescription desc){
            description = desc;
        }
    }

    public class Inventory{

        public ItemInstance[] backpack_slots {private set; get;}
        public ItemInstance held_item {private set; get;}

        const int default_size = 3;
        
        public Inventory(){
            backpack_slots = new ItemInstance[default_size];
        }
    }
}