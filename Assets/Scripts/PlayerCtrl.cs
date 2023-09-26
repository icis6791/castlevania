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
    public Transform groundChk;  //�ٴ�Ȯ�� ��ġ
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
    public GameObject saveMsg;  //���� �޽���
    public GameObject healthMsg;  //�ִ�ü�� ���� �޽���
    public GameObject forceMsg;  //���ݷ� ���� �޽���
    public GameObject jumpMsg;  
    public GameObject menu;

    public float speed;  //�̵��ӵ�
    public bool onGround;  //�ٴ�Ȯ�� ������ �Ǵ�
    public bool onPlatform;//����Ȯ�� ������ �Ǵ�
    public float jumpForce = 720f;    //������
    public float fireRate; //���� ��Ÿ��
    public float currHp; //���� ü��.

    //�ٴڷ��̾�, �÷��̾�̾�, ���� ���̾�.
    int groundLayer;
    int playerLayer;
    int platformLayer;
    bool facingRight = true;  //������ ���� Ȯ�ο�. �⺻�� ������ ��.
    bool isJump = false;   //������
    bool isDoubleJump = false;   //2��������
    public bool doubleJumpSkill = false;
    bool isPause;
    bool canDamage = true;
    [SerializeField]
    bool isAttack = false;
    bool isDie = false;
    float nextAttack;
    public float iniHp = 120f;  //ü��.
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
        //�� ���̾� ����ũ �Ҵ�
        groundLayer = LayerMask.NameToLayer("GROUND");
        playerLayer = LayerMask.NameToLayer("PLAYER");
        platformLayer = LayerMask.NameToLayer("PLATFORM");

        zephyr = GetComponent<Zephyr>();

        currHp = iniHp;

        //����� �����Ͱ� ������ ���� �������� ������ �⺻������.
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

        //�÷��̾�� �߳� ���̿� ������ �׷��� ���̾� Ȯ��.
        onGround = Physics2D.Linecast(transform.position, groundChk.position, 1 << groundLayer);
        onPlatform = Physics2D.Linecast(transform.position, groundChk.position, 1 << platformLayer);

        //�ٴڿ� ������ 
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

        //���� �޴�â�� �����ִ����� �Ǵ�.
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

        //������ ���� ���� ��ġ ����. ������ ���������� �̲������� �ʱ� ����.
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
        //�¿� �̵� �Է�
        float h = Input.GetAxisRaw("Horizontal");

        //������ ���� �� �ִ»��� + �������� �ƴҶ�
        if (canDamage && !isAttack && !isDie)
        {
            rb.velocity = new Vector2(h * speed, rb.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(h));
        }
        //������ ���� �� �ִ»��� + �����϶�
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
        //zŰ�� �Է¹ް�,  time �� nextAttack���� ũ��
        if (Input.GetKeyDown(KeyCode.X) && Time.time > nextAttack && (onGround || onPlatform) && !isDie)
        {
            audioSource.Stop();
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(attackClip);
            }
            //dash = false;
            //�ؽ������� �ִϸ��̼� Ʈ����-->�÷��̾� ���� �ִϸ��̼� ���. 
            isAttack = true;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("Attack");
            //�÷��̾� ���ݾִϸ��̼��� ����ϸ� �����ؼ� ���� �ִϸ��̼ǵ� ����Ǿ���.
            //�÷��̾� �ڽ� ������Ʈ�� Attack���� ���� weapon�� �ִϸ��̼� ���
            attack.PlayAnimation();
            //nextAttack���� ����ð� + fireRate. fireRate�� ���ݾִϸ��̼� ����ð���. 0.833��
            //�ѹ� �����ϸ� 0.611�ʰ� ������ ������ �� ������ ���Ѵ� �� ��.
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

    void Jump() //���� �޼���
    {
        //�ٴ� ����
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow) && !isDie)
        {
            StartCoroutine(JumpOff());
        }
        //���� ����
        else if (Input.GetKeyDown(KeyCode.Z) && ((onGround || onPlatform) || (!isDoubleJump && doubleJumpSkill)) && !isDie)
        {
            isJump = true;

            //2�� �������� �ƴϰ� �ٴڵ� �ƴ϶��
            if (!isDoubleJump && doubleJumpSkill && !onGround && !onPlatform)
            {
                //2�������� true��
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

    void Flip()  //�¿� ���� ��ȯ �޼���
    {
        if(!isDie)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator JumpOff() //�÷��� �Ʒ��� ���� �ڷ�ƾ
    {
        //�÷��̾� ���̾�� ���� ���̾� ������ �浹�� ��� ����.
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
                //������ �ִϸ��̼� + �������� ȿ�� + �ڷ� �з�����.
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
        //���ȭ�� ���� 1.5�� �� Ÿ��Ʋȭ������
        gameOverTr.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    void Save()  //���� �޼���
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
            DataManager.instance.SaveData();  //������ ������ �� ������ ����.
            currHp = iniHp;  //ü�� ��������
            float currHpPer = currHp / iniHp;
            stageUIManager.SetHUD(currHpPer);  //HUD�ֽ�ȭ

            //���� �޼���
            stageUIManager.ShowSave();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���̺� ����Ʈ���� ���尡��
        if(collision.CompareTag("SAVE"))
        {
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //���̺� ����Ʈ ����� ���� �Ұ���.
        if(collision.CompareTag("SAVE"))
        {
            canSave = false;
        }
    }
}
