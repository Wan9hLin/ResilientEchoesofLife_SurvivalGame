using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance = null;
    public List<Task> tasks;
    public int TaskIndex = 0;
    public Task currentTask;

    private bool Wpressed, Apressed, Spressed, Dpressed,SpacePressed,IsTask1;

    // 添加任务
    public void AddTask(Task newTask, TaskUI taskUI)
    {
        tasks.Add(newTask);
    }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentTask = tasks[TaskIndex];
        TaskUI.instance.UpdateTaskUI(currentTask);
        IsTask1 = true;
    }
    void Update()
    {
        if(Keyboard.current.wKey.wasPressedThisFrame)
        {
            Wpressed = true;
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Apressed = true;
        }
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Spressed = true;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Dpressed = true;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpacePressed = true;
        }

        if(Wpressed && Apressed && Spressed && Dpressed && SpacePressed && IsTask1)
        {
            NextTask();
            IsTask1 = false;
        }
    }
    // 根据任务名称获取任务
    public Task GetTaskByName(string taskName)
    {
        return tasks.Find(task => task.taskName == taskName);
    }

    public void NextTask()
    {
        currentTask.isComplete = true;
        TaskIndex++;
        currentTask = tasks[TaskIndex];
        TaskUI.instance.UpdateTaskUI(currentTask);
    }

    public void AddProgress()
    {
        if(currentTask.currentProgress == currentTask.goal)
        {
            NextTask();
        }
        else 
        {
            currentTask.currentProgress++;
            TaskUI.instance.UpdateTaskUI(currentTask);
            if (currentTask.currentProgress == currentTask.goal)
            {
                NextTask();
            }
        }
    }

}
