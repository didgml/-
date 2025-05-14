using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameObject enemybulletPrefab;
    public float minDelay = 1f; // �Ѿ� ���� ���� �ּڰ�
    public float maxDelay = 3f; // �Ѿ� ���� ���� �ִ�
    public float bulletSpeed = 4f; // �Ѿ��� �̵� �ӵ�
    public int enemydamage = 10; // �Ѿ��� ������
    public float lifetime = 5f; // �Ѿ��� ����

    public CircleCollider2D circleCollider2D;

    private bool isPlayerInsideTrigger; // player�� trigger �ȿ� �ִ��� ����
    private float nextSpawnTime; // ���� �Ѿ� ���� �ð�

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

    public void SetNextSpawnTime() // ���� �Ѿ� ���� �ð��� �������� ����
    {
        nextSpawnTime = Time.time + Random.Range(minDelay, maxDelay);
    }

    public void SpawnBullet()
    {
        // �Ѿ� ����
        GameObject enemybullet = Instantiate(enemybulletPrefab, transform.position, Quaternion.identity);

        // �Ѿ� �ʱ�ȭ �� �̵�
        StartCoroutine(MoveBullet(enemybullet));
    }

    private IEnumerator MoveBullet(GameObject enemybullet)
    {
        float spawnTime = Time.time;
        float speed = bulletSpeed;

        // �Ѿ��� ���� �ӵ��� �̵�
        while (Time.time - spawnTime < lifetime)
        {
            enemybullet.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

            yield return null;
        }

        // ������ �� �Ǹ� �Ѿ� ����
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
                player.TakeDamage(enemydamage); // �÷��̾�� ������
            }
            Destroy(coll.gameObject); // �÷��̾�� �浹�ϸ� �Ѿ� ����
        }
    }
}
