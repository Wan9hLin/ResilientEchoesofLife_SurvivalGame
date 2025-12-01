using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerAttack : MonoBehaviour
{
    public string animalTag = "Animal";
    private bool isAttacking = false;



    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)  // 检测鼠标左键是否被按下
        {

            isAttacking = true;
            
        }
    }



    void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.CompareTag(animalTag))
        {
            // 试图获取GenerAnimalController组件
            AnimalHealthController animal = other.transform.parent.GetComponent<AnimalHealthController>();
            //Debug.Log("Attack animal");
            if (animal != null)
            {
                Debug.Log("Attack animal");
                animal.TakeDamage(1);  // 这里假设每次攻击会对动物造成1点伤害
                isAttacking = false;
                return;
            }


/*
            // 试图获取GenerAnimalController_2组件
            GenerAnimalController_2 animal2 = other.GetComponent<GenerAnimalController_2>();
            if (animal2 != null)
            {
                Debug.Log("Attack animal 2");
                animal2.TakeDamage(1);  // 这里假设每次攻击会对动物造成1点伤害
                isAttacking = false;
                return;
            }*/
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Forest")
        {
            //TaskManager.instance.NextTask();
            TaskManager.instance.AddProgress();
            Debug.Log("Forest");
        }
    }
}