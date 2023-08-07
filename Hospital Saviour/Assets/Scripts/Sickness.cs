using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/Manual/class-ScriptableObject.html
[CreateAssetMenu(fileName = "New Sickness", menuName = "Sickness")]
public class Sickness : ScriptableObject
{
    public SicknessBase sicknessBase;
    public List<Sprite> healingOrderIcons;
    public float happinessDropLevel;
}
