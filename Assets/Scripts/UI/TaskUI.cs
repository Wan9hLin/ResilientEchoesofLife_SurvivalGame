using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    //public Task task;
    public static TaskUI instance = null;
    public Text taskNameText;
    public Text taskDescriptionText;
    public Text taskProgress;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 在UI上显示任务名称和描述
        /*taskNameText.text = task.taskName;
        taskDescriptionText.text = task.taskDescription;*/
    }

    public void UpdateTaskUI(Task task)
    {
        taskNameText.text = task.taskName;
        taskDescriptionText.text = task.taskDescription;
        taskProgress.text = "";
        if (task.goal > 0)
        {
            taskProgress.text = task.currentProgress.ToString() + " / " + task.goal.ToString();
        }
    }
}
