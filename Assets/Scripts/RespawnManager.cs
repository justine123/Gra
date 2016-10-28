using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.Characters.ThirdPerson;

public class RespawnManager : MonoBehaviour
{
    public GameObject[] myObjects;
    private ThirdPersonUserControl character;
    private float maxFactor = 20;
    private float minFactor = 1;
    private float timer;
    private float spawnTime;

    //these values depends on area dimenisions
    public float MinX = -3f;
    public float MaxX = 3f;
    public float MinZ = -70;
    public float MaxZ = 100;

    void Start()
    {
        SetRandomTime();
        character = GameObject.FindObjectOfType<ThirdPersonUserControl>();

        timer = minFactor;
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (character == null)
            character = GameObject.FindObjectOfType<ThirdPersonUserControl>();

        if (timer >= spawnTime)
        {
            Spawn();
            SetRandomTime();
        }
    }

    void Spawn()
    {
        if (character.lives <= 0)
            return;

        timer = 0;
        // Create an instance of the myObject prefab at the randomly selected spawn point's position and rotation.
        // Pick Up objects should be circa 5 times rarer than enemies
        myObjects.Where(o => o.CompareTag("Enemy") || (o.CompareTag("Pick Up") && Random.Range(1, 6) == 1))
                 .ToList()
                 .ForEach(o => Instantiate(o, new Vector3(Random.Range(MinX, MaxX), 0.5f, Random.Range(MinZ, MaxZ)), Quaternion.identity));
    }

    void SetRandomTime()
    {
        spawnTime = character.time != 0 ? Random.Range(minFactor, maxFactor) / (character.time + character.coins) : Random.Range(minFactor, maxFactor);
    }

}