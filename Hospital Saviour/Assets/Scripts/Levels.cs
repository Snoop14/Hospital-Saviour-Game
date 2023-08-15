using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://docs.unity3d.com/Manual/class-ScriptableObject.html
[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Levels : ScriptableObject
{
    public int patientCount;
    public List<Sickness> sicknessType;
    public List<float> spawnTimes;
    public int inActiveBedCount;
    public int activeBedCount;
    public int[] stars = new int[3];

    //bools to hold interactable status of machines
    public bool soupMachine;
    public bool pharmacy;
    public bool bandageDispenser;
    public bool ScalpelDispenser;
    public bool TweezerDispenser;
    public bool Surgery;
    public bool XRayMachine;
    public bool ECGMachine;


}
