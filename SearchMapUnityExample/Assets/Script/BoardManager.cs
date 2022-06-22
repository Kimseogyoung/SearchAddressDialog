using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA_Dialog;

public class BoardManager : MonoBehaviour
{
    public Board[,] boards;
    public Vector2 pos;
    public float scale;

    public int size;
    public float spacing;
    public float targetSpace;
    public Locale centerLc;
    public Locale globalLc;

    public int[,] aroundB;//�����¿� ����

    private Vector2[] dirs;
    private Vector2 preBoardPos;//��������ġ�� preBoard ��ġ
    private GameObject preBoard;//��������ġ�� preBoard 
    
    //  0.00272, 0.00345
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init()
    {

        centerLc = new Locale("", 0, 0);
        //���� �浵 �Ÿ�
        globalLc = new Locale("", 0.00272f, 0.00345f);

        preBoardPos = new Vector2(-1, -1);//�ʱ�ȭ
        boards = new Board[size,size];
        aroundB = new int[3, 3];
        pos = new Vector2(0, 0);

        dirs = new Vector2[4];
        dirs[0] = Vector2.up;
        dirs[1] = Vector2.down;
        dirs[2] = Vector2.left;
        dirs[3] = Vector2.right;

        AddBoard(size / 2, size / 2);
        SetCurrentPos(size / 2, size / 2);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {//Ȯ�ι�ư
            if (preBoard != null)
            {//preBoard�� �����Ѵٸ� ���� ��ġ
                AddBoard();
            }
        }
        if (Player.Instance != null)
        {
            //1. ������ ������ 
            Vector3 dir_p = Player.Instance.transform.forward;
            dir_p.y = 0;
            float min= Vector3.Distance(Vector3.forward, dir_p);
            Vector3 dir = Vector3.forward;

            if(min>= Vector3.Distance(Vector3.back, dir_p))
            {
                min = Vector3.Distance(Vector3.back, dir_p);
                dir = Vector3.back;
            }
            if (min >= Vector3.Distance(Vector3.left, dir_p))
            {
                min = Vector3.Distance(Vector3.left, dir_p);
                dir = Vector3.left;
            }
            if (min >= Vector3.Distance(Vector3.right, dir_p))
            {
                min = Vector3.Distance(Vector3.right, dir_p);
                dir = Vector3.right;
            }

            //2. �ش� ���� �Ÿ����
            //size�ʰ���  ������
            int state = aroundB[(int)dir.x + 1, (int)dir.z + 1];
            if (state == -1) return;
            
            //��ĭ�̰ų�, �̹� �߰��� ���尡 �ִٸ�
            //�Ÿ� ���
            //  ���� dir�� x=0 z=1�̸� ���� �����ε�, �̶� z�� �������θ� �Ÿ��� ����ؾ���.
            //  = ���� ���Ϳ��� 1�� �� �����Ͽ� �Ÿ���� = ���� ���� ����
            int plus = (int)Vector3.Dot(Vector3.one, dir);
            float target = plus * Vector3.Dot(boards[(int)pos.y, (int)pos.x].transform.position,  dir)+
                ((scale/2)+spacing)* plus;
            float current = plus * Vector3.Dot(Player.Instance.transform.position, dir);

            if (state == 0 && Mathf.Abs(target-current)<=spacing+targetSpace)
            {//�ش� ���� ���尡 ��ĭ�̶�� && Ư�� �Ÿ� ���̶��
                CreatePreBoard((int)(-dir.z + pos.y), (int)(dir.x + pos.x));
            }
            else if(state == 1 )
            {//�ִٸ� 
                //�ش� ����� �Ѿ���� üũ

                //!!!!!!!!!!!!!!!!!!!!!!!�밢�� ������ ��¿������?
                //�ذ� ��� 1. board �浹ó�� or Ray
                //�ذ� ��� 2. ����
                //�ϴ� �밢������ �������ٴϴ� ������ ����

                if (target <= 0 && current<target)
                {
                    Debug.Log("���� �Ѿ : r=" + (int)(-dir.z + pos.y) + " x=" + (int)(dir.x + pos.x));
                    SetCurrentPos((int)(-dir.z + pos.y), (int)(dir.x + pos.x));
                }
                else if (target > 0 && current > target)
                {
                    Debug.Log("���� �Ѿ : r=" + (int)(-dir.z + pos.y) + " x=" + (int)(dir.x + pos.x));
                    SetCurrentPos((int)(-dir.z + pos.y), (int)(dir.x + pos.x));
                }
            }
            else
            {
                if(preBoard!=null)
                {//�ʱ�ȭ��������
                    Debug.Log("����2");
                    Destroy(preBoard);
                    preBoardPos.x = -1;
                    preBoardPos.y = -1;
                }
            }


        }

        
    }
    public void SetCurrentPos(int r, int c)
    {
        if(r<0 ||c<0 || r>=size || c >= size)
        {//���� ���� �����Ϸ��� �� �� 
            return;
        }
        pos.x = c;
        pos.y = r;

        int newr = r;
        int newc = c;
        foreach(Vector2 d in dirs)
        {           //�ֺ� �װ� ��� ����
                    //����ĭ�̶�� ���� -1
                    //�̹� �ִ� ĭ�̶�� ǥ�� 1
                    //��ĭ�̶�� 0
            newr = r + (int)d.y;
            newc = c + (int)d.x;


            if(newr<0||newr>=size|| newc<0 || newc >= size)
            {
                aroundB[(int)d.x+1, (int)d.y+1] = -1;
            }
            else if(boards[newr,newc]!=null)
            {
                aroundB[(int)d.x+1, (int)d.y + 1] = 1;
            }
            else
            {
                aroundB[(int)d.x+1, (int)d.y+1] = 0;
            }
        }

    }
    public void CreatePreBoard(int r, int c)
    {
        if (preBoardPos.x == c && preBoardPos.y == r) return;//������ġ��� ����
        if (preBoard != null)
        {
            Debug.Log("����");
            Destroy(preBoard);
        }
        Debug.Log("����");
        preBoardPos.x = c;
        preBoardPos.y = r;
        preBoard = Instantiate(Resources.Load("Prefabs/BoardPre") as GameObject, transform);
        preBoard.transform.position = new Vector3(scale * (c - size / 2) + spacing * (c - size / 2), 0,
            -(scale * (r - size / 2) + spacing * (r - size / 2)));
    }
    public void AddBoard(int r, int c, ScanObject.Type type=0)
    {//Ư�� ��ġ�� ���� ������ ��, ex) ���� ����
        GameObject obj=Instantiate(Resources.Load("Prefabs/Board") as GameObject, transform);


        obj.transform.position = new Vector3(scale * (c - size / 2) + spacing * (c - size / 2), 0,
            -(scale * (r - size / 2) + spacing * (r - size / 2)));
        

        Board b = (obj.GetComponent<Board>());
        b.SetObject(type);
        b.OnChanged += ReSetLocale;
        boards[r, c] = b;
    }
    public void AddBoard()
    {//PreBoard�� ������
        GameObject obj = Instantiate(Resources.Load("Prefabs/Board") as GameObject, transform);

        if (preBoard != null)
        {
            obj.transform.position = preBoard.transform.position;
            Destroy(preBoard);

            //�ִ� ĭ���� ǥ��
            aroundB[(int)(preBoardPos.x - pos.x + 1), (int)(pos.y - preBoardPos.y + 1)] = 1;
            Debug.Log((int)(preBoardPos.x - pos.x + 1) + " " + (int)(pos.y - preBoardPos.y + 1));

            Board b = (obj.GetComponent<Board>());
            //�� ����
            b.SetMap(centerLc.lat-globalLc.lat * (preBoardPos.y - size / 2), 
                centerLc.lng +globalLc.lng * (preBoardPos.x - size / 2));
            
            boards[(int)(preBoardPos.y), (int)(preBoardPos.x)] = b;
            Debug.Log((int)(preBoardPos.y) + " " + (int)(preBoardPos.x));
            preBoardPos.x = -1;
            preBoardPos.y = -1;

            //���� ������Ʈ ����
            //b.SetObject(type);

        }
    }
    public void ReSetLocale(Locale lc)
    {
        centerLc = lc;
        
        //���� ���� ������ ���� ����
        //���� ���� �ʱ�ȭ
    }
    public void DeleteBoard(int r, int c)
    {

    }
}
