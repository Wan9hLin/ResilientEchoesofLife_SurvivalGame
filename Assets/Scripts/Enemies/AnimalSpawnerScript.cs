using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawnerScript : MonoBehaviour
{
    public GameObject[] animalPrefabs;   // 用于存储动物预制件的变量
    public Transform[] spawnPoints;  // 用于存储空对象的数组
    public float spawnInterval = 5.0f;  // 每隔多久生成一次动物

    private float timer = 0.0f;  // 一个计时器，用于计算上次生成动物后的时间
    private int currentAnimals = 0; // 添加一个新变量来跟踪当前动物数量
    public int maxAnimals = 10; // 添加一个新变量来限制最大动物数量


    private int index;
    private GameObject animalPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // 在游戏开始时生成随机数
        GenerateRandomNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval & currentAnimals < maxAnimals)
        {
            timer = 0.0f;
            SpawnAnimal();

            // 在生成动物之后生成新的随机数
            GenerateRandomNumbers();
        }
    }

    void GenerateRandomNumbers()
    {
        // 生成随机数并存储，等待下次使用
        index = Random.Range(0, spawnPoints.Length);
        animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
    }

    void SpawnAnimal()
    {
       
        Transform spawnPoint = spawnPoints[index];

        // 生成一个随机的偏移量
        float offsetRadius = 8.0f;  // 你可以根据需要更改这个值
        Vector3 offset = Random.insideUnitSphere * offsetRadius;
        offset.y = 0;  // 我们不想在垂直方向上有偏移

        // 计算动物的实际生成位置
        Vector3 spawnPosition = spawnPoint.position + offset;



        Instantiate(animalPrefab, spawnPosition, spawnPoint.rotation);

        currentAnimals++; // 每次生成新动物时增加计数器
    }

    public void OnAnimalDied()
    {
        currentAnimals--; // 每次生成新动物时增加计数器
    }
}
