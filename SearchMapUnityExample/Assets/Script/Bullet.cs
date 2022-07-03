using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private ObjectPool<Bullet> objectPool;
    private bool isAttacked=false;
    private float damage;

    public float Damage
    {
        set { damage = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartShoot(ObjectPool<Bullet> op)
    {
        objectPool = op;
        transform.position = op.parent.position;
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.AddForce((Player.Instance.transform.position+Vector3.up - op.parent.position).normalized*100.0f);
        StartCoroutine(Shoot());

    }
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(2.0f);
        objectPool.Enqueue(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {//���� �ƴ� ��� ������Ʈ�� �΋Hġ�� Ÿ�� ���� ������
            
            if (collision.gameObject.tag == "Player" && isAttacked == false)
            {//ù Ÿ���� �÷��̾� �϶�
                Debug.Log("Ÿ��");
                Player.Instance.stat.CurrentHp -= damage;

            }
            isAttacked = true;

        }
    }

}
