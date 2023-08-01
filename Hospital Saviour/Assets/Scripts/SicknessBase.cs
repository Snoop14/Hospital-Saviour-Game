using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://docs.unity3d.com/Manual/class-ScriptableObject.html
[CreateAssetMenu(fileName = "New Sickness Base", menuName = "Base Sickness")]
public class SicknessBase : ScriptableObject
{
    public string sicknessBaseName;
    public Sprite sicknessIcon;
    public Sprite sicknessIconBackGround;
}
