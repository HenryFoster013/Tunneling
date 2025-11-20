using UnityEngine;
using UnityEngine.Audio;

public static class GenericUtils{
    
    // SOUND EFFECTS //
    
    public static void PlaySFX(string name, SoundEffectLookup SoundLookup){SpawnSound(SoundLookup.GetSFX(name));}
    public static void PlaySFX(int id, SoundEffectLookup SoundLookup){SpawnSound(SoundLookup.GetSFX(id));}
    public static void PlaySFX(SoundEffect sound){SpawnSound(sound);}

    public static void SpawnSound(SoundEffect sound){
        if(sound == null)
            return;

        GameObject new_sfx = new GameObject(sound.Name);
        AudioSource asrc = new_sfx.AddComponent<AudioSource>();
        asrc.clip = sound.Clip();
        asrc.volume = sound.Volume();
        asrc.pitch = sound.Pitch();
        asrc.outputAudioMixerGroup  = sound.Mixer;
        asrc.Play();
        DestroyOverTime deletor = new_sfx.AddComponent<DestroyOverTime>();
        deletor.StartDeletion(asrc.clip.length + 1f);
        GameObject.DontDestroyOnLoad(new_sfx);
    }

    public static float FloatToDecibel(float input){ // float range 0 -> 1
        input = Mathf.Clamp(input, 0f, 1f);
        input = Mathf.Clamp(Mathf.Log10(input) * 20f, -80f, 0f);
        return input;
    }

    // LOOKUPS //

    public static SoundEffectLookup GetSFXLookup(){return Resources.Load<SoundEffectLookup>("_ SFX Lookup");}
}
