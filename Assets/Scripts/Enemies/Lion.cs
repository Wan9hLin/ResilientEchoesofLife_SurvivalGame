using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lion : MonoBehaviour
{
    private AttackAnimalController_2 animalController_2;
    private bool hasPlayedDeathSound = false;  // 记录是否已播放过死亡音效
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // 获取 GenerAnimalController 的引用
        animalController_2 = GetComponent<AttackAnimalController_2>();

        // 订阅 OnDeath 事件
        animalController_2.OnDeath += PlayDeathSound;

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
        animalController_2.OnDeath -= PlayDeathSound;
    }
}
