using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;
using Color = UnityEngine.Color;

public class Karas : MonoBehaviour, Enemy
{
    public enum State
    {
        JumpUp = 0,
        Move,
        Attack_A,
        Attack_B,
        Attack_C,
        Landing,
        Die
    }

    public GameObject ravenPrefab; //��� ������
    public int ravenMaxPool = 12;
    public List<GameObject> ravenPool = new List<GameObject>();

    public GameObject bulletPrefab_A;  //�Ѿ� ������
    public List<GameObject> bulletAPool = new List<GameObject>();
    public GameObject bulletPrefab_B;
    public List<GameObject> bulletBPool = new List<GameObject>();
    public int bulletMaxPool = 20;

    public GameObject ballPrefab;  //�� ������
    public int ballMaxPool = 8;
    public List<GameObject> ballPool = new List<GameObject>();

    public State state = State.JumpUp;
    public Transform ground;  //�ٴ�üũ��
    public Transform bulletSpawn_A;  //�Ѿ� �߻� ��ġ
    public Transform bulletSpawn_B;  //�Ѿ� �߻� ��ġ
    public Transform ballSpawn;  //�� �߻� ��ġ
    Transform bossTr;
    GameObject target;  //�÷��̾�
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;

    Vector3 endPos;  //�̵��� ������
    float x; //�����ϰ� ���� x, y
    float y;
    float t;  //�þ�� �ð�
    float t1;  //�پ��� �ð�
    float angle;  //�� ����.
    public int health = 400;  //ü��
    public bool isDie = false;
    bool needLanding = false;  //���� �Ǵ�
    public bool facingRight = false;  //�¿� ���� �Ǵ�
    //public bool isReady = false;
    public bool onGround;  //�ٴڿ� �ִ��� �Ǵ�
    bool canAttack = true;
    public bool isAttack = false;
    int groundLayer;  //�ٴ� ���̾�
    int stack;  //������ ������ ���� ����.
    int nextPat;  //���� ����
    int moveStack;  //�̵� ����
    float dis;  //�÷��̾���� �Ÿ�

    void Awake()
    {
        CreatePooling();
    }

    void Start()
    {
        bossTr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindWithTag("PLAYER");

        groundLayer = LayerMask.NameToLayer("GROUND");
    }

    void Update()
    {
        t += Time.deltaTime;
        t1 -= Time.deltaTime * 50;

        if (stack > 4)  //������ 5�� �̻��̸� �����ʿ�
        {
            needLanding = true;
        }
        else
        {
            needLanding = false;
        }

        //�ٴ� üũ
        onGround = Physics2D.Linecast(transform.position, ground.position, 1 << groundLayer);

        dis = target.transform.position.x - transform.position.x;

        if (dis > 0 && !facingRight)
        {
            Flip();
        }
        else if (dis < 0 && facingRight)
        {
            Flip();
        }
    }

    public void CreatePooling()
    {
        //���̾��Ű���� �� ������Ʈ �����ϴ°Ͱ� ����
        GameObject ravenPools = new GameObject("RavenPools");
        GameObject bulletAPools = new GameObject("BulletAPools");
        GameObject bulletBPools = new GameObject("BulletBPools");
        GameObject ballPools = new GameObject("BallPools");

        for (int i = 0; i < ravenMaxPool; i++)
        {
            //���������ϸ鼭 ������ ������ objectPools�� �ڽ����� �ִ°�
            var obj = Instantiate<GameObject>(ravenPrefab, ravenPools.transform);
            obj.name = "Raven_" + i.ToString("00");
            obj.SetActive(false); ///�̸� ������ ������Ʈ ��Ȱ��ȭ
            ravenPool.Add(obj); //������Ʈ Ǯ�� ������ ������Ʈ �߰�
        }

        for (int i = 0; i < bulletMaxPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab_A, bulletAPools.transform);
            var _obj = Instantiate<GameObject>(bulletPrefab_B, bulletBPools.transform);
            obj.name = "BulletA_" + i.ToString("00");
            _obj.name = "BulletB_" + i.ToString("00");
            obj.SetActive(false);
            _obj.SetActive(false);
            bulletAPool.Add(obj);
            bulletBPool.Add(_obj);
        }

        for (int i = 0; i < ballMaxPool; i++)
        {
            var obj = Instantiate<GameObject>(ballPrefab, ballPools.transform);
            obj.name = "Ball_" + i.ToString("00");
            obj.SetActive(false);
            ballPool.Add(obj);
        }
    }

    public GameObject GetRaven()
    {
        for (int i = 0; i < ravenPool.Count; i++)
        {
            //Ǯ�� �ִ� �Ѿ˵� �߿��� ��Ȱ��ȭ �ȳ��� ����������
            if (!ravenPool[i].activeSelf)
            {
                return ravenPool[i];
            }
        }
        return null;
    }

    public GameObject GetBulletA()
    {
        for (int i = 0; i < bulletAPool.Count; i++)
        {
            if (!bulletAPool[i].activeSelf)
            {
                return bulletAPool[i];
            }
        }
        return null;
    }

    public GameObject GetBulletB()
    {
        for (int i = 0; i < bulletBPool.Count; i++)
        {
            if (!bulletBPool[i].activeSelf)
            {
                return bulletBPool[i];
            }
        }
        return null;
    }

    public GameObject GetBall()
    {
        for (int i = 0; i < ballPool.Count; i++)
        {
            if (!ballPool[i].activeSelf)
            {
                return ballPool[i];
            }
        }
        return null;
    }

    IEnumerator NextPattern()  //���� ������ ���ϴ� �ڷ�ƾ
    {
        if (onGround) //�ٴ��� ���� 0�� ����
        {
            nextPat = 0;
            canAttack = false;
        }
        else if (needLanding) //�����ʿ��� �� 4�� ����
        {
            nextPat = 4;
        }
        else if (Mathf.Abs(dis) >= 6)  // �÷��̾���� �Ÿ��� 6�̻��̸� �̵�����
        {
            nextPat = 1;
        }
        else if (canAttack)  //�� ������ �ƴϰ� ���� �����϶� ������ ����
            nextPat = Random.Range(1, 4);

        if (!isDie)
        {
            switch (nextPat)
            {
                case 0:
                    state = State.JumpUp;
                    StartCoroutine(JumpUp());
                    yield return new WaitForSeconds(3f);
                    break;
                case 1:
                    if(moveStack >= 3)
                    {
                        StartCoroutine(NextPattern());
                        break;
                    }
                    state = State.Move;
                    canAttack = true;
                    StartCoroutine(Move());
                    moveStack++;
                    yield return new WaitForSeconds(2.5f);
                    StartCoroutine(NextPattern());
                    break;
                case 2:
                    state = State.Attack_A;
                    anim.SetTrigger("Attack_A");
                    stack++;
                    moveStack = 0;
                    yield return new WaitForSeconds(2);
                    StartCoroutine(NextPattern());
                    break;
                case 3:
                    state = State.Attack_B;
                    anim.SetTrigger("Attack_B");
                    stack++;
                    moveStack = 0;
                    //bossTr.position = this.transform.position;
                    Attack_B();
                    yield return new WaitForSeconds(4);
                    StartCoroutine(NextPattern());
                    break;
                case 4:
                    state = State.Landing;
                    stack = 0;
                    moveStack = 0;
                    yield return new WaitForSeconds(1);
                    StartCoroutine(Land());
                    break;
            }
        }
    }

    IEnumerator JumpUp()
    {
        t = 0f;
        while (t < 1f)
        {
            //���� ���ƿ�����
            bossTr.position = Vector3.Lerp(bossTr.position,
                                                            new Vector3(bossTr.position.x,
                                                            1.6f, 0), t * 0.15f);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(NextPattern());
        isAttack = false;
    }

    void Raven()
    {
        //�¿� ���⿡ ���缭 ��� �߻�
        if (!facingRight)
        {
            for (int i = 0; i < 12; i++)
            {
                /*//�����ϰ� ������ �ٲٰ� ��� ����
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(5, 60));
                var raven = Instantiate(ravenPrefab, bulletSpawn_A.position, bulletSpawn_A.rotation);*/
                //�����ϰ� ������ �ٲٰ� ��� ����
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(5, 60));
                var raven = GetRaven();
                raven.transform.position = bulletSpawn_A.position;
                raven.transform.rotation = bulletSpawn_A.rotation;
                raven.SetActive(true);
            }
        }
        else if (facingRight)
        {
            for (int i = 0; i < 12; i++)
            {
                /*//�����ϰ� ������ �ٲٰ� ��� ����
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(-5, -60));
                var raven = Instantiate(ravenPrefab, bulletSpawn_A.position, bulletSpawn_A.rotation);*/
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(-5, -60));
                var raven = GetRaven();
                raven.transform.position = bulletSpawn_A.position;
                raven.transform.rotation = bulletSpawn_A.rotation;
                raven.SetActive(true);
            }
        }
        //�߻� ��ġ ȸ���� �ʱ�ȭ.
        bulletSpawn_A.eulerAngles = Vector3.zero;
    }

    void Attack_A()
    {
        //�������� �Ѿ� �߻�.
        for (int i = 0; i < 10; i++)
        {
            //������ ������ �Ѿ� ����.
            //var bullet_A = Instantiate(bulletPrefab_A, bulletSpawn_A.position, bulletSpawn_A.rotation);
            bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(10, 55));
            var bullet_A = GetBulletA();
            bullet_A.transform.position = bulletSpawn_A.position;
            bullet_A.transform.rotation = bulletSpawn_A.rotation;
            bullet_A.SetActive(true);
            //var bullet_B = Instantiate(bulletPrefab_B, bulletSpawn_B.position, bulletSpawn_B.rotation);
            bulletSpawn_B.eulerAngles = new Vector3(0, 0, Random.Range(-10, -55));
            var bullet_B = GetBulletB();
            bullet_B.transform.position = bulletSpawn_B.position;
            bullet_B.transform.rotation = bulletSpawn_B.rotation;
            bullet_B.SetActive(true);
        }

        //���� ��ġ ���� �ʱ�ȭ
        bulletSpawn_A.eulerAngles = Vector3.zero;
        bulletSpawn_B.eulerAngles = Vector3.zero;
    }

    void Attack_B()
    {
        for (int i = 0; i < 4; i++)
        {
            //������ ������ �� ����.
            //float random = Random.Range(-70, 71);
            //print(random);
            //ballSpawn.localEulerAngles = new Vector3(0, 0, 250);
            //print(ballSpawn.localEulerAngles.z);
            var ball = GetBall();
            ball.transform.position = ballSpawn.position;
            //ball.transform.rotation = ballSpawn.rotation;
            ball.SetActive(true);
        }
        //ballSpawn.eulerAngles = Vector3.zero;
    }

    void Flip()
    {
        if (!isAttack && !isDie)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator Move()
    {
        //x, y�� �������� ������ ��ǥ�� �������� ����.
        x = Random.Range(173, 188);
        y = Random.Range(1, 2.5f);
        endPos = new Vector3(x, y, 0);

        //������ 5�̻� �Ÿ��� �̵��ϰԲ�
        while (Mathf.Abs(bossTr.position.x - endPos.x) > 4)
        {
            x = Random.Range(173, 188);
            endPos = new Vector3(x, y, 0);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        t = 0f;
        while (t < 1.5f)
        {
            bossTr.position = Vector3.Lerp(bossTr.position, endPos, t * 0.05f);
            yield return null;
        }
    }

    IEnumerator Land()  //���� �ڷ�ƾ
    {
        anim.SetTrigger("Landing");
        stack = 0;
        t = 0f;
        while (t < 2.5f)
        {
            //�ٴ����� ����.
            bossTr.position = Vector3.Slerp(bossTr.position,
                                                             new Vector3(bossTr.position.x, -1.99f, 0),
                                                             t * 0.05f);
            yield return null;
        }
        //�ٴ���ġ
        bossTr.position = new Vector3(bossTr.position.x, -2, 0);

        needLanding = false;
    }

    void IsAttack()
    {
        isAttack = !isAttack;
    }

    public void TakeDamage(int damage)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if (health <= 0)
        {
            //rb.isKinematic = true;
            //rb.velocity = Vector2.zero;
            GetComponent<CapsuleCollider2D>().enabled = false;
            PlayerCtrl player = GameObject.FindWithTag("PLAYER").GetComponent<PlayerCtrl>();
            player.bossComB = true;
            anim.SetTrigger("Die");
            isDie = true;
            StartCoroutine(Vanish());
        }
    }

    IEnumerator DamageCoroutine()
    {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(1.2f);
        Color color = sprite.color;
        t1 = 80;
        while (true)
        {
            color.a = t1 / 80;
            sprite.color = color;
            yield return null;
            if (t1 <= 0)
            {
                t1 = 0;
                Destroy(gameObject);
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)  //���뿡 ����� ��
    {
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(15);
            }
        }
    }
}
