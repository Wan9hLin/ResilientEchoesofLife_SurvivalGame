using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    private WanderAnimalController animalController;
    private bool hasPlayedDeathSound = false;  // 记录是否已播放过死亡音效
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // 获取 GenerAnimalController 的引用
        animalController = GetComponent<WanderAnimalController>();

        // 订阅 OnDeath 事件
        animalController.OnDeath += PlayDeathSound;

        audioSource = GetComponent<AudioSource>();
    }

    private void PlayDeathSound()
    {
        if (!hasPlayedDeathSound)
        {
            Debug.Log("DeadSound");
            audioSource.Play();
            hasPlayedDeathSound = true;
        }
    }

    private void OnDestroy()
    {
        // 当动物被销毁时，取消订阅 OnDeath 事件
        animalController.OnDeath -= PlayDeathSound;
    }
}
