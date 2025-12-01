using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookMeat : MonoBehaviour
{
    public float cookedTimer;
    public float cookedSettingTime;
    public GameObject cookedMeat;
    public GameObject effect;
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
        if (other.CompareTag("CampFire"))
        {
            cookedTimer += Time.deltaTime;
            if (cookedTimer > cookedSettingTime)
            {
                ChangeToCooked();
            }
        }

    }

    private void ChangeToCooked()
    {
        //create cooked meat
        Instantiate(cookedMeat, transform.position, Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, transform.position, Quaternion.identity),2f);
        //destroy raw meat
        Destroy(gameObject);
    }

}
