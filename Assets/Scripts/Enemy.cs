using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public bool isMelee;
    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Dead,
        RangedAttack,
        MoveCloser,
        Patrol,
    }

    public EnemyState state;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;

    [Header("Attack Stats")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float rateOfFire;
    private float ROF;

    [Header("Vision")]
    public bool playerDetected;
    public float viewRange;
    public float viewAngle;
    private float distanceFromPlayer;

    [Header("Weapons")]
    public GameObject sword;
    public GameObject staff;
    public GameObject staffAttackOrbPrefab;
    public Transform staffShootPos;
    private GameObject staffAttackOrb;

    [Header("Drops")]
    public GameObject healthPotionDrop;
    public GameObject manaPotionDrop;

    [Header("VFX")]
    public GameObject deathSkull_VFX;
    public ParticleSystem hit_VFX;

    [Header("Patrol")]
    public Transform[] points;
    private int destPoint = 0;

    [Header("Misc")]
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject player;
    public bool isDead;
    public LayerMask playerMask;
    public LayerMask wallMask;
    private PlayerController playerController;

    

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        //state = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (state == EnemyState.Patrol)
        {
            agent.autoBraking = false;
        }
    }

    private void Update()
    {
        if (isMelee)
        {
            sword.gameObject.SetActive(true);
            EnemyAILogicMelee();
            DistanceCheckMelee();
        }
        else
        {
            staff.gameObject.SetActive(true);
            EnemyAILogicRanged();
            DistanceCheckRanged();
        }
        HealthManager();
        PlayerDetection();
        


    }

    public void EnemyAILogicMelee()
    {
        if (!isDead)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Chasing:
                    Chasing();
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Dead:
                    Dead();
                    break;
            }
        }
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    public void EnemyAILogicRanged()
    {
        if (!isDead)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.RangedAttack:
                    RangedAttack();
                    break;
                case EnemyState.Chasing:
                    Chasing();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Dead:
                    Dead();
                    break;


            }
        }
    }


    public void Idle()
    {

    }

    public void Chasing()
    {
        agent.isStopped = false;
        animator.SetBool("attacking", false);
        agent.SetDestination(player.transform.position);
    }

    public void MoveCloser()
    {

    }

    public void Attack()
    {
        animator.SetBool("attacking", true);
    }

    public void RangedAttack()
    {
        agent.isStopped = true;
        transform.LookAt(player.transform);
        if (Time.time > ROF)
        {
            ROF = Time.time + rateOfFire;
            staffShootPos.transform.LookAt(player.transform.position);

            staffAttackOrb = Instantiate(staffAttackOrbPrefab, staffShootPos.position, Quaternion.identity);
            staffAttackOrb.transform.LookAt(player.transform.position);
            staffAttackOrb.GetComponent<Rigidbody>().AddRelativeForce(staffAttackOrb.transform.forward * 300f, ForceMode.Acceleration);

        }
        
    }

    public void HealthManager()
    {
        if (currentHealth <= 0)
        {
            state = EnemyState.Dead;
        }
    }
    public void Dead()
    {
        
        Instantiate(healthPotionDrop, transform.position, transform.rotation);
        Instantiate(deathSkull_VFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        hit_VFX.Play();
    }


    private void PlayerDetection()
    {
        if (!playerDetected && state != EnemyState.Dead)
        {
            Collider[] playerInView = Physics.OverlapSphere(transform.position, viewRange, playerMask);

            for (int i = 0; i < playerInView.Length; i++)
            {
                Transform target = playerInView[i].transform;
                player = target.gameObject;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if(!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, wallMask))
                    {
                        playerDetected = true;
                    }
                }
            } 
        }
    }

    public void DistanceCheckMelee()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer <= 1.5f && playerDetected)
        {
            state = EnemyState.Attacking;
        }
        else if (distanceFromPlayer >= 1.5f && playerDetected)
        {
            state = EnemyState.Chasing;
        }
    }

    public void DistanceCheckRanged()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceFromPlayer <= 8 && playerDetected)
        {
            state = EnemyState.RangedAttack;
        }
        else if (distanceFromPlayer >= 8f && playerDetected)
        {
            state = EnemyState.Chasing;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            TakeDamage(25f);
            playerController.sword.GetComponent<Collider>().enabled = false;
        }
    }

    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
