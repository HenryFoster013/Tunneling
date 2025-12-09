using System.Collections;
using System.Collections.Generic;
using RandomUtils;
using UnityEngine;

namespace StationUtils{

    public class StationNameGenerator{

        string[] prefixes;
        string[] suffixes;
        string[] additionals;
        Seed seed;

        public StationNameGenerator(TextAsset pref, TextAsset suff, TextAsset addi, Seed new_seed){
            prefixes = TextToArray(pref);
            suffixes = TextToArray(suff);
            additionals = TextToArray(addi);
            seed = new_seed;
        }

        public string[] TextToArray(TextAsset text_asset){
            return text_asset.text.Split(new[] {'\n'}, System.StringSplitOptions.None);
        } 

        public string NextName(){
            bool additional_tag = seed.RangeInt(0, 3) == 0;
            string result = prefixes[seed.RangeInt(0, prefixes.Length)] + suffixes[seed.RangeInt(0, suffixes.Length)];
            if(additional_tag)
                result += " " + additionals[seed.RangeInt(0, additionals.Length)];
            return result;
        }
    }

    public class StationMap{

        Seed seed;
        StationNameGenerator name_generator;

        int main_length, above_length, below_length, above_start, below_start;
        string[] map_names;

        const int max_length = 8;

        public StationMap(Seed seed, StationNameGenerator name_gen){
            int main_length = seed.RangeInt(0, max_length + 1);
            SubMap(ref above_start, ref above_length);
            SubMap(ref below_start, ref below_length);
        }

        void SubMap(ref int start_pos, ref int length){
            start_pos = 0;
            length = 0;
            if(seed.RangeInt(0,3) == 0){
                start_pos = seed.RangeInt(1, main_length - 1);
                length = seed.RangeInt(0, main_length - start_pos - 1);
            }
        }
    }

    public class Station{

        public string name {get; private set;}
        public StationMap map {get; private set;}

        Seed seed;
        StationNameGenerator name_generator;

        // Constructors //

        public Station(){seed = new Seed();}
        public Station(int seed_key){seed = new Seed(seed_key);}
        public Station(Seed seed_pass){seed = seed_pass;}

        public void Generate(TextAsset prefixes, TextAsset suffixes, TextAsset additionals){
            name_generator = new StationNameGenerator(prefixes, suffixes, additionals, seed);
            map = new StationMap(seed, name_generator);
            name = name_generator.NextName();
        }
    }
}