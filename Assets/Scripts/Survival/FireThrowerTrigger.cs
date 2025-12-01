using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThrowerTrigger : MonoBehaviour
{
    public GameObject BossCG2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && BossHealthController.instance.currentHealth >= 50)
        {
            BossController.instance.ChangeState(BossController.BossState.FireThrower);
        }

        if (other.tag == "Player" && BossHealthController.instance.currentHealth < 50)
        {
            BossCG2.SetActive(true);
        }
    }
}
