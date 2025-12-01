using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class MyPlayerAttack : Action
{
    public SharedString animalTag = "Animal";
    private bool isAttacking = false;

    public override void OnStart()
    {
        // 在行为树开始时执行的初始化操作
    }

    public override TaskStatus OnUpdate()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键是否被按下
        {
            isAttacking = true;
        }

        return TaskStatus.Running;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag(animalTag.Value))
        {
            AnimalHealthController animal = other.GetComponent<AnimalHealthController>();
            if (animal != null)
            {
                Debug.Log("Attack animal");
                animal.TakeDamage(1);
                isAttacking = false;
                return;
            }
        }
    }
}
