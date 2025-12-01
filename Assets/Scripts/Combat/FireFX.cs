using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFX : MonoBehaviour
{
    public float lifeTime = 2.0f;  // 火存在的时间

    void Start()
    {
        Invoke("DestroyFire", lifeTime);
    }

    void DestroyFire()
    {
        Destroy(gameObject);
    }

}
