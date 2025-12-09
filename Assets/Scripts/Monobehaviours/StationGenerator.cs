using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StationUtils;
using UnityEngine.UI;
using TMPro;
using RandomUtils;

public class StationGenerator : MonoBehaviour{

    [Header(" - Text Assets - ")]
    [SerializeField] TextAsset Prefixes_TXT;
    [SerializeField] TextAsset Suffixes_TXT;
    [SerializeField] TextAsset Additionals_TXT;
    [Header(" - UI - ")]
    [SerializeField] TMP_Text Display;

    StationNameGenerator name_generator;
    Seed seed;

    void Start(){
        seed = new Seed(Random.Range(0, 9999));
        name_generator = new StationNameGenerator(Prefixes_TXT, Suffixes_TXT, Additionals_TXT, seed);
        GenerateName();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space))
            GenerateName();
    }

    void GenerateName(){
        Display.text = name_generator.NextName();
    }
}
