using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeroKnight : MonoBehaviour {
    
    //플레이어 공격 데미지 추가
    [SerializeField] private Transform attackPoint;             // 공격 위치
    [SerializeField] private float attackRange = 0.5f;          // 공격 범위
    [SerializeField] private LayerMask enemyLayers;             // 공격 대상이 될 레이어
    [SerializeField] private int attackDamage = 1;              // 데미지 양

    [SerializeField] private GameObject hitboxObject;

    [SerializeField] private float speedBoostAmount = 5.0f;
    [SerializeField] private float speedBoostDuration = 1000.0f;

    private float origianlJumpForce;
    private Coroutine jumpBoostCoroutine;

    public void ApplyJumpBoost(float boostAmount, float duration)
    {
        if (jumpBoostCoroutine != null)
            StopCoroutine(jumpBoostCoroutine);

        jumpBoostCoroutine = StartCoroutine(JumpBoostRoutine(boostAmount, duration));
    }

    private IEnumerator JumpBoostRoutine(float boostAmount, float duration)
    {
        origianlJumpForce = m_jumpForce;
        m_jumpForce += boostAmount;

        yield return new WaitForSeconds(duration);

        m_jumpForce = origianlJumpForce;
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        float originalSpeed = m_speed;
        m_speed += speedBoostAmount;

        yield return new WaitForSeconds(speedBoostDuration);

        m_speed = originalSpeed;
    }

    void EnableHitbox()  // 애니메이션 이벤트로 호출
    {
        hitboxObject.SetActive(true);
    }

    void DisableHitbox()  // 애니메이션 이벤트로 호출
    {
        hitboxObject.SetActive(false);
    }
    private float hitboxOffsetX;

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [SerializeField] private PlatformEffector2D m_platformeEffector;
    [SerializeField] private float m_dropwaitTimeValue = 2f;
    private float m_dropwaitTime;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime = 0f;

    //구르기 수정
    private float m_lastRollTime = -Mathf.Infinity; //초기값
    private float m_rollCooldown = 1.5f; //쿨타임
    //구르기가 실행되는 동안 무적 추가
    private bool m_isInvincible = false;
    //무적 아이템 추가
    public float invincibleTimer = 0f;
    public bool IsInvincible
    {
        get {  return m_isInvincible; }
    }
    public void SetInvincible(bool value)
    {
        m_isInvincible = value;
    }

    //리스폰 UI 추가
    public GameObject gameOver;
    private bool isGameOver = false;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        if (hitboxObject == null)
            hitboxObject = transform.Find("Hitbox")?.gameObject;

        m_dropwaitTime = m_dropwaitTimeValue;

        hitboxOffsetX = hitboxObject.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update ()
    {
        if (isGameOver)
            return;
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;

            //구르기 무적 해제 추가
            if (invincibleTimer <= 0f)
                m_isInvincible = false;
        }

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // S를 눌렀을 때 플랫폼 이팩터를 플레이어가 관통하여 아래로 이동 할 수 있게 변경  
        if (Input.GetKey(KeyCode.S) && m_grounded)
        {
            if (m_dropwaitTime <= 0)
            {
                m_platformeEffector.rotationalOffset = 180f;
                m_dropwaitTime = m_dropwaitTimeValue;

                //m_groundSensor.Disable(0.3f);
            }
            else
            {
                m_dropwaitTime -= Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            m_platformeEffector.rotationalOffset = 0f;
        }


        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;

            Vector3 hitboxPos = hitboxObject.transform.localPosition;
            hitboxPos.x = Mathf.Abs(hitboxPos.x);
            hitboxObject.transform.localPosition = hitboxPos;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;

            Vector3 hitboxPos = hitboxObject.transform.localPosition;
            hitboxPos.x = -Mathf.Abs(hitboxPos.x);
            hitboxObject.transform.localPosition = hitboxPos;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll 수정 전
        //else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        //{
            //m_rolling = true;
            //m_animator.SetTrigger("Roll");
            //m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        //}


        // Roll 수정 후 실행 후 한번 만 굴러지던 오류 수정 및 구르기 쿨타임 추가
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding && Time.time >= m_lastRollTime + m_rollCooldown)
        {
            m_rolling = true;
            m_lastRollTime = Time.time;
            m_rollCurrentTime = 0f;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            //구르기 무적
            m_isInvincible = true;
        }

        //무적 아이템 지속시간 감소
        if (m_isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                m_isInvincible = false;
            }
        }


        //Jump
        if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }

        // R키를 눌렀을 때 씬 리로드
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //게임 오버시 조작 불가
        if (isGameOver) return;
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    //리스폰 설정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Trap"))
        {
            if (m_isInvincible) return;

            TriggerGameOver();
        }

        if (collision.CompareTag("SpeedUpItem"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(SpeedBoostCoroutine());
        }

        if (collision.CompareTag("Enemy"))
        {
            if (m_isInvincible) return;

            TriggerGameOver();
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveNextLevel();
        }
    }
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        {
            isGameOver = true;
            gameOver.SetActive(true);

            m_body2d.velocity = Vector2.zero;
            m_body2d.isKinematic = true;

            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
    }
    public void Respawn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    //데미지 함수
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }
}
