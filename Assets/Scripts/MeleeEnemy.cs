using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MeleeEnemy : MonoBehaviour
{

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    private bool isDead;

    [Header("Attacking")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    private float ROF;
    public LayerMask playerMask;

    [Header("VFX")]
    public GameObject deathSkull_VFX;
    public ParticleSystem hit_VFX;

    [Header("Drops")]
    public GameObject healthPotionDrop;
    public GameObject manaPotionDrop;


    public PlayerController player;
    public Animator animator;
    public NavMeshAgent agent;

    private float distanceFromPlayer;
    private bool canAttack;


    private void Awake()
    {
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        agent.SetDestination(player.transform.position);

        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer <= 1.5f)
        {
            animator.SetBool("attack", true);
            if (Time.time > ROF)
            {
                canAttack = true;
                ROF = Time.time + attackSpeed;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, playerMask))
                {
                    player.TakeDamage(10f);
                    canAttack = false;
                }
            }
        }
        else
        {
            animator.SetBool("attack", false);
        }

        HealthManager();
    }

    public void HealthManager()
    {
        if (currentHealth <= 0)
        {
            Instantiate(healthPotionDrop, transform.position, transform.rotation);
            Instantiate(deathSkull_VFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        hit_VFX.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            TakeDamage(50f);
            player.sword.GetComponent<Collider>().enabled = false;
        }
    }
}
