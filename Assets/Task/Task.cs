using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Task", menuName = "Task/New Task")]
public class Task : ScriptableObject
{
    public string taskName;
    public string taskDescription;
    public int goal;
    public int currentProgress;
    public bool isComplete;
}
