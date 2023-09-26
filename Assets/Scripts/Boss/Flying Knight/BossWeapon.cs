using System.Collections;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public Rigidbody2D wp_a;  //무기A
    public Rigidbody2D wp_b;  //무기B
    public Transform spawnA;  //무기생성 위치
    public Transform spawnB;

    EdgeCollider2D edge;  //무기이동시킬때 트리거로 쓸 엣지콜라이더
    GameObject target;  //플레이어.
    Transform tr;

    float rotSpeed = 150f; //회전 속도.
    float timeA;  //회전값저장
    float timeB;
    float t = 0f;  //시간저장
    float angle_A;  //무기가 플레이어를 바라볼 각도
    float angle_B;
    public int damage = 40; //데미지

    FlyingKnight boss;  //보스 상태 체크용
    Vector3 lookDir;  //무기가 플레이어를 바라볼 방향

    private void Start()
    {
        tr = gameObject.transform;
        boss = GetComponentInParent<FlyingKnight>();
        target = GameObject.FindWithTag("PLAYER");
        spawnA = GameObject.FindWithTag("SPAWNA").GetComponent<Transform>();
        spawnB = GameObject.FindWithTag("SPAWNB").GetComponent<Transform>();

        //혹여 다른 패턴중에 닿으면 안되니까 엣지 비활성화.
        edge = GameObject.Find("Forrest Sky4").GetComponent<EdgeCollider2D>();
        edge.enabled = false;
    }

    public IEnumerator IRotation(float targetAngle)  //전체회전
    {
        //무기회전하는 자리로.
        StartCoroutine(RotateWeapon());
        yield return new WaitForSeconds(1.5f);

        float st = tr.eulerAngles.z;
        float er = targetAngle;

        //시간 초기화.
        t = 0f;
        //1.5초 동안. 프레임단위로 반복.
        while (t < 1.5f)
        {
            float speed = t;
            //z축 회전을 360도
            float zRotation = Mathf.Lerp(st, er, speed) % 360.0f;

            tr.eulerAngles = new Vector3(tr.eulerAngles.x, tr.eulerAngles.y, zRotation);
            yield return null;
        }
        //회전 끝나고 무기 원래 자리로.
        StartCoroutine(SetPosition());
        yield break;
    }

    public IEnumerator IFire()  //무기 발사
    {
        //각 무기회전.
        StartCoroutine(RotateWeapon());
        //B패턴일 때
        if (boss.state == FlyingKnight.State.Attack_B)
        {
            //엣지 활성화
            edge.enabled = true;
            //각 무기회전시간 보장.
            yield return new WaitForSeconds(1.5f);

            //무기 칼날방향(아래)로 날리기
            t = 0f;
            while (t < 2.3f)
            {
                wp_a.transform.Translate(-transform.up * 20f * Time.deltaTime);
                wp_b.transform.Translate(-transform.up * 20f * Time.deltaTime);
                yield return null;
            }
            //엣지 비활성화
            edge.enabled = false;

            //각 무기위치와 회전을 스폰으로.
            wp_a.transform.position = spawnA.position;
            wp_a.transform.rotation = spawnA.rotation;
            wp_b.transform.position = spawnB.position;
            wp_b.transform.rotation = spawnB.rotation;

            yield return new WaitForSeconds(0.5f);

            //스폰에서 원래자리로 내리기
            t = 0f;
            while (t < 2f)
            {
                //print("선형 보간중");
                wp_a.transform.position = Vector3.Lerp(wp_a.transform.position,
                                                                               new Vector3(spawnA.position.x, tr.position.y, 0),
                                                                               t * 0.075f);
                wp_b.transform.position = Vector3.Lerp(wp_b.transform.position,
                                                                               new Vector3(spawnB.position.x,
                                                                               tr.position.y, 0), t * 0.075f);

                yield return null;
            }
            yield break;
        }
        //C패턴일 때
        else if (boss.state == FlyingKnight.State.Attack_C)
        {
            //각 무기 회전시간 보장
            yield return new WaitForSeconds(2.5f);

            t = 0f;
            while (t < 1.5f)
            {
                wp_a.transform.Translate(-transform.up * 20f * Time.deltaTime);
                wp_b.transform.Translate(-transform.up * 20f * Time.deltaTime);
                yield return null;
            }

            wp_a.transform.position = spawnA.position;
            wp_a.transform.rotation = spawnA.rotation;
            wp_b.transform.position = spawnB.position;
            wp_b.transform.rotation = spawnB.rotation;

            yield return new WaitForSeconds(0.5f);

            t = 0f;
            while (t < 2f)
            {
                //print("선형 보간중");
                wp_a.transform.position = Vector3.Lerp(wp_a.transform.position,
                                                                               new Vector3(spawnA.position.x,
                                                                               tr.position.y, 0), t * 0.075f);
                wp_b.transform.position = Vector3.Lerp(wp_b.transform.position,
                                                                               new Vector3(spawnB.position.x,
                                                                               tr.position.y, 0), t * 0.075f);

                yield return null;
            }
            yield break;
        }
    }

    IEnumerator SetPosition()  //각 무기 원래 회전으로 되돌리는 코루틴
    {
        //A패턴일때
        if (boss.state == FlyingKnight.State.Attack_A)
        {
            //시작회전값
            timeA = -90f;
            timeB = 90f;
            while (true)
            {
                wp_a.transform.eulerAngles = new Vector3(0, 0, timeB);
                //print("timeA" + timeA);
                wp_b.transform.eulerAngles = new Vector3(0, 0, timeA);
                //print("timeB" + timeB);
                yield return null;
                //0이 될때까지. 어긋날 수도 있으니 확실히 0으로 바꾸기.
                if (timeA >= 0)
                {
                    wp_a.transform.eulerAngles = new Vector3(0, 0, 0);
                    wp_b.transform.eulerAngles = new Vector3(0, 0, 0);
                    yield break;
                }
            }
        }
        //B패턴일때
        else if (boss.state == FlyingKnight.State.Attack_B)
        {
            timeA = -130f;
            timeB = 130f;
            while (true)
            {
                wp_a.transform.eulerAngles = new Vector3(0, 0, timeA);
                wp_b.transform.eulerAngles = new Vector3(0, 0, timeB);

                yield return null;

                if (timeA >= 0)
                {
                    wp_a.transform.eulerAngles = new Vector3(0, 0, 0);
                    wp_b.transform.eulerAngles = new Vector3(0, 0, 0);
                    yield break;
                }
            }
        }
    }

    public IEnumerator RotateWeapon()   //각 무기 공격방향으로 회전시키는 코루틴
    {
        if (boss.state == FlyingKnight.State.Attack_A)
        {
            timeA = 0f;
            timeB = 0f;
            while (true)
            {
                wp_a.transform.eulerAngles = new Vector3(0, 0, timeA);
                wp_b.transform.eulerAngles = new Vector3(0, 0, timeB);
                yield return null;
                //각 무기 회전값의 절대값이 90도가 될때까지.
                if (Mathf.Abs(timeA) >= 90)
                {
                    timeA = -90;
                    timeB = 90;
                    yield break;
                }
            }
        }
        else if (boss.state == FlyingKnight.State.Attack_B)
        {
            timeA = 0f;
            timeB = 0f;
            while (true)
            {
                wp_a.transform.eulerAngles = new Vector3(0, 0, timeB);
                wp_b.transform.eulerAngles = new Vector3(0, 0, timeA);
                yield return null;

                if (Mathf.Abs(timeA) >= 130)
                {
                    yield break;
                }
            }
        }
        else if (boss.state == FlyingKnight.State.Attack_C)
        {
            //시간 초기화.
            t = 0;
            while (t < 2f)
            {
                //A, B에 시간차를 둔다.
                if (t < 1f)
                {
                    //무기가 플레이어를 바라본다.
                    wp_a.transform.rotation = Quaternion.AngleAxis(angle_A, Vector3.forward);
                }
                if (t < 2f)
                {
                    wp_b.transform.rotation = Quaternion.AngleAxis(angle_B, Vector3.forward);
                }

                yield return null;
            }
        }
    }

    void Update()
    {
        //시간 * 회전속도를 누적.
        timeA += Time.deltaTime * rotSpeed;
        timeB -= Time.deltaTime * rotSpeed;
        //시간 누적.
        t += Time.deltaTime;

        //무기가 타겟을 바라보는 방향.
        Vector2 dir_A = (target.transform.position - wp_a.transform.position);
        //방향 벡터를 이용해서
        //Mathf.Atan2 =탄젠트 값으로 y/x값을 가지는 radian을 반환.
        //Mathf.Rad2Deg = radian값을 각도로 반환.
        //angle에 90도를 더해야 무기아래쪽이 플레이어를 향함.
        angle_A = Mathf.Atan2(dir_A.y, dir_A.x) * Mathf.Rad2Deg + 90;
        Vector2 dir_B = (target.transform.position - wp_b.transform.position);
        angle_B = Mathf.Atan2(dir_B.y, dir_B.x) * Mathf.Rad2Deg + 90;

        if (boss.state == FlyingKnight.State.Die)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
