using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomUtils{

    public class Seed{

        public int value {get; private set;}
        System.Random random;

        public Seed(){
            value = Random.Range(0, 999999999);
            random = new System.Random(value);
        }

        public Seed(int seed){
            value = seed;
            random = new System.Random(value);
        }

        // By defalt returns [0,1], this multiplies that value by the range (max - min), then adds the offset (min)
        public float Range(float min, float max){return (float)(random.NextDouble() * (max - min) + min);}
        public int RangeInt(int min, int max){return random.Next(min, max);}
        public int RandomInt(){return random.Next();}

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public string RandomString(int length){
            string result = "";
            for (int i = 0; i < length; i++){
                result += chars[RangeInt(0, chars.Length)];
            }
            return result;
        }
    }
}