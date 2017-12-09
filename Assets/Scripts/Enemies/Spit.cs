﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour
{
    Vector2 direction;
    float speed = 15;
    int damage = 10;
    Transform player;
    Vector2 playerPos;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerPos = player.transform.position;
        direction = (new Vector2(playerPos.x, playerPos.y + 1) - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
    }

	void Update ()
    {
        this.transform.Translate(new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Zasca escupitajo: " + other);

        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().RecieveDamage(damage);
        }

        Destroy(this.gameObject);

    }
}