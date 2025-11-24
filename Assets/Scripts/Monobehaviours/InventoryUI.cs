using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventoryUtils;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject Slot;
    [SerializeField] Transform SlotHolder;
    [SerializeField] Color EmptyColour;
    [SerializeField] Color TakenColour;

    Inventory inventory;
    const float base_tile_size = 100f;

    // Main //

    void Start(){
        GenerateUI(new Inventory());
    }

    // Interaction //

    // UI Generation //

    void GenerateUI(Inventory inv){
        inventory = inv;
        Regenerate();
    }

    void CleanHolder(){
        Slot.SetActive(false);
        foreach(Transform t in SlotHolder)
            Destroy(t.gameObject);
    }

    void Regenerate(){
        CleanHolder();
        GenerateTiles();
        CentreTiles();
    }

    void CentreTiles(){
        float x_pos = ((inventory.Scale().x - 1) * -(base_tile_size / 2));
        float y_pos = ((inventory.Scale().y - 1) * (base_tile_size / 2));
        SlotHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_pos, y_pos);
    }

    void GenerateTiles(){
        for(int y = 0; y < inventory.Scale().y; y++){
            for (int x = 0; x < inventory.Scale().x; x++){
                GameObject new_tile = GameObject.Instantiate(Slot);
                new_tile.transform.parent = SlotHolder;
                new_tile.transform.localScale = new Vector3(1f,1f,1f);
                new_tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * base_tile_size, y * -base_tile_size);
                if(inventory.CoordTaken(x, y))
                    new_tile.GetComponent<Image>().color = TakenColour;
                else
                    new_tile.GetComponent<Image>().color = EmptyColour;
                new_tile.SetActive(true);
            }
        }
    }
}
