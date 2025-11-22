using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New SFX", menuName = "Custom/Sound/SFX")]
public class SoundEffect : ScriptableObject
{
    [Header(" - Main - ")]
    [SerializeField] string Name;
    [SerializeField] AudioClip BaseClip;
    [SerializeField] AudioClip[] ClipRange;
    [SerializeField] AudioMixerGroup Mixer;
    [Header(" - Variation - ")]
    [SerializeField] float BasePitch = 1;
    [SerializeField] float PitchVariation = 0;
    [SerializeField] float BaseVolume = 1;
    [SerializeField] float VolumeVariation = 0;
    [Header(" - 3D - ")]
    [SerializeField] bool _3D;
    [SerializeField] float Radius;

    // FUNCTIONALITY //

    float RandomiseValue(float based, float variation){
        if(variation == 0)
            return based;
        
        bool negative = (Random.Range(0, 2) == 0);
        float mult = 1;
        if(negative)
            mult = -1;
        
        return based + (Random.Range(0, variation) * mult);
    }

    // GETTERS //

    public AudioClip Clip(){
        if(BaseClip != null)
            return BaseClip;
        return ClipRange[Random.Range(0, ClipRange.Length)];
    }

    public float GetPitch(){return RandomiseValue(BasePitch, PitchVariation);}
    public float GetVolume(){return RandomiseValue(BaseVolume, VolumeVariation);}
    public string GetName(){return Name;}
    public AudioMixerGroup GetMixer(){return Mixer;}
    public bool Is3D(){return _3D;}
    public float GetRadius(){return Radius;}
}
