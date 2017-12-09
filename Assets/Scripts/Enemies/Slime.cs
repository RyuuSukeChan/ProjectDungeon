﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [Header("Game Elements")]
    MoveMaster moveM;

    [Header("Slime Fields")]
    [SerializeField] int life = 30;
    [SerializeField] public int phase = 3;
    [SerializeField] float speed = 1;
    Vector2 slimePos;
    [SerializeField]
    float detectionRadius;
    bool canAttack = false;
    int damage = 3;
    float attackCooldown = 1;
    float IdleCounter;
    float IdleTime;
    Animator myAnim;

    //provisional
    float deadTime = 0.5f;
    float deadCounter;

    [SerializeField]float stunnedTime;
    float stunnedCounter;

    [SerializeField] Rigidbody2D rb;

    [Header("Player Fields")]
    Transform player;
    Vector2 playerPos;

    [Header("Slime Prefab")]
    [SerializeField] GameObject slimePreFab;

    [SerializeField]
    SlimeState currentSlimeState;
    enum SlimeState
    {
        Idle,
        Patrol,
        Attack,
        Stunned,
        Dividing,
        Dead
    }

    private void Start()
    {
        myAnim = GetComponentInChildren<Animator>();
        moveM = GameObject.FindGameObjectWithTag("MoveMaster").GetComponent<MoveMaster>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update ()
    {
        playerPos = player.transform.position;
        slimePos = this.transform.position;

        switch(currentSlimeState)
        {
            case SlimeState.Idle:
                Idle();
                break;

            case SlimeState.Patrol:
                Patrol();
                break;

            case SlimeState.Attack:
                Attack();
                break;

            case SlimeState.Stunned:
                Stunned();
                break;

            case SlimeState.Dividing:
                Dividing();
                break;

            case SlimeState.Dead:
                Dead();
                break;

            default:
                break;
        }
    }

    #region UPDATE METHODS

    void Idle()
    {
        IdleCounter += Time.deltaTime;

        if(IdleCounter >= IdleTime)
        {
            IdleCounter = 0;
            IdleTime = Random.Range(4, 8);
            PatrolState();
        }

        if(Vector2.Distance(playerPos,slimePos) < detectionRadius)
        {
            AttackState();
        }
    }

    void Patrol()
    {
        //Target nearby random Vector2 to move to it
        //When arrive, IdleState();

        if(Vector2.Distance(playerPos, slimePos) < detectionRadius)
        {
            AttackState();
        }
    }

    void Attack()
    {
        moveM.Move(this.gameObject, playerPos, speed);

        if(Vector2.Distance(playerPos, slimePos) > detectionRadius)
        {
            IdleState();
        }
    }

    void Stunned()
    {
        if (stunnedCounter < stunnedTime)
        {
            stunnedCounter += Time.deltaTime;

        }
        else
        {
            stunnedCounter = 0;
            IdleState();
        }
    }

    void Dividing()
    {
        IdleState();
        DivideAndDestroy(phase);
    }

    void Dead()
    {
        myAnim.SetTrigger("Dead");
        deadCounter -= Time.deltaTime;

        if (deadCounter <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    #endregion

    public void RecieveDamage(int damage)
    {
        life -= damage;


        if(life <= 0)
        {
            phase--;
            if(phase <= 0)
            {
                DeadState();
                deadCounter = deadTime;           
            }
            else
            {
                DividingState();
            }
        }
        else
        {
            StunnedState();

            Vector2 oppositePosition;
            oppositePosition.x = player.position.x * -1;
            oppositePosition.y = player.position.y * -1;

            rb.AddForce(oppositePosition,ForceMode2D.Impulse);
        }
    }

    private void DivideAndDestroy(int phase)
    {
        slimePreFab.GetComponent<Slime>().life = phase * 10;
        slimePreFab.GetComponent<Slime>().speed *= 2;
        slimePreFab.transform.localScale *= 0.8f;

        for(int i = 0; i < phase + 1;i++)Instantiate(slimePreFab);

  
        Debug.Log(this.gameObject.tag);
            
        Destroy(this.gameObject);
    }

    #region STATE METHODS

    void IdleState()
    {
        currentSlimeState = SlimeState.Idle;
    }

    void PatrolState()
    {
        currentSlimeState = SlimeState.Patrol;
    }

    void AttackState()
    {
        currentSlimeState = SlimeState.Attack;
    }

    void StunnedState()
    {
        currentSlimeState = SlimeState.Stunned;
    }

    void DividingState()
    {
        currentSlimeState = SlimeState.Dividing;
    }

    void DeadState()
    {
        currentSlimeState = SlimeState.Dead;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slimePos, detectionRadius);       
    }
}
