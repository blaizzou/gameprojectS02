﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipBehavior : MonoBehaviour
{
    private EnemyShipMovement shipMovement;
    private FollowPlayer follow;
    private Ray ray;
    private Transform player;

    private int RIGHT = 1;
    private int LEFT = -1;

    public int ShipAILevel = 1;
    public int ShipPower = 2;
    public GameObject dropUranium;
    public GameObject dropHealth;
    public GameObject deathAnimation;

    void Start()
    {
        shipMovement = GetComponent<EnemyShipMovement>();
        follow = GetComponent<FollowPlayer>();
        player = GameObject.FindGameObjectWithTag("Spaceship").transform;
        //player = GameObject.FindGameObjectsWithTag("player");
    }

    void Update()
    {
        checkDistance();
        //shipMovement.strafe(transform.right);
        //player.position += (Vector3.up /50);
    }

    void checkDistance()
    {
        if (Vector3.Distance(transform.position, player.position) < 500)
        {
            if (Vector3.Distance(transform.position, player.position) < follow.socialDistancing)
            {
                switch (ShipAILevel)
                {
                    case 1:
                        shipMovement.shoot();
                        break;
                    case 2:
                        shipMovement.shoot();
                        shipMovement.strafe(LEFT, player);
                        break;
                }
                   
                shipMovement.reactorShutDown();
            }
            else 
            {
                follow.followTarget(player);
                shipMovement.reactorIgnit();
            }
        }
    }

    void explore()
    {
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            DestroyShip();
        }
    }

    public void TakeDamage(float damage)
    {
        DestroyShip();
    }

    void DestroyShip()
    {
        int sizeDrop = Random.Range(ShipPower - 1, ShipPower + 2);
        int typeDrop = Random.Range(1, 3);
        print(typeDrop);

        GameObject animation = Instantiate(deathAnimation, transform.localPosition, Random.rotation);
        animation.transform.localScale = new Vector3(10, 10, 10);
        if (sizeDrop >= 1 && typeDrop != 0)
        {
            GameObject drop = dropUranium;
            if (typeDrop == 2)
                drop = dropHealth;
            GameObject newDrop = Instantiate(drop, transform.localPosition, transform.rotation);
            newDrop.GetComponent<DropBehavior>().setValue(sizeDrop);
        }
        Destroy(gameObject);
    }
}
