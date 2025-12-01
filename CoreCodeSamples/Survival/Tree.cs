using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tree : MonoBehaviour
{
    private int health = 3;  
    private MeshCollider meshCollider; 
    private Rigidbody rb; 

    [SerializeField] private GameObject fxHit; 
    [SerializeField] private GameObject fxHitBlocks;
    [SerializeField] private GameObject fxTreeDestroyFX;
    [SerializeField] private GameObject fxTreeDestroyed;
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject logPrefab;


    private void Awake()
    {
        // Initialize Rigidbody and MeshCollider components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; 

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.convex = true; 
        meshCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect axe hits and apply damage
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

        // Get the point of impact and spawn hit effects
        Instantiate(fxHit, hitPoint, transform.rotation);
        Instantiate(fxHitBlocks, hitPoint, transform.rotation);

        if (health <= 0)
        {
            // Spawn felling effect and enable physics
            Instantiate(fxTreeDestroyed, transform.position, transform.rotation);    
            rb.isKinematic = false;
            meshCollider.enabled = true;
            transform.position += new Vector3(0, 0.2f, 0);

            // Apply force and torque based on axe's impact direction
            Vector3 forceDirection = (transform.position - axeCollider.transform.position).normalized;
            rb.AddForce(forceDirection * 1000, ForceMode.Impulse);
            rb.AddTorque(new Vector3(0, forceDirection.y > 0 ? 50 : -50, 0), ForceMode.Impulse);


            Invoke(nameof(DestroyTree), 8f);
        }
    }


    private void DestroyTree()
    {
        Instantiate(fxTreeDestroyFX, transform.position, transform.rotation);

        // Randomize the number of branches and logs dropped
        int branchCount = Random.Range(0, 4); 
        int logCount = 3 - branchCount;

        // Spawn branches
        for (int i = 0; i < branchCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 dropPosition = transform.position + randomOffset;
            Instantiate(branchPrefab, dropPosition, Quaternion.identity);
        }

        // Spawn logs
        for (int i = 0; i < logCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 dropPosition = transform.position + randomOffset;
            Instantiate(logPrefab, dropPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

