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

    public class Inventory{

        // Public
        public ItemInstance[] inventory {private set; get;}
        public ItemInstance held_item {private set; get;}
        public Vector2Int scale {private set; get;}

        // Private
        bool[] occupied_tiles;

        // Constants
        static readonly Vector2Int default_scale = new Vector2Int(8, 4);
        const int line_width = 8;
        
        // Creation //
        
        public Inventory() : this(default_scale) { }

        public Inventory(Vector2Int passed_scale){
            scale = passed_scale;
            inventory = new ItemInstance[Size()];
            occupied_tiles = new bool[Size()];
            RefreshOccupation();
        }

        // Functionality //

        // Getters

        public int Size(){return (scale.x * scale.y);}
        public Vector2Int Scale(){return scale;}

        // Validation

        public bool CoordTaken(int x, int y){return CoordTaken(new Vector2Int(x, y));}
        public bool CoordTaken(Vector2Int coord){
            int index = CoordToIndex(coord);
            if(!ValidSingularBound(index))
                return false;
            return occupied_tiles[index];
        }

        bool ValidMap(){
            if(!CheckRectangular(map))
                return;
            ItemInstance[] generated_slots = MapToSlots(map);
            
        }

        bool ValidBounds(ItemInstance item, int location, ItemInstance[] slots){
            if(!ValidSingularBound(location))
                return false;

            int end_pos = location;
            for(int i = 0; i < item.Scale().y - 1; i++)
                end_pos += line_width;
            end_pos += item.Scale().x;

            return (end_pos < slots.Length);
        }

        bool ValidSingularBound(int location, ItemInstance[] slots){return (location > -1 && location < slots.Length);}

        // Generation

        public void LoadMap(InventoryMap map){
            if(!ValidMap(map))
                return;
        }

        public bool AddItem(ItemInstance item, int location){
            RefreshOccupation();

            if(item == null)
                return false;
            if(!ValidBounds(item, location))
                return false;
            if(!CanFitItem(item, location))
                return false;

            inventory[location] = item;
            RefreshOccupation();
            return true;
        }

        // Item Fitting

        int CoordToIndex(Vector2Int coord){return coord.x + (coord.y * line_width);}

        bool CanFitItem(ItemInstance item, int location){
            RefreshOccupation();
            if(!ValidBounds(item, location))
                return false;
            foreach(int pos in CoveredLocations(item, location)){
                if(occupied_tiles[pos])
                    return false;
            }
            return true;
        }

        List<int> CoveredLocations(ItemInstance item, int location){
            List<int> locales = new List<int>();
            if(!ValidBounds(item, location))
                return locales;

            for(int row = 0; row < item.Scale().y; row++){
                int start_point = location + (row * line_width);
                for(int pos = start_point; pos < start_point + item.Scale().x; pos++){
                    locales.Add(pos);
                }
            }
            return locales;
        }

        // Occupation Management

        void ClearOccupation(){
            for(int i = 0; i < Size(); i++)
                occupied_tiles[i] = false;
        }

        void FillOccupation(){
            for(int i = 0; i < inventory.Length; i++){
                if(inventory[i] != null){
                    MarkOccupation(inventory[i], i);
                }
            }
        }

        void MarkOccupation(ItemInstance item, int location){
            if(item == null)
                return;
            if(CanFitItem(item, location))
                return;
            foreach(int pos in CoveredLocations(item, location))
                occupied_tiles[pos] = true;
        }

        void RefreshOccupation(){
            ClearOccupation();
            FillOccupation();
        }
    }
}