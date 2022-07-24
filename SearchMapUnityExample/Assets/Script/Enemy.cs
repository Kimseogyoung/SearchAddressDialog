using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    public enum Type
    {
        Slime,Turtle
    }
    public enum State
    {
        Idle, Moving, Attack, Dizzy, Die, Sense
    }


    public string enemyName;
    public Stat stat;//hp, speed;

    public float scanRange;
    public float attackRange;
    public float targetResetRange;
    public bool isImmediate;


    private Slider hpSlider;
    private bool isRun=false;
    protected GameObject targetObj;
    private Animator anim;
    private IEnumerator attackCo;
    private State state = State.Idle;
    public State enemyState
    {
        get { return state; }
        set
        {
            state = value;
            if (state == State.Moving)
            {
                if (isRun == true) anim.CrossFade("Run", 0.2f);
                else anim.CrossFade("Walk", 0.2f);
            }
            else if (state == State.Attack)
            {
                anim.CrossFade("Attack", 0.2f, -1, 0);
            }
            else anim.CrossFade(state.ToString(), 0.2f);
        }
    }

    public delegate void EnemyHandler(Enemy e);//�׾�����
    public event EnemyHandler OnDie;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        hpSlider = GetComponentInChildren<Slider>();
        Init();
    }

    protected virtual void Init()
    {
        stat.OnDie += Die;
        stat.OnChanged += ChangeHpSlider;
        stat.Init();
    }
    private void ChangeHpSlider()
    {
        hpSlider.value = stat.CurrentHp / stat.GetMaxHP();
    }
    void Die()
    {
        OnDie?.Invoke(this);
        Debug.Log("���");
        Destroy(gameObject);
        

    }
    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Moving:
                UpdateMoving();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Dizzy:
                break;
            case State.Die:
                break;
            case State.Sense:
                break;
        }
        hpSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up*1.5f);

    }
    protected void UpdateIdle() {
        GameObject player = GameObject.Find("player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= scanRange)
        {
            targetObj = player;
            enemyState = State.Moving;
        }
    }
    protected void UpdateMoving() {
        if (targetObj != null)
        {           
            float distance = (targetObj.transform.position - transform.position).magnitude;
            //Debug.Log(distance);
            if (distance <= attackRange)
            {//�Ÿ��� ���ݹ������� ������
                enemyState = State.Attack;
                return;

            }
            else if (distance >= targetResetRange)
            {//���¹����� �Ѿ��� ��
                stat.AddExtraSpeed(0);
                isRun = false;
                enemyState = State.Idle;
                return;

            }
            else if (distance >= targetResetRange / 2 && isRun==false)
            {//���󰡴� ������ 2���� 1���� �ֶ� �޸���
                stat.AddExtraSpeed(stat.originalSpeed * 0.5f);
                isRun = true;
                enemyState = State.Moving;
            }
            else if (distance < targetResetRange / 2 && isRun == true)
            {//���󰡴� ������ 2���� 1���� ����� �� �ȱ�
                stat.AddExtraSpeed(0);
                isRun = false;
                enemyState = State.Moving;
            }



            //�̵�
            Vector3 dir = (targetObj.transform.position - transform.position).normalized;
            dir.y = 0;
            
            transform.rotation = Quaternion.LookRotation(dir);
            transform.position += dir * stat.Speed * Time.deltaTime;
            //Debug.Log(dir * stat.Speed * Time.deltaTime);
            
        }
        else
        {
            enemyState = State.Idle;
        }
    }
    protected void UpdateDie() { 
        
    }
    protected void UpdateAttack() {
        if (targetObj != null)
        {
            Vector3 dir = targetObj.transform.position - transform.position;
            dir.y = 0;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    protected virtual void Attack()
    {

    }
    IEnumerator WaitAttackTime()
    {
        yield return new WaitForSeconds(stat.attackSpeed);
        if (enemyState == State.Attack)
        {
            enemyState = State.Attack;
        }
    }

    void OnHitEvent()
    {//���� Ÿ���� ������ ���
     //Ÿ�� ��� �ߵ��� hit �̺�Ʈ
       
        if (targetObj != null)
        {
            Stat targetStat = targetObj.GetComponent<Player>().stat;
            if (isImmediate == true)
                targetStat.CurrentHp -= stat.Damage;
            if (targetStat.CurrentHp <= 0)
            {//Ÿ�� ü���� 0�϶�
                targetObj = null;
                enemyState = State.Idle;
            }
            else
            {
                float distance = (targetObj.transform.position - transform.position).magnitude;
                if (distance <= attackRange)
                {//�Ÿ��� ���ݹ������� ������
                 //��� ����
                 //���ݼӵ��� �°� �����ؾ���.

                    //����ü �� ���� �޼���
                    Attack();

                    //���� ���� ����
                    if (attackCo != null)
                    {
                        StopCoroutine(attackCo);

                    }
                    attackCo = WaitAttackTime();
                    StartCoroutine(attackCo);
                }
                else if (distance >= targetResetRange)
                {//���¹����϶�
                    //idle
                    enemyState = State.Idle;
                }
                else if (distance > attackRange)
                {
                    enemyState = State.Moving;
                }

            }
            // enemyState = State.Moving;
        }
        else
        {
            enemyState = State.Idle;
        }
    }
}
