using UnityEngine;
using UnityEngine.Audio;

public static class SoundUtils{
    
    // SOUND EFFECTS //
    
    public static void PlaySFX(string name, SoundEffectLookup SoundLookup){SpawnSound(SoundLookup.GetSFX(name), Vector3.zero);}
    public static void PlaySFX(string name, Vector3 position, SoundEffectLookup SoundLookup){SpawnSound(SoundLookup.GetSFX(name), position);}
    public static void PlaySFX(SoundEffect sound){SpawnSound(sound, Vector3.zero);}
    public static void PlaySFX(SoundEffect sound, Vector3 position){SpawnSound(sound, position);}

    public static void SpawnSound(SoundEffect sound, Vector3 position){
        if(sound == null)
            return;

        GameObject new_sfx = new GameObject(sound.GetName());
        new_sfx.transform.position = position;
        
        AudioSource asrc = new_sfx.AddComponent<AudioSource>();
        asrc.clip = sound.Clip();
        asrc.volume = sound.GetVolume();
        asrc.pitch = sound.GetPitch();
        asrc.outputAudioMixerGroup  = sound.GetMixer();

        if(sound.Is3D()){
            asrc.spatialBlend = 1.0f;
            asrc.minDistance = 0.001f;
            asrc.maxDistance = sound.GetRadius();
            asrc.rolloffMode = AudioRolloffMode.Linear;
            asrc.dopplerLevel = 0f;
        }

        asrc.Play();

        DestroyOverTime deletor = new_sfx.AddComponent<DestroyOverTime>();
        deletor.StartDeletion(asrc.clip.length + 1f);
        GameObject.DontDestroyOnLoad(new_sfx);
    }

    public static float FloatToDecibel(float input){
        input = Mathf.Clamp(input, 0f, 1f);
        input = Mathf.Clamp(Mathf.Log10(input) * 20f, -80f, 0f);
        return input;
    }

    // LOOKUPS //

    public static SoundEffectLookup GetSFXLookup(){return Resources.Load<SoundEffectLookup>("_ SFX Lookup");}
}
