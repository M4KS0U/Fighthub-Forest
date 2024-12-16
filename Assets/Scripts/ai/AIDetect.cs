using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetect : MonoBehaviour
{
    public enum DETECT_TYPE
    {
        PAWN,
        AI,
        WALL,
    };

    [Tooltip("The type of the object (Pawn = Player/Controllable)")]
    [Header("Detect Type")]
    public DETECT_TYPE detectTypeField;

    public DETECT_TYPE detectType { get; set; }

    public bool IsType(DETECT_TYPE type) { return detectType == type; }
}
