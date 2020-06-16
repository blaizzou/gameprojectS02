﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyShipBehavior : MonoBehaviour
{
    private EnemyShipMovement shipMovement;
    private FollowPlayer follow;
    private Ray ray;
    private Transform player;

    private float life;
    private float damage;

    private float dmgAnimeLapse = 0.1f;
    private float dmgAnimeCount = 0;
    private bool damaged = false;

    public int ShipAILevel = 1;
    public int ShipPower = 2;
    public GameObject dropUranium;
    public GameObject dropHealth;
    public GameObject deathAnimation;
    public ParticleSystem damageSmoke;
    public Material damageTexture;
    private Material originalTexture;

    void Start()
    {
        shipMovement = GetComponent<EnemyShipMovement>();
        follow = GetComponent<FollowPlayer>();
        player = GameObject.FindGameObjectWithTag("Spaceship").transform;
        //player = GameObject.FindGameObjectsWithTag("player");
        life = ShipPower * 6;
        shipMovement.setDamage(ShipPower * 2);
        originalTexture = GetComponent<Renderer>().material;
        damageSmoke.Stop();
    }

    void Update()
    {
        checkDistance();
        //shipMovement.strafe(transform.right);
        //player.position += (Vector3.up /50);
        if (damaged)
            checkDmgAnimation();
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
                        follow.followTarget(player);
                        break;
                    case 2:
                        shipMovement.shoot();
                        shipMovement.strafe(-1, player);
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

    public void TakeDamage(float playerDamage)
    {
        print("enemy received " + playerDamage + " damage");
        life -= playerDamage;
        GetComponent<Renderer>().material = damageTexture;
        dmgAnimeCount = dmgAnimeLapse;
        damaged = true;
        damageSmoke.Play();
        if (life <= 0)
            DestroyShip();
    }

    private void checkDmgAnimation()
    {
        if (dmgAnimeCount > 0)
        {
            dmgAnimeCount -= Time.deltaTime;
            if (dmgAnimeCount <= 0)
                GetComponent<Renderer>().material = originalTexture;

        }
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
