using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameObject enemybulletPrefab;
    public float minDelay = 1f; // 총알 생성 간격 최솟값
    public float maxDelay = 3f; // 총알 생성 간격 최댓값
    public float bulletSpeed = 4f; // 총알의 이동 속도
    public int enemydamage = 10; // 총알의 데미지
    public float lifetime = 5f; // 총알의 수명

    public CircleCollider2D circleCollider2D;

    private bool isPlayerInsideTrigger; // player가 trigger 안에 있는지 여부
    private float nextSpawnTime; // 다음 총알 생성 시간

    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        SetNextSpawnTime();
    }

    void Update()
    {
        if (isPlayerInsideTrigger && Time.time >= nextSpawnTime)
        {
            SpawnBullet();
            SetNextSpawnTime();
        }
    }

    public void SetNextSpawnTime() // 다음 총알 생성 시간을 랜덤으로 설정
    {
        nextSpawnTime = Time.time + Random.Range(minDelay, maxDelay);
    }

    public void SpawnBullet()
    {
        // 총알 생성
        GameObject enemybullet = Instantiate(enemybulletPrefab, transform.position, Quaternion.identity);

        // 총알 초기화 및 이동
        StartCoroutine(MoveBullet(enemybullet));
    }

    private IEnumerator MoveBullet(GameObject enemybullet)
    {
        float spawnTime = Time.time;
        float speed = bulletSpeed;

        // 총알이 일정 속도로 이동
        while (Time.time - spawnTime < lifetime)
        {
            enemybullet.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

            yield return null;
        }

        // 수명이 다 되면 총알 삭제
        Destroy(enemybullet);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Player player = coll.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(enemydamage); // 플레이어에게 데미지
            }
            Destroy(coll.gameObject); // 플레이어와 충돌하면 총알 삭제
        }
    }
}
