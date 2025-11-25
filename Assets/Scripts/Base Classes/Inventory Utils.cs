using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryUtils{

    public class ItemInstance{
        
        public ItemDefinition Definition {private set; get;}
        
        public ItemInstance(ItemDefinition desc){
            Definition = desc;
        }

        // Definition Interactions //

        public Vector2Int Scale(){return new Vector2Int(1,1) + Definition.GetAdditionalSize();}
    }
}