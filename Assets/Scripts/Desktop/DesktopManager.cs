using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenericUtils;
using static SoundUtils;

public class DesktopManager : MonoBehaviour{
    
    [Header(" - Startup - ")]
    [SerializeField] GameObject FadeIn;
    [SerializeField] SoundEffect StartSound;
    [Header(" - Cursor - ")]
    [SerializeField] RectTransform CursorRect;
    [SerializeField] RectTransform CanvasRect;
    [SerializeField] Camera UI_Camera;
    [Header(" - Hooks - ")]
    [SerializeField] Transform WindowHolder;
    [SerializeField] Transform TabHolder;

    List<WindowManager> WindowManagers = new List<WindowManager>();

    void Start(){
        PlaySFX(StartSound);
        FadeIn.SetActive(true);
    }

    void Update(){
        Cursor.visible = false;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, UI_Camera, out localPoint);
        CursorRect.localPosition = localPoint;
    }
}
