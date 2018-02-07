﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game Elements")]
    InputMaster inputM;
    CollisionMaster collisionM;
    AudioMaster audioM;
    public CameraBehaviour cameraBeh;

    [Header("Player Fields")]
    bool godMode;
    [SerializeField]
    int life = 100;
    [SerializeField]
    int essences = 100;
    [SerializeField]
    int maxLife = 100;
    [SerializeField]
    GameObject lifeParticles;
    [SerializeField]
    GameObject bloodParticles;
    ParticleSystem blood;
    bool isDead;
    Vector2 movementSpeed = Vector2.zero;
    Vector2 speed = new Vector2(5, 5);
    float deadCounter = 0;

    float attackCooldown = 1;
    float attackCounter;
    int damage = 10;
    float arrowCooldown = 1.5f;
    float arrowCounter;
    Animator myAnim;
    public GameObject arrowGameObject;

    public Vector2 attackBoxPos;
    public Vector2 attackBoxSize;
    public ContactFilter2D filter;

    PlayerState currentPlayerState;
    enum PlayerState
    {
        Idle,
        Move,
        Dead,
    }

    Skill currentskill1;
    Skill currentskill2;
    enum Skill
    {
        ChargedStrike,
    }

    void Start()
    {
        inputM = GameObject.FindGameObjectWithTag("InputMaster").GetComponent<InputMaster>();

        //audioM = GameObject.FindGameObjectWithTag("SoundMaster").GetComponent<AudioMaster>();

        blood = bloodParticles.GetComponent<ParticleSystem>();
        collisionM = GetComponent<CollisionMaster>();
        myAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                Idle();
                break;

            case PlayerState.Move:
                Move();
                break;

            case PlayerState.Dead:
                Dead();
                break;

            default:
                break;
        }

        if(attackCooldown > 0)
        {
            attackCounter -= Time.deltaTime;
        }

        if(arrowCounter > 0)
        {
            arrowCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;
        }

        if(godMode)
        {
            speed = new Vector2(8, 8);
            attackCooldown = 0;
            arrowCooldown = 0;
            damage = 50;
            this.gameObject.layer = 1;
        }
        else
        {
            speed = new Vector2(5, 5);
            attackCooldown = 1;
            arrowCooldown = 1.5f;
            damage = 10;
            this.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    #region UPDATE METHODS

    void Idle()
    {
        if (inputM.GetAxis().x > 0.1 || inputM.GetAxis().y > 0.1 || inputM.GetAxis().x < -0.1 || inputM.GetAxis().y < -0.1) //PLAYER MOVES
        {
            myAnim.SetBool("Moving", true);
            MoveState();
        }
    }

    void Move()
    {
        Vector3 provisionalPos;

        provisionalPos = this.transform.position;
        movementSpeed = speed;

        if (inputM.GetAxis().x < 0.1 && inputM.GetAxis().x > -0.1 && inputM.GetAxis().y < 0.1 && inputM.GetAxis().y > -0.1) //PLAYER STOP
        {
            myAnim.SetBool("Moving", false);
            IdleState();
        }

        if (collisionM.IsLeftWalled && inputM.GetAxis().x < 0) //LEFT WALL
        {
            movementSpeed.x = 0;
            myAnim.SetBool("Moving", false);
        }

        if (collisionM.IsRightWalled && inputM.GetAxis().x > 0) //RIGHT WALL
        {
            movementSpeed.x = 0;
            myAnim.SetBool("Moving", false);
        }

        if (collisionM.IsTopWalled && inputM.GetAxis().y > 0) //TOP WALL
        {
            movementSpeed.y = 0;
            myAnim.SetBool("Moving", false);
        }

        if (collisionM.IsBottomWalled && inputM.GetAxis().y < 0) //BOTTOM WALL
        {
            movementSpeed.y = 0;
            myAnim.SetBool("Moving", false);
        }

        if (inputM.GetAxis().x > 0.1) //FLIP RIGHT
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            attackBoxPos.x = 0.75f;
        }

        if (inputM.GetAxis().x < -0.1) //FLIP LEFT
        {
            this.gameObject.transform.localScale = new Vector3(-1, 1, 1);
            attackBoxPos.x = -0.75f;
        }

        provisionalPos.x += inputM.GetAxis().x * Time.deltaTime * movementSpeed.x;
        provisionalPos.y += inputM.GetAxis().y * Time.deltaTime * movementSpeed.y;
        provisionalPos.z += inputM.GetAxis().y * Time.deltaTime * movementSpeed.y;
        this.transform.position = provisionalPos;
    }

    void Dead()
    {
        deadCounter += Time.deltaTime;

        if(deadCounter >= 2)
        {
            Debug.Log("GAME OVER");
        }
    }

    #endregion

    #region STATE METHODS

    public void IdleState()
    {
        currentPlayerState = PlayerState.Idle;
    }

    public void MoveState()
    {
        currentPlayerState = PlayerState.Move;
    }

    public void DeadState()
    {
        currentPlayerState = PlayerState.Dead;
    }

    #endregion

    #region MECHANICS METHODS

    public void Attack() //ATTACK IF PLAYER IS IDLE OR MOVING
    {
        if (attackCounter <= 0 && currentPlayerState == PlayerState.Idle || attackCounter <= 0 && currentPlayerState == PlayerState.Move || attackCounter <= 0)
        {
            attackCounter = attackCooldown;

            myAnim.SetTrigger("Attack");

            //audioM.SwordSound();

            Vector3 pos = this.transform.position + (Vector3)attackBoxPos;
            Collider2D[] results = new Collider2D[5];

            int numColliders = Physics2D.OverlapBox(pos, attackBoxSize, 0, filter, results);

            for (int i = 0; i < numColliders; i++)
            {
                if (results[i].gameObject.tag == "Slime")
                {
                    Damage(results[i].gameObject, "Slime");
                }
                if (results[i].gameObject.tag == "SpitterSlime")
                {
                    Damage(results[i].gameObject, "SpitterSlime");
                }
                if (results[i].gameObject.tag == "PassiveSlime")
                {
                    Damage(results[i].gameObject, "PassiveSlime");
                }
                if (results[i].gameObject.tag == "Destructible")
                {
                    Damage(results[i].gameObject, "Destructible");
                }
                if (results[i].gameObject.tag == "BossSlime")
                {
                    Damage(results[i].gameObject, "BossSlime");
                }
                if (results[i].gameObject.tag == "BossSpite")
                {
                    Damage(results[i].gameObject, "BossSpite");
                }
                if (results[i].gameObject.tag == "MiniBossSlime")
                {
                    Damage(results[i].gameObject, "MiniBossSlime");
                }
            }
        }
    }

    void Damage (GameObject target, string targetType) //OBJECTIVE RECIEVES THE HIT
    {
        if(targetType == ("Slime"))
        {
            target.GetComponent<Slime>().RecieveDamage(damage);
        }
        if (targetType == ("SpitterSlime"))
        {
            target.GetComponent<SpitterSlime>().RecieveDamage(damage);
        }
        if (targetType == ("PassiveSlime"))
        {
            target.GetComponent<PassiveSlime>().RecieveDamage(damage);
        }
        if (targetType == ("Destructible"))
        {
            target.GetComponent<DestructibleBehaviour>().GetDestroyed();
        }
        if (targetType == ("BossSlime"))
        {
            target.GetComponent<BossSlime>().RecieveDamage(damage);
        }
        if (targetType == ("BossSpite"))
        {
            target.GetComponent<BossSpite>().Hit();
        }
        if (targetType == ("MiniBossSlime"))
        {
            target.GetComponent<MiniBossSlime>().RecieveDamage(damage);
        }
    }
    
    public void RecieveDamage(int damage)
    {
        if(!godMode)
        {
            life -= damage;
            blood.Emit(30);
            //audioM.PlayerDamageSound();
        }

        if (life <= 0)
        {
            isDead = true;
            DeadState();
        }
    }

    public void RecieveHP(int HP)
    {
        life += HP;

        if (life >= maxLife)
        {
            life = maxLife;
        }
    }

    public void ReceiveEssences(int numberOfEssences)
    {
        essences += numberOfEssences;
    }

    public void StartLifeParticles()
    {
        lifeParticles.SetActive(true);
    }

    public void StopLifeParticles()
    {
        lifeParticles.SetActive(false);
    }

    public void Fall()
    {
        this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 5;
        DeadState();
    }

    #endregion

    #region GETTERS/SETTERS

    public int Life { get { return life; } }
    public bool IsDead { get { return isDead; } }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Vector3 pos = this.transform.position + (Vector3)attackBoxPos;
        Gizmos.DrawWireCube(pos, attackBoxSize);
    }
}
