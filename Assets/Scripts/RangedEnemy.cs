using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    public enum RangedEnemyState
    {
        Idle,
        Chase,
        Attack,
        Patrol,
        Dead,
    }

    public RangedEnemyState state;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    private bool isDead;

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
    public float distanceFromPlayer;

    [Header("Weapons")]
    public Transform projectileShootPos;
    public GameObject projectilePrefab;
    private GameObject projectile;

    [Header("VFX")]
    public GameObject deathSkull_VFX;
    public ParticleSystem hit_VFX;

    [Header("Patrol")]
    public Transform[] points;
    private int destPoint = 0;

    [Header("Drops")]
    public GameObject[] itemsToDrop;
    public GameObject healthPotionDrop;
    public GameObject manaPotionDrop;

    [Header("Misc")]
    public Animator animator;
    private NavMeshAgent agent;
    public LayerMask playerMask;
    public LayerMask wallMask;
    public LayerMask enemyLayer;
    private PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        if (points.Length == 0)
        {
            state = RangedEnemyState.Idle;
        }
        else if (points.Length > 0)
        {
            state = RangedEnemyState.Patrol;
            agent.autoBraking = false;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            EnemyStates();
            DistanceCheck();
            HealthManager();
            PlayerDetection();
        }
    }

    public void EnemyStates()
    {
        if (!isDead)
        {
            switch (state)
            {
                case RangedEnemyState.Idle:
                    Idle();
                    break;
                case RangedEnemyState.Chase:
                    Chase();
                    break;
                case RangedEnemyState.Attack:
                    Attack();
                    break;
                case RangedEnemyState.Patrol:
                    Patrol();
                    break;
                case RangedEnemyState.Dead:
                    Dead();
                    break;
            }
        }
    }

    private void Idle()
    {
        animator.SetBool("idle", true);
    }

    private void Chase()
    {
        animator.SetBool("idle", false);
        animator.SetBool("walk", true);
        agent.isStopped = false;

        agent.SetDestination(player.transform.position);
    }

    private void Patrol()
    {
        
        animator.SetBool("idle", false);
        animator.SetBool("walk", true);
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
    }

    private void Attack()
    {
        
        agent.isStopped = true;

        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        if (Time.time > ROF)
        {
            ROF = Time.time + rateOfFire;
            projectileShootPos.transform.LookAt(player.transform.position);

            projectile = Instantiate(projectilePrefab, projectileShootPos.position, Quaternion.identity);
            projectile.transform.LookAt(player.transform.position);
            projectile.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.forward * 300f, ForceMode.Acceleration);

        }
    }

    private void Dead()
    {
        Instantiate(itemsToDrop[Random.Range(0, 2)], transform.position, transform.rotation);
        Instantiate(deathSkull_VFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    public void HealthManager()
    {
        if (currentHealth <= 0)
        {
            state = RangedEnemyState.Dead;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        hit_VFX.Play();
        playerDetected = true;
        AlertNearbyEnemies();
    }

    private void PlayerDetection()
    {
        if (!playerDetected && state != RangedEnemyState.Dead)
        {
            Collider[] playerInView = Physics.OverlapSphere(transform.position, viewRange, playerMask);

            for (int i = 0; i < playerInView.Length; i++)
            {
                Transform target = playerInView[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, wallMask))
                    {
                        playerDetected = true;
                    }
                }
            }
        }
    }

    private void DistanceCheck()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //Player too close -> move backwards
        if (distanceFromPlayer <= 3f && playerDetected)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walk", false);
            animator.SetBool("walkback", true);
            agent.ResetPath();
            agent.Move(transform.forward * -2 * Time.deltaTime);
        }
        //Player within attack range -> attack
        else if (distanceFromPlayer <= 8 && playerDetected)
        {
            animator.SetBool("idle", true);
            animator.SetBool("walk", false);
            animator.SetBool("walkback", false);
            state = RangedEnemyState.Attack;
        }
        //Is a patrol and the player is too far away -> return back to patrol
        else if (points.Length > 0 && distanceFromPlayer >= 14f && playerDetected)
        {
            agent.ResetPath();
            state = RangedEnemyState.Patrol;
            playerDetected = false;
        }
        //Not a patrol and player is out of attack range -> chase player forever
        else if (distanceFromPlayer >= 8f && playerDetected)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walk", true);
            animator.SetBool("walkback", false);
            state = RangedEnemyState.Chase;
        }
    }

    public void AlertNearbyEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 5f, enemyLayer);
        int i = 0;

        while (i < enemies.Length)
        {
            enemies[i].gameObject.GetComponent<RangedEnemy>().playerDetected = true;
            i++;
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            TakeDamage(50f);
            player.sword.GetComponent<Collider>().enabled = false;
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
