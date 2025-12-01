using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target;
    public GameObject fireBallObj;
    public GameObject fireField;
    // Start is called before the first frame update
    private void Awake()
    {
       
        rb = GetComponent<Rigidbody>();

    }


    private void FixedUpdate()
    {
        if (target != null)
        {
            // 计算火球指向玩家的方向
            Vector3 directionToTarget = target.position+new Vector3(0, 1f, 0) - transform.position;
            directionToTarget.Normalize();

            // 通过刚体给火球施加力，使其向玩家移动
            rb.velocity = directionToTarget * 100f;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log(collision.transform.name);
            PlayerStatsManager.Instance.TakeDamage(10);
            
            if (PlayerToolController.Instance.currentItemData.itemName == "Shield" && Parry.Instance.GetColliderIsActive())
            {   
                //parry success
                GameObject fireball = Instantiate(fireBallObj, collision.contacts[0].point + Vector3.forward * 1f, Quaternion.identity);
                int randomNumber = Random.Range(0, 2);
                switch (randomNumber)
                {
                    case 0:
                        fireball.GetComponent<Rigidbody>().AddForce(transform.forward * 10f + Vector3.up * 8f + Vector3.left * 10f, ForceMode.Impulse);
                        Debug.Log("left fireball");
                        break;
                    case 1:
                        fireball.GetComponent<Rigidbody>().AddForce(transform.forward * 10f + Vector3.up * 8f + Vector3.right * 10f, ForceMode.Impulse);
                        Debug.Log("right fireball");
                        break;
                }
                

            }
            else
            {
                SpawnFirefieldOnSlope(collision.transform);

            }

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerStatsManager playerStas = other.transform.root.GetComponentInParent<PlayerStatsManager>();
        Debug.Log(other.transform.name + "@@@" + playerStas);
        if (playerStas && PlayerToolController.Instance.currentItemData.itemName == "Shield")
        {
            playerStas.TakeDamage(10);
            Debug.Log("damage player");
            

            Vector3 fireballSpawnPoint = other.ClosestPointOnBounds(transform.position+Vector3.forward*2f);
            GameObject fireball = Instantiate(fireBallObj, fireballSpawnPoint, Quaternion.identity);
            int randomNumber = Random.Range(0, 2);
            switch (randomNumber)
            {
                case 0:
                    fireball.GetComponent<Rigidbody>().AddForce(transform.forward * 10f + Vector3.up * 8f + Vector3.left * 10f, ForceMode.Impulse);
                    Debug.Log("left fireball");
                    break;
                case 1:
                    fireball.GetComponent<Rigidbody>().AddForce(transform.forward * 10f + Vector3.up * 8f + Vector3.right * 10f, ForceMode.Impulse);
                    Debug.Log("right fireball");
                    break;
            }
        }else
        {
            SpawnFirefieldOnSlope(other.transform);
        }
    }

    void SpawnFirefieldOnSlope(Transform playerTransform)
    {
        // 随机生成在玩家附近的位置
        Vector3 randomOffset = Random.insideUnitCircle.normalized * 5f;
        Vector3 firefieldSpawnPoint = playerTransform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);

        // 发射射线从生成位置朝向下方
        if (Physics.Raycast(firefieldSpawnPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            // 获取斜坡表面的法线
            Vector3 slopeNormal = hit.normal;


            GameObject fireFieldObj = Instantiate(fireField, hit.point, Quaternion.FromToRotation(Vector3.up, slopeNormal));


            // 创建一个新的物体，并旋转使其贴合在斜坡上
            /*var newObject = Instantiate(fireField, hit.point , Quaternion.identity);*/
        }
    }
       
    
}
