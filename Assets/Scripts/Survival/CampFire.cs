using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{

    public float addWarmthTimer;
    public float addWarthRate=2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatsManager warmth= other.GetComponent<PlayerStatsManager>();
            warmth.isWarthDecrease = false;
            addWarmthTimer += Time.deltaTime;
            if(addWarmthTimer > addWarthRate)
            {
                warmth.AddWarmthValue();

            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            other.GetComponent<PlayerStatsManager>().isWarthDecrease = true;

           

        }
    }
}
