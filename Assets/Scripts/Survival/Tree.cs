using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] private GameObject fxTreeDestroyed;
    //[SerializeField] private Transform treeStump;

    private int health = 3;  // 这里将树的生命值设置为3
    private MeshCollider meshCollider; // 记录 MeshCollider 组件
    private Rigidbody rb; // 记录 Rigidbody 组件

    [SerializeField] private GameObject fxHit; 
    [SerializeField] private GameObject fxHitBlocks;
    [SerializeField] private GameObject fxTreeDestroyFX;

    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject logPrefab;

    private void Awake()
    {
        // 获取或者添加 Rigidbody 和 MeshCollider 组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // 初始时 Rigidbody 是不活动的

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.convex = true; // 设置 MeshCollider 为 Convex 以进行物理模拟
        meshCollider.enabled = false; // 初始时 MeshCollider 是不活动的
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Axe")
        {
            Debug.Log("Axe enyert");
            TakeDamage(1, other);
        }
    }

    private void TakeDamage(int damage, Collider axeCollider)
    {
        health -= damage;
        AudioManager.instance.PlaySFX("CutdownTrees");

        // 获取斧头击中的位置
        Vector3 hitPoint = axeCollider.ClosestPoint(transform.position);

        // 在击中的位置生成特效
        Instantiate(fxHit, hitPoint, transform.rotation);
        Instantiate(fxHitBlocks, hitPoint, transform.rotation);

        if (health <= 0)
        {
            // 在原来的位置创建一个树桩
          // Instantiate(treeStump, transform.position, transform.rotation);

            // 创建树倒下的特效
            Instantiate(fxTreeDestroyed, transform.position, transform.rotation);

           

            rb.isKinematic = false;
            meshCollider.enabled = true;
            transform.position += new Vector3(0, 0.2f, 0);

            // 根据斧头的击打方向添加力和旋转力矩
            Vector3 forceDirection = (transform.position - axeCollider.transform.position).normalized;
            rb.AddForce(forceDirection * 1000, ForceMode.Impulse);
            rb.AddTorque(new Vector3(0, forceDirection.y > 0 ? 50 : -50, 0), ForceMode.Impulse);


            Invoke(nameof(DestroyTree), 8f);
        }
    }

    private void DestroyTree()
    {
        Instantiate(fxTreeDestroyFX, transform.position, transform.rotation);

        int branchCount = Random.Range(0, 4); // 随机生成树枝的数量，范围是0-3
        int logCount = 3 - branchCount; // 树干的数量是3减去树枝的数量

        // 生成树枝
        for (int i = 0; i < branchCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 dropPosition = transform.position + randomOffset;
            Instantiate(branchPrefab, dropPosition, Quaternion.identity);
        }

        // 生成树干
        for (int i = 0; i < logCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 dropPosition = transform.position + randomOffset;
            Instantiate(logPrefab, dropPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

