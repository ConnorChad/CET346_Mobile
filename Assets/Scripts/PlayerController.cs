using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    public Transform cam;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    private float kbHorizonal;
    private float kbVertical;
    public CinemachineFreeLook freeCam;

    float rotateMove = 0f;
    [Header("JoySticks")]
    public Joystick joystick;
    public Joystick rightJoystick;

    [Header("Movement")]
    public float speed;
    public float rotateSpeed;
    public float jumpHeight;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Combat")]
    public GameObject projectilePrefab;
    private GameObject projectileObj;
    public Transform shootPos;
    public LayerMask enemyLayer;
    public float viewDistance;
    public float viewAngle;
    private List<Collider> myList = new List<Collider>();
    private GameObject currentGO;
    private bool triggered = false;
    public float fireRate;
    private float ROF;
    [Header("Shake")]
    public float shakeDetectionThreshold;
    public float minShakeInterval;
   
    private float sqrShakeDetectionThreshhold;
    private float timeSinceLastShake;

    [Header("Tools")]
    public GameObject axe;
    public GameObject sword;
    public GameObject key;
    public GameObject staff;

    [Header("Misc")]
    public UIManager uiManager;
    public GameObject inventoryUI;
    public Transform throwPoint;
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public Animator animator;
    private bool canMove;
    public Transform teleporterEndPoint;
    public Transform teleporter2;
    private bool isDead = false;

    Collider other;

    
    private bool inInventory;
    public KeyHole keyHole;

    public enum ToolEqupped
    {
        none,
        axe,
        sword,
        key,
        staff,
    }

    public ToolEqupped toolEquipped;


    private void Start()
    {
        canMove = true;
        mana = maxMana;
        health = maxHealth;
        Physics.IgnoreLayerCollision(9, 11);
        inInventory = false;
        sqrShakeDetectionThreshhold = Mathf.Pow(shakeDetectionThreshold, 2);
        cc = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        HealthManager();
        if (!isDead)
        {
            freeCam.m_XAxis.Value += rightJoystick.Horizontal * (rotateSpeed * 2) * Time.deltaTime;
            freeCam.m_YAxis.Value += rightJoystick.Vertical * (rotateSpeed / 100) * Time.deltaTime;
            if (canMove)
            {
                Movement();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                animator.SetTrigger("magic");
            }

            if (triggered && !other)
            {
                uiManager.interactButton.interactable = false;
            }
        }
        

    }

    public void Movement()
    {
        
        freeCam.m_YAxisRecentering.m_enabled = false;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        horizontalMove = joystick.Horizontal + Input.GetAxisRaw("Horizontal");
        verticalMove = joystick.Vertical + Input.GetAxisRaw("Vertical");
        rotateMove = rightJoystick.Horizontal;
        Vector3 direction = new Vector3(horizontalMove, 0f, verticalMove).normalized;
        velocity.y += gravity * Time.deltaTime;

        if (horizontalMove == 0 && verticalMove == 0)
        {
            cc.enabled = false;
        }
        else
        {
            cc.enabled = true;
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //for smoothing
            //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //for smoothing

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDirection.normalized * speed * Time.deltaTime);
            freeCam.m_YAxisRecentering.m_enabled = true;
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }

        if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshhold && Time.unscaledTime >= timeSinceLastShake + minShakeInterval && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            timeSinceLastShake = Time.unscaledTime;
            animator.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(JumpDelay());
        }

        cc.Move(velocity * Time.deltaTime);

    }

    public void Fire()
    {
        if (toolEquipped == ToolEqupped.sword)
        {
            StartCoroutine(SwordAttack());
        }
        else if (toolEquipped == ToolEqupped.staff)
        {
            FireWithFOV();
        }
        
    }

    IEnumerator SwordAttack()
    {
        uiManager.attackButton.interactable = false;
        Debug.Log("Attack!");
        animator.SetTrigger("sword");
       // yield return new WaitForSeconds(0.1f);
        sword.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(2f);
        uiManager.attackButton.interactable = true;
        sword.GetComponent<Collider>().enabled = false;
    }

    IEnumerator JumpDelay()
    {
        animator.SetTrigger("jump");
        yield return new WaitForSeconds(0.3f);
        
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        timeSinceLastShake = Time.unscaledTime;
    }

    public void FireWithFOV()
    {
        if (Time.time > ROF)
        {
            ROF = Time.time + fireRate;
            if (mana >= 10)
            {
                myList.Clear();

                //Adds all enemis in view to an array
                Collider[] enemiesInView = Physics.OverlapSphere(transform.position, viewDistance, enemyLayer);

                Collider min = null;
                float minDist = Mathf.Infinity;
                for (int i = 0; i < enemiesInView.Length; i++)
                {
                    Transform target = enemiesInView[i].transform;
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                    {
                        myList.Add(enemiesInView[i]);
                    }

                }

                //Work out which enemy is closest to the player
                foreach (Collider hit in myList)
                {
                    float dist = Vector3.Distance(hit.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        min = hit;
                        minDist = dist;
                    }
                }

                //If an enemy is in view, fire the projectile
                if (min != null)
                {
                    shootPos.transform.LookAt(min.transform.position + new Vector3(0,1,0));
                    projectileObj = Instantiate(projectilePrefab, shootPos.position, Quaternion.identity);
                    projectileObj.transform.LookAt(min.transform.position + new Vector3(0, 1, 0));
                    projectileObj.GetComponent<Rigidbody>().AddRelativeForce(projectileObj.transform.forward * 2000, ForceMode.Acceleration);
                    mana -= 10;
                }
            }
        }
    }
    

    public void TakeDamage(float damage)
    {
        health -= damage;
        Handheld.Vibrate();
    }


    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void OpenCloseInventory()
    {
        inInventory = !inInventory;

        if (inInventory)
        {
            inventoryUI.SetActive(true);
        }
        else if (!inInventory)
        {
            inventoryUI.SetActive(false);
        }
    }

    public void Interact()
    {
        if (currentGO.GetComponent<DoorButton>())
        {
            currentGO.GetComponent<DoorButton>().OpenDoor();
        }
        if (currentGO.GetComponent<ItemPickup>())
        {
            currentGO.GetComponent<ItemPickup>().PickUp();
        }
        if (currentGO.GetComponent<Tree>() && toolEquipped == ToolEqupped.axe)
        {
            StartCoroutine(TreeChopping());
        }
        if (currentGO.GetComponent<KeyHole>() && toolEquipped == ToolEqupped.key)
        {
            UseKey();
        }
        if (currentGO.GetComponentInParent<MoveableRock>())
        {
            currentGO.GetComponentInParent<MoveableRock>().MoveRock();
            currentGO = null;
            uiManager.interactButton.interactable = false;
        }
        if (currentGO.GetComponent<MoveRockLever>())
        {
            currentGO.GetComponent<MoveRockLever>().OpenDoor();
        }

    }

    IEnumerator TreeChopping()
    {
        canMove = false;
        animator.SetBool("melee", true);
        yield return new WaitForSeconds(6f);
        Instantiate(currentGO.GetComponent<Tree>().woodToDrop, currentGO.GetComponent<Tree>().woodSpawnPoint.position, currentGO.GetComponent<Tree>().woodSpawnPoint.rotation);
        Destroy(currentGO.gameObject);
        animator.SetBool("melee", false);
        canMove = true;
    }

    public void UseKey()
    {
        keyHole = currentGO.GetComponent<KeyHole>();
        Instantiate(key, keyHole.keyPosition.transform.position, keyHole.keyPosition.transform.rotation);
        keyHole.OpenDoor();
    }
    public void HealthManager()
    {
        if (health <= 0)
        {
            isDead = true;
            uiManager.EnableDeathPanel();
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Interactable"))
        {
            uiManager.interactButton.interactable = true;
            currentGO = other.gameObject;
            triggered = true;
            this.other = other;

        }

        if (other.gameObject.CompareTag("Teleporter"))
        {
            cc.enabled = false;
            transform.position = teleporterEndPoint.position;
            transform.rotation = teleporterEndPoint.rotation;
            cc.enabled = true;
        }
        if (other.gameObject.CompareTag("Teleporter2"))
        {
            cc.enabled = false;
            transform.position = teleporter2.position;
            transform.rotation = teleporter2.rotation;
            cc.enabled = true;
        }
        if (other.gameObject.CompareTag("Teleporter3"))
        {
            FindObjectOfType<SceneController>().LoadMainMenu();
        }

        if (other.gameObject.CompareTag("KeyHole"))
        {
            keyHole = other.gameObject.GetComponent<KeyHole>();
        }


        if (other.gameObject.CompareTag("FakeRock"))
        {
            other.gameObject.GetComponent<FakeRock>().Fall();
        }

        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = other.gameObject.transform;
        }

        if (other.gameObject.CompareTag("Lava"))
        {
            cc.enabled = false;
            health -= 25;
            transform.position = other.gameObject.GetComponent<Lava>().checkPoint.transform.position;
            cc.enabled = true;
        }

        if (other.gameObject.CompareTag("EnemySword"))
        {
            health -= 10;
        }

        if (other.gameObject.CompareTag("SpawnTrigger"))
        {
            other.GetComponent<SpawnTrigger>().Spawn();
        }

        if (other.gameObject.CompareTag("Lever"))
        {
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            uiManager.interactButton.interactable = false;
        }

        if (other.gameObject.CompareTag("PressurePad"))
        {
            other.gameObject.GetComponentInParent<Animator>().SetBool("down", false);
        }
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (other.gameObject.CompareTag("KeyHole"))
        {
            keyHole = null;
        }
    }
}
