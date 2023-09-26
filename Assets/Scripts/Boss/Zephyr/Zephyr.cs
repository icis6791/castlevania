
using System.Collections;
using UnityEngine;

public class Zephyr : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Move,
        Attack_A,
        Attack_B,
        Attack_C,
        Die
    }

    public State state = State.Idle;
    public Transform ground;
    public Transform p1, p2, p3;
    public bool isPaused = false;
    Transform bossTr;
    GameObject target;
    Animator anim;
    Rigidbody2D rb;
    WaitForSecondsRealtime ws;

    float speed = 1.5f;
    float dis;
    bool isDie;
    bool facingRight = false;
    bool isAttack = false;
    float t;
    int nextPat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossTr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("PLAYER");

        //StartCoroutine(NextPattern());
        ws = new WaitForSecondsRealtime(1.5f);
    }

    void Update()
    {
        t += Time.unscaledDeltaTime;
        dis = target.transform.position.x - bossTr.position.x;
    }

    private void FixedUpdate()
    {
        if (dis > 0 && !facingRight)
        {
            Flip();
            /*facingRight = !facingRight;
            bossTr.Rotate(0, 180, 0, Space.Self);*/
        }
        else if (dis < 0 && facingRight)
        {
            Flip();
            /*facingRight = !facingRight;
            bossTr.Rotate(0, 0, 0, Space.Self);*/
        }
    }

    void Flip()
    {
        if (!isAttack)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator NextPattern()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        print("패턴호출");
        print("거리는" + dis);
        if (Mathf.Abs(dis) > 4.5f)
        {
            nextPat = 0;
        }
        else
        {
            nextPat = 1;
        }

        if (!isDie)
        {
            switch (nextPat)
            {
                case 0:
                    state = State.Move;
                    StartCoroutine(Move());
                    break;
                case 1:
                    print("공격패턴");
                    state = State.Attack_A;
                    StartCoroutine(Charge());
                    yield return ws;
                    StartCoroutine(Attack_A());
                    break;
                case 2:
                    state = State.Attack_B;
                    StartCoroutine(Charge());
                    yield return ws;
                    //StartCoroutine(Attack_B());
                    break;
                case 3:
                    state = State.Attack_C;
                    break;
                case 4:
                    break;
            }
        }
        yield return null;
    }

    IEnumerator Move()
    {
        print("이동호출");
        t = 0;
        while (true)
        {
            print("이동중");
            rb.velocity = new Vector2(speed * dis / Mathf.Abs(dis), rb.velocity.y);

            anim.SetFloat("Dis", Mathf.Abs(rb.velocity.x));

            if (Mathf.Abs(dis) < 3.5f)
            {
                rb.velocity = Vector2.zero;
                anim.SetFloat("Dis", Mathf.Abs(rb.velocity.x));
                break;
            }
            yield return null;
        }

        StartCoroutine(NextPattern());
        yield break;
    }

    IEnumerator Attack_A()
    {
        t = 0;
        while (true)
        {
            anim.SetTrigger("Attack_A");
            print("공격이동");
            bossTr.position = Vector3.Lerp(bossTr.position, new Vector3(target.transform.position.x, bossTr.position.y, 0), t * 0.019f);

            if (Mathf.Abs(dis) < 1.65f)
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.5f);
        StartCoroutine(NextPattern());
        yield break;
    }

    /*IEnumerator Attack_B()
    {
        
    }*/

    IEnumerator Charge()
    {
        anim.SetTrigger("Charge");
        print("시간정지");
        yield return new WaitForSecondsRealtime(1);
        t = 0;
        while (t < 0.502f)
        {
            Time.timeScale = 0;
            isPaused = true;
            yield return null;
        }
        Time.timeScale = 1;
        isPaused = false;
        yield break;
    }
}
