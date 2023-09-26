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

    public GameObject ravenPrefab; //까마귀 프리팹
    public int ravenMaxPool = 12;
    public List<GameObject> ravenPool = new List<GameObject>();

    public GameObject bulletPrefab_A;  //총알 프리팹
    public List<GameObject> bulletAPool = new List<GameObject>();
    public GameObject bulletPrefab_B;
    public List<GameObject> bulletBPool = new List<GameObject>();
    public int bulletMaxPool = 20;

    public GameObject ballPrefab;  //공 프리팹
    public int ballMaxPool = 8;
    public List<GameObject> ballPool = new List<GameObject>();

    public State state = State.JumpUp;
    public Transform ground;  //바닥체크용
    public Transform bulletSpawn_A;  //총알 발사 위치
    public Transform bulletSpawn_B;  //총알 발사 위치
    public Transform ballSpawn;  //공 발사 위치
    Transform bossTr;
    GameObject target;  //플레이어
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;

    Vector3 endPos;  //이동할 목적지
    float x; //랜덤하게 받을 x, y
    float y;
    float t;  //늘어나는 시간
    float t1;  //줄어드는 시간
    float angle;  //공 각도.
    public int health = 400;  //체력
    public bool isDie = false;
    bool needLanding = false;  //착륙 판단
    public bool facingRight = false;  //좌우 방향 판단
    //public bool isReady = false;
    public bool onGround;  //바닥에 있는지 판단
    bool canAttack = true;
    public bool isAttack = false;
    int groundLayer;  //바닥 레이어
    int stack;  //공격할 때마다 쌓일 스택.
    int nextPat;  //다음 패턴
    int moveStack;  //이동 스택
    float dis;  //플레이어와의 거리

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

        if (stack > 4)  //스택이 5개 이상이면 착륙필요
        {
            needLanding = true;
        }
        else
        {
            needLanding = false;
        }

        //바닥 체크
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
        //하이어라키에서 빈 오브젝트 생성하는것과 동일
        GameObject ravenPools = new GameObject("RavenPools");
        GameObject bulletAPools = new GameObject("BulletAPools");
        GameObject bulletBPools = new GameObject("BulletBPools");
        GameObject ballPools = new GameObject("BallPools");

        for (int i = 0; i < ravenMaxPool; i++)
        {
            //동적생성하면서 위에서 지정한 objectPools의 자식으로 넣는것
            var obj = Instantiate<GameObject>(ravenPrefab, ravenPools.transform);
            obj.name = "Raven_" + i.ToString("00");
            obj.SetActive(false); ///미리 생성된 오브젝트 비활성화
            ravenPool.Add(obj); //오브젝트 풀에 생성된 오브젝트 추가
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
            //풀에 있는 총알들 중에서 비활성화 된놈을 가져오려고
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

    IEnumerator NextPattern()  //다음 패턴을 정하는 코루틴
    {
        if (onGround) //바닥일 때는 0번 패턴
        {
            nextPat = 0;
            canAttack = false;
        }
        else if (needLanding) //착륙필요할 땐 4번 패턴
        {
            nextPat = 4;
        }
        else if (Mathf.Abs(dis) >= 6)  // 플레이어와의 거리가 6이상이면 이동패턴
        {
            nextPat = 1;
        }
        else if (canAttack)  //위 조건이 아니고 공격 가능일땐 랜덤한 패턴
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
            //위로 날아오르기
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
        //좌우 방향에 맞춰서 까마귀 발사
        if (!facingRight)
        {
            for (int i = 0; i < 12; i++)
            {
                /*//랜덤하게 각도를 바꾸고 까마귀 생성
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(5, 60));
                var raven = Instantiate(ravenPrefab, bulletSpawn_A.position, bulletSpawn_A.rotation);*/
                //랜덤하게 각도를 바꾸고 까마귀 생성
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
                /*//랜덤하게 각도를 바꾸고 까마귀 생성
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(-5, -60));
                var raven = Instantiate(ravenPrefab, bulletSpawn_A.position, bulletSpawn_A.rotation);*/
                bulletSpawn_A.eulerAngles = new Vector3(0, 0, Random.Range(-5, -60));
                var raven = GetRaven();
                raven.transform.position = bulletSpawn_A.position;
                raven.transform.rotation = bulletSpawn_A.rotation;
                raven.SetActive(true);
            }
        }
        //발사 위치 회전값 초기화.
        bulletSpawn_A.eulerAngles = Vector3.zero;
    }

    void Attack_A()
    {
        //양쪽으로 총알 발사.
        for (int i = 0; i < 10; i++)
        {
            //랜덤한 각도로 총알 생성.
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

        //스폰 위치 각도 초기화
        bulletSpawn_A.eulerAngles = Vector3.zero;
        bulletSpawn_B.eulerAngles = Vector3.zero;
    }

    void Attack_B()
    {
        for (int i = 0; i < 4; i++)
        {
            //랜덤한 각도로 공 생성.
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
        //x, y의 범위에서 랜덤한 좌표를 목적지로 설정.
        x = Random.Range(173, 188);
        y = Random.Range(1, 2.5f);
        endPos = new Vector3(x, y, 0);

        //무조건 5이상 거리를 이동하게끔
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

    IEnumerator Land()  //착륙 코루틴
    {
        anim.SetTrigger("Landing");
        stack = 0;
        t = 0f;
        while (t < 2.5f)
        {
            //바닥으로 착지.
            bossTr.position = Vector3.Slerp(bossTr.position,
                                                             new Vector3(bossTr.position.x, -1.99f, 0),
                                                             t * 0.05f);
            yield return null;
        }
        //바닥위치
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

    private void OnCollisionEnter2D(Collision2D other)  //몸통에 닿았을 때
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
