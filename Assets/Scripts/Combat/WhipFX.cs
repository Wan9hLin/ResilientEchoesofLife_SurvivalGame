using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipFX : MonoBehaviour
{
    public GameObject fireEffectPrefab;

    public void SpawnFireEffect()
    {
        if (fireEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 5.0f;
            GameObject fireEffect = Instantiate(fireEffectPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

            Destroy(fireEffect, 2f);
        }
    }

}
