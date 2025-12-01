using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public static Parry Instance;
    public GameObject effect;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetColliderIsActive()
    {
        if (GetComponent<Collider>()) {
            return GetComponent<Collider>().enabled;
        }else
        {
            return false;
        }
        
    }

    public void ParryEffect()
    {
        StartCoroutine(ActivateObjectAfterDelay(effect));
        AudioManager.instance.PlaySFX("Parry");
    }
    
    IEnumerator ActivateObjectAfterDelay(GameObject obj)
    {
        
        obj.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        obj.SetActive(false);
    }
}
