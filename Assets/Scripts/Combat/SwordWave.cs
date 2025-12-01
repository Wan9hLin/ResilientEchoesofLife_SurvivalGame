using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : MonoBehaviour
{
    public GameObject swordWavePrefab; // 这是你的剑气特效预制件

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 这个方法用于在剑处召唤剑气特效
    public void SummonSwordWave()
    {
        if (swordWavePrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.up * 1.0f;
            GameObject swordWave = Instantiate(swordWavePrefab, spawnPosition, transform.rotation);

            // 将剑气特效的方向设置为剑的方向
            swordWave.transform.up = transform.forward;

            Destroy(swordWave, 2f);
        }
    }

}

