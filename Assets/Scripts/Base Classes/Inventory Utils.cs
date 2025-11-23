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

        public ItemInstance[] inventory {private set; get;}
        public ItemInstance held_item {private set; get;}
        public Vector2 scale {private set; get;}

        bool[] occupied_tiles;

        const Vector2 default_scale = new Vector2(8, 3);
        const int line_width = 8;
        
        // Creation //
        
        public Inventory(){Inventory(default_scale);}
        public Inventory(Vector2 passed_scale){
            scale = passed_scale;
            inventory = new ItemInstance[Size()];
            occupied_tiles = new bool[Size()];
            RefreshOccupied();
        }

        // Functionality //

        public int Size(){return (scale.x * scale.y);}

        void ClearOccupied(){
            for(int i = 0; i < Size(); i++)
                occupied_tiles[i] = false;
        }

        void RefreshOccupied(){
            ClearOccupied();
        }

        public void AddItem(ItemInstance item, int position){

        }
    }
}