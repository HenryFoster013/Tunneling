using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopManager : MonoBehaviour{
    
    [Header(" - Hooks - ")]
    [SerializeField] Transform WindowHolder;
    [SerializeField] Transform TabHolder;

    List<WindowManager> WindowManagers = new List<WindowManager>();

}
