using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA_Dialog;

public class BoardManager : MonoBehaviour
{
    private Vector2 pos;
    public Vector2 Pos
    {
        //get { return pos; }
        set { 
            pos = value;
            GameManager.Instance.Pos = pos;
        }
    }

    public Board[,] boards;
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
        Pos = new Vector2(0, 0);

        centerLc = new Locale("", 0, 0);
        //���� �浵 �Ÿ�
        globalLc = new Locale("", 0.00272f, 0.00345f);

        preBoardPos = new Vector2(-1, -1);//�ʱ�ȭ
        boards = new Board[size, size];
        aroundB = new int[3, 3];

        dirs = new Vector2[4];
        dirs[0] = Vector2.up;
        dirs[1] = Vector2.down;
        dirs[2] = Vector2.left;
        dirs[3] = Vector2.right;

        AddBoard(size / 2, size / 2);
        SetCurrentPos(new Vector2(size / 2, size / 2));
        Player.Instance.OnChangedPos += SetCurrentPos;
    }
    public void Reset()
    {
        for (int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                if(boards[i, j] != null)
                {
                    boards[i, j].Destroy();
                }
            }
        }
        for (int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                aroundB[i, j] = 0;
            }
        }
        Pos = new Vector2(0, 0);

        centerLc.lat = 0;
        centerLc.lng = 0;

        preBoardPos.x = -1;
        preBoardPos.y = -1;

        dirs[0] = Vector2.up;
        dirs[1] = Vector2.down;
        dirs[2] = Vector2.left;
        dirs[3] = Vector2.right;

        AddBoard(size / 2, size / 2);
        SetCurrentPos(new Vector2(size / 2, size / 2));
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
            float min = Vector3.Distance(Vector3.forward, dir_p);
            Vector3 dir = Vector3.forward;

            if (min >= Vector3.Distance(Vector3.back, dir_p))
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
            float target = plus * Vector3.Dot(boards[(int)pos.y, (int)pos.x].transform.position, dir) +
                ((scale / 2) + spacing) * plus;
            float current = plus * Vector3.Dot(Player.Instance.transform.position, dir);

            if (state == 0 && Mathf.Abs(target - current) <= spacing + targetSpace)
            {//�ش� ���� ���尡 ��ĭ�̶�� && Ư�� �Ÿ� ���̶��
                CreatePreBoard((int)(-dir.z + pos.y), (int)(dir.x + pos.x));
            }
            else if (state == 1)
            {//�ִٸ� 
                //�ش� ����� �Ѿ���� üũ

            }
            else
            {
                if (preBoard != null)
                {//�ʱ�ȭ��������
                    Destroy(preBoard);
                    preBoardPos.x = -1;
                    preBoardPos.y = -1;
                }
            }


        }


    }
    public void SetCurrentPos(Vector2 v)
    {
        Pos = v;
        int r = (int)v.y;
        int c = (int)v.x;

        if (r < 0 || c < 0 || r >= size || c >= size)
        {//���� ���� �����Ϸ��� �� �� 
            return;
        }

        int newr = r;
        int newc = c;

        foreach (Vector2 d in dirs)
        {           //�ֺ� �װ� ��� ����
                    //����ĭ�̶�� ���� -1
                    //�̹� �ִ� ĭ�̶�� ǥ�� 1
                    //��ĭ�̶�� 0
            newr = r - (int)d.y;
            newc = c + (int)d.x;


            if (newr < 0 || newr >= size || newc < 0 || newc >= size)
            {
                aroundB[(int)d.x + 1, (int)d.y + 1] = -1;
            }
            else if (boards[newr, newc] != null)
            {
                aroundB[(int)d.x + 1, (int)d.y + 1] = 1;
            }
            else
            {
                aroundB[(int)d.x + 1, (int)d.y + 1] = 0;
            }
            //Debug.Log("����" + ((int)d.x + 1) + " " + ((int)d.y + 1) + "=" + aroundB[(int)d.x + 1, (int)d.y + 1]
            //    +"��ǥ"+ newr+" "+ newc);
        }

    }
    public void CreatePreBoard(int r, int c)
    {
        if (GameManager.Instance.isEnemyMode == true) return; //�ο����̶�� ����
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
    public void AddBoard(int r, int c, ScanObject.Type type = 0)
    {//Ư�� ��ġ�� ���� ������ ��, ex) ���� ����
        GameObject obj = Instantiate(Resources.Load("Prefabs/Board") as GameObject, transform);


        obj.transform.position = new Vector3(scale * (c - size / 2) + spacing * (c - size / 2), 0,
            -(scale * (r - size / 2) + spacing * (r - size / 2)));


        Board b = (obj.GetComponent<Board>());
        b.SetObject(type);
        b.OnChanged += ReSetLocale;


        //����κ�
        b.Pos = new Vector2(c, r);
        boards[r, c] = b;
        GameManager.Instance.BoardCnt += 1;
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
            
            b.SetMap(centerLc.lat - globalLc.lat * (preBoardPos.y - size / 2),
                centerLc.lng + globalLc.lng * (preBoardPos.x - size / 2));


            //���� �κ�
            b.Pos = new Vector2((int)(preBoardPos.x), (int)(preBoardPos.y));
            boards[(int)(preBoardPos.y), (int)(preBoardPos.x)] = b;
            GameManager.Instance.BoardCnt += 1;
            b.SetObject((ScanObject.Type)Util.Choose(new float[] {0f,5f,10f,0f }));



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
