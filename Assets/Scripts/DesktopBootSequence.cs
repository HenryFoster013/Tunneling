using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static GenericUtils;
using static SoundUtils;

public class DesktopBootSequence : MonoBehaviour{
    
    [Header(" - Main - ")]
    [SerializeField] string NextScene;
    [Header(" - SFX - ")]
    [SerializeField] SoundEffect StartupSound;
    [SerializeField] SoundEffect LogoSound;
    [SerializeField] SoundEffect CommandSound;
    [Header(" - UI - ")]
    [SerializeField] TMP_Text CommandText;
    [SerializeField] TextAsset Lines;
    [SerializeField] GameObject Logo;

    const float initial_delay = 1.5f;
    const float login_delay = 1.5f;
    const float command_delay = 0.05f;
    const float before_logo_display = 1f;
    const float dark_delay = 1f;
    const float logo_time = 6f;

    const string ready_text = "[<color=\"green\">READY</color>]";
    const string error_text = "[<color=\"red\">ERROR</color>]";

    string commands;
    string[] lines;

    void Start(){
        StartCoroutine(LoginSequence());
    }

    void Update(){
        Cursor.visible = false;
    }

    IEnumerator LoginSequence(){
        lines = TextToArray(Lines);
        Logo.SetActive(false);
        CommandText.gameObject.SetActive(true);
        PlaySFX(StartupSound);

        commands = "";
        CommandText.text = commands;

        for(int i = 0; i < lines.Length; i++){
            if(i == 0){
                yield return new WaitForSeconds(initial_delay);
                commands += "\n";
            }
            else if(i == 1){
                yield return new WaitForSeconds(login_delay);
                commands += "\n";
            }
            else{
                yield return new WaitForSeconds(command_delay + Random.Range(0f, 0.2f));
                if(Random.Range(0, 5) == 0)
                    commands += error_text;
                else
                    commands += ready_text;
            }

            PlaySFX(CommandSound);
            commands += lines[i] + "\n";
            CommandText.text = commands;
        }

        yield return new WaitForSeconds(before_logo_display);
        CommandText.gameObject.SetActive(false);
        yield return new WaitForSeconds(dark_delay);
        Logo.SetActive(true);
        PlaySFX(LogoSound);
        yield return new WaitForSeconds(logo_time);
        SceneManager.LoadScene(NextScene);
    }

}
