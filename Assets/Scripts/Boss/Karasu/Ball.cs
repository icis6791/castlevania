using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float t;
    float speed;
    float randomRot;
    public int damage = 30;
    GameObject target;
    Vector3 originScale;
    Vector3 originRot;

    private void Awake()
    {
        target = GameObject.FindWithTag("PLAYER");
        originScale = this.transform.localScale;
        originRot = Vector3.zero;
    }

    private void OnEnable()
    {
        speed = Random.Range(5, 6.5f);
        randomRot = Random.Range(-70, 70);
        this.transform.localRotation = Quaternion.Euler(0, 0, randomRot);
        StartCoroutine(Set());
    }

    IEnumerator Set()
    {
        //크기를 2초 동안 4로 만들기
        float targetScale = 4f; // 목표 크기
        float initialScale = this.transform.localScale.x; // 시작 크기
        float elapsedTime = 0f;
        float duration = 2.0f; //끝나는 시간

        while (elapsedTime < duration)
        {
            // 크기를 선형 보간
            float scaleFactor = Mathf.Lerp(initialScale, targetScale, elapsedTime / duration);
            this.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = new Vector3(4, 4, 4);
        t = 0f;
        while (t < 2.3f)
        {
            if (t > 0.2f && t < 0.8f)
            {
                //공이 발사되고 0.35에서 0.7초 사이에는 플레이어 방향을 바라보게 해서 유도탄으로 만듬.
                Vector2 dir = (target.transform.position - this.transform.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
                this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            //공 아래쪽 방향으로 날아가기.
            this.transform.Translate(Vector3.down * speed * Time.deltaTime);
            yield return null;
        }
        Disable();
    }

    private void Disable()
    {
        this.transform.localScale = originScale;
        this.transform.localRotation = Quaternion.Euler(originRot);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        t += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어에게 데미지 주기.
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Disable();
            }
        }
    }
}
