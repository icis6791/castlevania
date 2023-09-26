using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;

public class PlayerCtrl : MonoBehaviour
{
    public enum PlayerSkill
    {
        dash, doubleJump
    }

    CinemachineVirtualCamera[] virtualCams;
    public Transform groundChk;  //바닥확인 위치
    public Image hpBar;
    Rigidbody2D rb;
    GameObject gameOver;
    Transform gameOverTr;
    Animator anim;
    public Attack attack;
    SpriteRenderer sprite;
    AudioSource audioSource;
    StageUIManager stageUIManager;
    public AudioClip damageClip, dieClip, attackClip, jumpClip;
    public Zephyr zephyr;
    public GameObject saveMsg;  //저장 메시지
    public GameObject healthMsg;  //최대체력 증가 메시지
    public GameObject forceMsg;  //공격력 증가 메시지
    public GameObject jumpMsg;  
    public GameObject menu;

    public float speed;  //이동속도
    public bool onGround;  //바닥확인 참거짓 판단
    public bool onPlatform;//발판확인 참거짓 판단
    public float jumpForce = 720f;    //점프힘
    public float fireRate; //공격 쿨타임
    public float currHp; //현재 체력.

    //바닥레이어, 플레이어레이어, 발판 레이어.
    int groundLayer;
    int playerLayer;
    int platformLayer;
    bool facingRight = true;  //오른쪽 왼쪽 확인용. 기본값 오른쪽 참.
    bool isJump = false;   //점프중
    bool isDoubleJump = false;   //2단점프중
    public bool doubleJumpSkill = false;
    bool isPause;
    bool canDamage = true;
    [SerializeField]
    bool isAttack = false;
    bool isDie = false;
    float nextAttack;
    public float iniHp = 120f;  //체력.
    float t;
    bool canSave = false;
    bool isMenuOpen = false;

    public bool bossComA;
    public bool bossComB;
    public bool healthGain;
    public bool forceGain;

    private void Awake()
    {
        virtualCams = GameObject.FindWithTag("CAMS").GetComponentsInChildren<CinemachineVirtualCamera>(true);
        for(int i = 0; i < virtualCams.Length; i++)
        {
            virtualCams[i].Follow = this.transform;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        attack = GetComponentInChildren<Attack>();
        sprite = GetComponent<SpriteRenderer>();
        hpBar = GameObject.FindWithTag("HPBAR").GetComponent<Image>();
        gameOver = GameObject.FindWithTag("GAMEOVER");
        gameOverTr = gameOver.transform.GetChild(0);
        stageUIManager = GameObject.FindWithTag("MANAGER").GetComponent<StageUIManager>();
        //각 레이어 마스크 할당
        groundLayer = LayerMask.NameToLayer("GROUND");
        playerLayer = LayerMask.NameToLayer("PLAYER");
        platformLayer = LayerMask.NameToLayer("PLATFORM");

        zephyr = GetComponent<Zephyr>();

        currHp = iniHp;

        //저장된 데이터가 있으면 값을 가져오고 없으면 기본값으로.
        if (File.Exists(DataManager.instance.path + $"{DataManager.instance.nowSlot}"))
        {
            transform.position = DataManager.instance.nowPlayer.playerTr;
            bossComA = DataManager.instance.nowPlayer.bossComa;
            bossComB = DataManager.instance.nowPlayer.bossComb;
            healthGain = DataManager.instance.nowPlayer.healthGain;
            forceGain = DataManager.instance.nowPlayer.forceGain;
            iniHp = DataManager.instance.nowPlayer.iniHp;
            doubleJumpSkill = DataManager.instance.nowPlayer.doubleJumpSkill;
            attack.damage = DataManager.instance.nowPlayer.damage;
        }
        else
        {
            transform.position = new Vector3(-7.44f, -2.21f, 0);
            bossComA = false;
            bossComB = false;
            healthGain = false;
            forceGain = false;
            attack.damage = 20;
            iniHp = 120;
            doubleJumpSkill = false;
        }
    }

    void Update()
    {
        t -= Time.unscaledDeltaTime * 0.28f;

        if (Time.timeScale == 0)
        {
            isPause = true;
        }
        else
            isPause = false;

        //플레이어와 발끝 사이에 라인을 그려서 레이어 확인.
        onGround = Physics2D.Linecast(transform.position, groundChk.position, 1 << groundLayer);
        onPlatform = Physics2D.Linecast(transform.position, groundChk.position, 1 << platformLayer);

        //바닥에 있을때 
        if (onGround || onPlatform)
        {
            isJump = false;
            isDoubleJump = false;
        }

        Jump();
        StartCoroutine(Attack());

        if (!isAttack)
        {
            anim.SetBool("OnGround", onGround || onPlatform);
            anim.SetFloat("Fall", rb.velocity.y);
        }

        Save();

        //현재 메뉴창이 열려있는지로 판단.
        if (!stageUIManager.isMenuOpen)
        {
            stageUIManager.OpenMenu();
        }
        else if (stageUIManager.isMenuOpen)
        {
            stageUIManager.CloseMenu();
        }
    }

    void FixedUpdate()
    {
        Move();

        //가만히 있을 때는 위치 고정. 내리막 오르막에서 미끄러지지 않기 위함.
        if (Input.GetAxisRaw("Horizontal") == 0 && canDamage)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Move()
    {
        //좌우 이동 입력
        float h = Input.GetAxisRaw("Horizontal");

        //데미지 입을 수 있는상태 + 공격중이 아닐때
        if (canDamage && !isAttack && !isDie)
        {
            rb.velocity = new Vector2(h * speed, rb.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(h));
        }
        //데미지 입을 수 있는상태 + 공중일때
        if (canDamage && !onGround && !onPlatform && !isDie)
        {
            rb.velocity = new Vector2(h * speed, rb.velocity.y);
        }

        if (h > 0 && !facingRight && !isAttack)
        {
            Flip();
        }
        else if (h < 0 && facingRight && !isAttack)
        {
            Flip();
        }
    }

    IEnumerator Attack()
    {
        //z키를 입력받고,  time 이 nextAttack보다 크면
        if (Input.GetKeyDown(KeyCode.X) && Time.time > nextAttack && (onGround || onPlatform) && !isDie)
        {
            audioSource.Stop();
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(attackClip);
            }
            //dash = false;
            //해쉬값으로 애니메이션 트리거-->플레이어 공격 애니메이션 재생. 
            isAttack = true;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("Attack");
            //플레이어 공격애니메이션이 재생하면 연계해서 무기 애니메이션도 재생되야함.
            //플레이어 자식 오브젝트인 Attack에서 현재 weapon의 애니메이션 재생
            attack.PlayAnimation();
            //nextAttack에는 현재시간 + fireRate. fireRate는 공격애니메이션 재생시간임. 0.833초
            //한번 공격하면 0.611초가 지나기 전에는 또 공격을 못한다 이 말.
            nextAttack = Time.time + fireRate;
            yield return new WaitForSeconds(0.611f);
            isAttack = false;
        }
        else if (Input.GetKeyDown(KeyCode.X) && Time.time > nextAttack && !onGround && !onPlatform && !isDie)
        {
            audioSource.Stop();
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(attackClip);
            }
            isAttack = true;
            anim.SetTrigger("JumpAttack");
            attack.PlayAnimation();
            nextAttack = Time.time + fireRate;
            yield return new WaitForSeconds(0.35f);
            isAttack = false;
        }
    }

    void Jump() //점프 메서드
    {
        //바닥 점프
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow) && !isDie)
        {
            StartCoroutine(JumpOff());
        }
        //보통 점프
        else if (Input.GetKeyDown(KeyCode.Z) && ((onGround || onPlatform) || (!isDoubleJump && doubleJumpSkill)) && !isDie)
        {
            isJump = true;

            //2단 점프중이 아니고 바닥도 아니라면
            if (!isDoubleJump && doubleJumpSkill && !onGround && !onPlatform)
            {
                //2단점프를 true로
                isDoubleJump = true;
            }
        }

        if (isJump && !isPause && !isDie)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(jumpClip);
            }
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce);
            anim.SetTrigger("Jump");
            isJump = false;
        }
    }

    void Flip()  //좌우 방향 전환 메서드
    {
        if(!isDie)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator JumpOff() //플랫폼 아래로 점프 코루틴
    {
        //플레이어 레이어와 발판 레이어 사이의 충돌을 잠시 무시.
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
    }

    public void TakeDamage(int damage)
    {
        if (canDamage)
        {
            canDamage = false;
            currHp -= damage;
            float currHpPer = currHp / iniHp;
            stageUIManager.SetHUD(currHpPer);
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(damageClip);
            }

            if (currHp <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                //데미지 애니메이션 + 깜빡깜빡 효과 + 뒤로 밀려나기.
                if (facingRight)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(Vector2.left * 5, ForceMode2D.Impulse);
                    anim.SetTrigger("Damage");
                    StartCoroutine(DamageEff());
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(Vector2.right * 5, ForceMode2D.Impulse);
                    anim.SetTrigger("Damage");
                    StartCoroutine(DamageEff());
                }
            }
        }
    }

    IEnumerator DamageEff()
    {
        for (float i = 0; i < 0.6f; i += 0.2f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        canDamage = true;
    }

    IEnumerator Die()
    {
        isDie = true;
        audioSource.Stop();
        if(!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(dieClip);
        }
        rb.velocity = Vector2.zero;
        anim.StopPlayback();
        anim.SetTrigger("Die");

        t = 1;
        while(true)
        {
            Time.timeScale = t;

            if (t<=0)
            {
                t = 0;
                break;
            }
            yield return null;
        }
        //GameManager.instance.isGameOver = true;
        //사망화면 띄우고 1.5초 뒤 타이틀화면으로
        gameOverTr.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    void Save()  //저장 메서드
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && canSave && !isDie)
        {
            DataManager.instance.nowPlayer.playerTr = transform.position;
            DataManager.instance.nowPlayer.bossComa = bossComA;
            DataManager.instance.nowPlayer.bossComb = bossComB;
            DataManager.instance.nowPlayer.iniHp = iniHp;
            DataManager.instance.nowPlayer.damage = attack.damage;
            DataManager.instance.nowPlayer.healthGain = healthGain;
            DataManager.instance.nowPlayer.forceGain = forceGain;
            DataManager.instance.nowPlayer.doubleJumpSkill = doubleJumpSkill;
            DataManager.instance.SaveData();  //저장할 값들을 각 변수에 저장.
            currHp = iniHp;  //체력 만땅으로
            float currHpPer = currHp / iniHp;
            stageUIManager.SetHUD(currHpPer);  //HUD최신화

            //저장 메세지
            stageUIManager.ShowSave();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //세이브 포인트에서 저장가능
        if(collision.CompareTag("SAVE"))
        {
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //세이브 포인트 벗어나면 저장 불가능.
        if(collision.CompareTag("SAVE"))
        {
            canSave = false;
        }
    }
}
