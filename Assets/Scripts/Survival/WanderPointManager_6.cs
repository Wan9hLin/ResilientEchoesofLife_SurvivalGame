using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderPointManager_6 : MonoBehaviour
{
    public static WanderPointManager_6 Instance;  // 静态的WanderPointsManager实例，可以被其他脚本访问

    public Transform[] wanderPoints;  // 存储所有漫步点的数组

    void Awake()
    {
        // 在Awake方法中，将这个脚本的实例保存到静态的Instance变量中
        Instance = this;
    }
}
