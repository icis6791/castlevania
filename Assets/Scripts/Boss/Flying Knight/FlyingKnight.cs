using System.Collections;
using UnityEngine;

public class FlyingKnight : MonoBehaviour, Enemy
{
    public enum State  //보스의 상태
    {
        Idle = 0,
        Move,
        Attack_A,
        Attack_B,
        Attack_C,
        Die
    }

    public State state = State.Idle;  //기본값은 이동
    Transform playerTr;  //플레이어 위치
    Transform bossTr;  //보스위치
    BossWeapon bossWp;  //보스무기
    WaitForSeconds ws;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;

    public AudioClip damageClip;
    public GameObject itemdrop;
    public float speed = 0.6f;  //이동에 곱해줄 속도
    public Vector3 endPos;  //랜덤(x, y). 목표지점.
    public bool isDie = false;  //사망
    public int health = 280;

    int nextPat;  //다음 패턴
    int xCo;  //랜덤으로 받을 x좌표
    int yCo;  //랜덤으로 받을 y좌표
    float fraction;  //Slerp에 쓸 계수
    float t;

    private void Awake()
    {
        //플레이어를 태그로 찾은 다음, null이 아니면 transform 컴포넌트를 받는다.
        var player = GameObject.FindGameObjectWithTag("PLAYER");

        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
    }

    private void OnEnable()
    {
        //보스 자신 위치
        bossTr = GetComponent<Transform>();
        //보스의 무기 제어
        bossWp = GetComponentInChildren<BossWeapon>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();  

        transform.position = new Vector3(40, -9, 0);
        //4초뒤부터 패턴 실행.
        Invoke("First", 2f);
        ws = new WaitForSeconds(4.5f);
    }

    void First()
    {
        StartCoroutine(NextPattern());
    }

    IEnumerator NextPattern()
    {
        //다음의 패턴을 랜덤으로 결정하는 메서드.
        nextPat = Random.Range(0, 4);

        print(nextPat);
        //죽지 않는 동안
        if (!isDie)
        {
            switch (nextPat)
            {
                case 0:
                    state = State.Move;
                    //print("이동");
                    // 좌표를 랜덤하게 설정.
                    xCo = Random.Range(32, 47);
                    yCo = Random.Range(-10, -8);
                    //이동할 목적지.
                    endPos = new Vector3(xCo, yCo, 0);
                    yield return ws;
                    //다음 패턴 호출
                    StartCoroutine(NextPattern());
                    break;
                case 1:
                    state = State.Attack_A;
                    endPos = playerTr.position;

                    //보스무기 스크립트의 무기 회전 코루틴 호출. 360도 회전.
                    StartCoroutine(bossWp.IRotation(360));
                    yield return ws;
                    StartCoroutine(NextPattern());
                    break;
                case 2:
                    state = State.Attack_B;
                    //보스 무기 스크립트의 무기 발사 코루틴 호출.
                    StartCoroutine(bossWp.IFire());
                    yield return new WaitForSeconds(6);
                    StartCoroutine(NextPattern());
                    break;
                case 3:
                    state = State.Attack_C;
                    //보스 무기 스크립트의 무기 발사 코루틴 호출.
                    StartCoroutine(bossWp.IFire());
                    yield return new WaitForSeconds(7);
                    StartCoroutine(NextPattern());
                    break;
            }
        }
    }

    private void Update()
    {
        t -= Time.deltaTime * 50;

        //실시간으로 변하거나 값을 더해줘야 하는 변수들은 여기서 제어.
        //각 상태에 따라
        if (state == State.Move)
        {
            //Slerp로 이동. fraction이 1이 될수록 느려지고 멈춤.
            fraction += speed * Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, endPos, fraction);
            //초기화
            fraction = 0;
        }
        else if (state == State.Attack_A)
        {
            fraction += speed * Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, endPos, fraction);

            fraction = 0;
        }
        else if (state == State.Attack_B)
        {
            //제자리.
            bossTr.position = this.transform.position;
        }
        else if (state == State.Attack_C)
        {
            bossTr.position = this.transform.position;
        }
    }

    public void TakeDamage(int damage)  //데미지 받는 메서드
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if (health <= 0)
        {
            print("사망");
            GetComponent<CapsuleCollider2D>().enabled = false;
            //플레이어 스크립트의 보스클리어 참으로.
            PlayerCtrl player = GameObject.FindWithTag("PLAYER").GetComponent<PlayerCtrl>();
            player.bossComA = true;
            anim.SetTrigger("Die");
            isDie = true;
            state = State.Die;
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
        t = 80;
        while (true)
        {
            color.a = t / 80;
            sprite.color = color;
            yield return null;
            if (t <= 0)
            {
                t = 0;
                Destroy(gameObject);
                break;
            }
        }
        var item = Instantiate(itemdrop, transform.position, transform.rotation); //보스 죽고나면 위치에 아이템 생성.
    }
}
