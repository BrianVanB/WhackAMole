using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    //Provides access to the mole Model that will be spawned
    public GameObject molePrefab;

    //provides access to the mole object after it is instantiated
    private GameObject mole;

    //Mole properties
    public float timeVisible = 1f;      //Base time a mole is visible for
    public float randomOffset = 0.5f;   //time offset for some randomness
    private float timeRemaining;        //Time remaining of the mole being visible

    public float spawnCooldown = 5;     //Time before a mole can respawn
    private float cooldownRemaining;    //How much of the cooldown is remaining

    private bool empty = true;  //Indicates if a mole is currently alive in this hole or not

    private void Start()
    {
        //Prepare the mole prefab
        mole = Instantiate(molePrefab, transform.position, Quaternion.identity);
        mole.transform.SetParent(transform, false);
        mole.SetActive(false);
    }

    void Update()
    {
        //Mole spawning right now is a simple random
        if(CanSpawn)
            if (Random.Range(1, 1000) == 1)
                Spawn();

        //When a mole is visible, update the time the mole remains visible for
        if(!empty)
            timeRemaining -= Time.deltaTime;

        //Remove the mole when it's time is up
        if (timeRemaining < 0 && !empty)
            Despawn();

        //Reduce cooldown if there still is a cooldown remaining
        if(cooldownRemaining > 0) 
            cooldownRemaining -= Time.deltaTime;
    }

    //Indicates if a mole can spawn from this hole
    public bool CanSpawn { get { return empty && cooldownRemaining <= 0; } }

    //Spawns a mole in this hole
    public void Spawn()
    {
        empty = false;
        mole.SetActive(true);
        mole.transform.position = transform.position;
        timeRemaining = timeVisible + Random.Range(-randomOffset, randomOffset);
    }

    //removes the mole from the hole and set the cooldown before a new once can appear
    private void Despawn()
    {
        empty = true;
        mole.SetActive(false);
        cooldownRemaining = spawnCooldown;
    }

    //Returns whether a mole was hit and despawns the mole on hit
    public bool Hit()
    {
        if (!empty)
        {
            Despawn();
            return true;
        }
        return false;
    }

}
