using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ��Ϸ�������ű�
/// </summary>
public class GameManager : MonoBehaviour
{
    //����
    #region
    //��Ϸ������ֻ��Ҫʵ����һ�Σ���GameManager���ɵ���
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }
    #endregion

    //���ԣ�
    #region
    //������ĳ���
    public int xColumn;
    public int yRow;
    //��䶯��ʱ��
    public float fillTime;
    //������ʾʱ��
    public float gameTime = 60;
    private bool gameOver;
    //��ҵ÷�
    public int playerScore;
    //��ҵ÷ֶ���ʱ��
    private float addScoreTime;
    //��ǰ��ҷ���,�ۼӷ�����playerScore
    private float currentScore;
    #endregion

    //���ã�
    #region
    //��ȡ�ɿ�����С����Ԥ�Ƽ�
    public GameObject chocolateGridPrefab;
    //��Ʒ��ά���飨�����������������Ƕ�ά�ģ�
    //����GameSweet�������飨�����ű������õ�GameSweet���������õ���ǰ��Ϸ�����X��Yλ����������Ʒ����������
    //private GameObject[,] sweets:�õ�������Ʒ�������Ҫ�õ��������ԣ���Ҫ����sweets[x,y].getComponent�������ں����Ĳ�����ҪЩ�ܶ��getComponent����
    //private GameSweet[,] sweets; �õ�������Ʒ����Ļ�������
    private GameSweet[,] sweets;
    //Ҫ������������Ʒ����
    private GameSweet pressedSweet;//��갴�µ���Ʒ
    private GameSweet enterSweet;//����갴�µ���Ʒ���н�������Ʒ
    //��ϷUI��ʾ��
    public Text timeText;//ʱ��Text����
    public Text playerScoreText;//����Text����
    //��ȡ������������
    public GameObject gameOverPanel;
    //��ȡ��Ϸ�����÷�
    public Text gameOverScoreText;

    #endregion

    //��Ʒ���ࡢ�ֵ���ṹ��
    #region
    //��Ʒ������
    public enum SweetType
    {
        EMPTY,//������
        NORMAL,//��ͨ����
        BARRIER,//�ϰ�����
        ROW_CLEAR,//����������
        COLUMN_CLEAR,//����������
        RAINBOWCANDY,//�ʺ��ã�ȫ�����ͣ�
        COUNT//������ͣ����������õ�ʱ����ϸ��
    }
    //ͨ����Ʒ����õ���Ӧ��ƷԤ�Ƽ��������ֵ����ͣ���ʹʹ��publicҲ�޷�������ʾ��unity��inspector��壩
    public Dictionary<SweetType, GameObject> sweetPrefabDictionary;
    //Ҫ����Ʒ���ͺͶ�Ӧ����ƷԤ�Ƽ����浽�ֵ䣬����ʹ�ýṹ�壬��Ϊ�ṹ�������ʾ������У���ͨ�ṹ�����޷���ʾ������еģ���Ҫ����[System.Serializable]�����л���Ȼ�󴴽�һ���ṹ�����顪��������Ʒ����+��ƷԤ���塱��ΪԤ���屣�浽�ṹ�������У�Ȼ��ͨ����Ʒ��������Ʒ�ṹ���������ҵ���Ӧ�Ľṹ�壬�ٴӽṹ���л����ƷԤ�Ƽ�)
    //��Ʒ�ṹ��
    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType sweetType;
        public GameObject sweetPrefab;
    }
    //����ṹ������
    public SweetPrefab[] sweetPrefabs;
    #endregion

    //��Ϸ��ʼʱʵ������Ϸ����
    #region
    /// <summary>
    /// ��Ϸ��������Ҫ��������Ϸ����ʵ����ǰʵ����
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// ʵ������Ϸ����
    /// </summary>
    private void Start()
    {
        //ʵ�����ֵ�
        sweetPrefabDictionary = new Dictionary<SweetType, GameObject>();
        for(int i = 0; i < sweetPrefabs.Length; i++)
        {
            //����ӽṹ�������б��������Ľṹ��ġ���Ʒ���͡��������ڡ���Ʒ�ֵ䡱�У���ô���Ľṹ���ֵ���浽�ֵ���
            if (!sweetPrefabDictionary.ContainsKey(sweetPrefabs[i].sweetType))
            {
                //����Ʒ���ͺͶ�Ӧ��Ԥ�Ƽ���ΪԤ�Ƽ���ӵ���Ʒ�ֵ���
                sweetPrefabDictionary.Add(sweetPrefabs[i].sweetType, sweetPrefabs[i].sweetPrefab);
            }
        }

        //1��ʵ��������
        //2��ÿʵ����һ������Ϊ������ʵ����һ��������Ʒ����
        //3����ȡÿһ��������Ʒ���󣬽������������Ϣ���浽������Ϣ�ű�GameSweet��
        //4���ÿ���Ʒ�����������
        //5������AllFill�����������
        sweets = new GameSweet[xColumn, yRow];
        for (int x = 0; x < xColumn; x++)
        {
            for(int y = 0; y < yRow; y++)
            {
                //ʵ�����ɿ���С����
                GameObject chocolateGrid = Instantiate(chocolateGridPrefab, CorrectGridPosition(x, y), Quaternion.identity);
                //����Ϸ������ʵ�������������������Ϊ��Ϸ���������Ӷ���������Ϸ����Hierarchy���ܱ�������
                chocolateGrid.transform.SetParent(transform);
            }
        }

        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                CreateNewSweet(x, y, SweetType.EMPTY);
            }
        }

        //�������Ʒ֮ǰʵ���������ϰ�
        //���Ҫ���ɱ����ϰ�λ�õĿհ���Ʒ����
        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetType.BARRIER);


        //���������Ʒ����
        StartCoroutine(AllFill());
    }
    #endregion

    //Update��FixUpdate
    #region
    private void Update()
    {
        if (gameOver)
        {
            //�����Ϸ����������ķ���������ִ����
            return;
        }
        //����ʱ
        gameTime -= Time.deltaTime;
        if(gameTime <= 0)
        {
            gameTime = 0;
            //��ʾʧ����壬����ʧ�ܶ���
            gameOverPanel.SetActive(true);
            //��ʾ�÷�
            playerScoreText.text = playerScore.ToString();//����ҵ÷�ֱ����ʾ�ڵ÷ְ��ϣ�����������������Ҫʱ������
            gameOverScoreText.text = playerScore.ToString();
            gameOver = true;
            return;
        }
        //��ʾʱ��
        //ToString��ʾ��float����ת��ΪString���ͣ�����0��ʾС�������λ��Ϊ0��������С���㣬�����0.0��ʾ����һ��С����0.00��ʾ��������С��
        timeText.text = gameTime.ToString("0");
       
        //�ۼ�addScoreTimeʱ�䣬���ﵽ0.05ʱ���ҵ�ǰ����С����һ�õķ���ʱ����0.05s�ļ�����ӷ���
        if(addScoreTime <= 0.05f)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {

            if(currentScore < playerScore)
            {
                currentScore++;
                playerScoreText.text = currentScore.ToString();
                addScoreTime = 0;
            }
        } 
    }
    #endregion

    //������Ʒ���󷽷���������Ʒ���ɵ�����λ�÷���
    #region
    /// <summary>
    /// ������������λ��
    /// </summary>
    /// <param name="x">x����</param>
    /// <param name="y">y����</param>
    public Vector3 CorrectGridPosition(int x,int y)
    {
        //ʵ����Ҫʵ�����ɿ�����xλ�� = GameManagerλ��X���� - �����񳤶ȵ�һ�� + ���ж�Ӧ��X����
        //ʵ����Ҫʵ�����ɿ�����yλ�� = GameManagerλ��Y���� + ������߶ȵ�һ�� - ���ж�Ӧ��Y���꣨��Ʒ��������䣬������Ʒʵ����ʱ��������Ǵ������±����ģ�
        return new Vector3(transform.position.x - (xColumn / 2f) + x,
                            transform.position.y + (yRow / 2f) - y,
                            0);
    }

    /// <summary>
    /// ������Ʒ����
    /// </summary>
    /// <param name="x">x����</param>
    /// <param name="y">y����</param>
    /// <param name="type">��Ʒ����</param>
    /// <returns></returns>
    public GameSweet CreateNewSweet(int x,int y,SweetType type)
    {
        //����һ����Ʒ����������GameObject�������丸��������ΪGameManager
        GameObject newSweet = Instantiate(sweetPrefabDictionary[type], CorrectGridPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;

        //�������Ʒ���󱣴浽GameSweet��ά������
        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);

        return sweets[x, y];
    }
    #endregion

    //��Ʒ��䷽��
    #region
    /// <summary>
    /// ȫ����䷽��
    /// </summary>
    public IEnumerator AllFill()
    {
        bool needRefill = true;
        while (needRefill)
        {
            //�ȴ���һ��������
            yield return new WaitForSeconds(fillTime);

            //ִ�зֲ����
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //��������Ѿ�ƥ��õ���Ʒ
            needRefill = ClearAllMatchedSweet();
        }
    }

    /// <summary>
    /// �ֲ���䷽��
    /// </summary>
    /// <returns>���ز������ͣ���ʾ��ǰ����Ƿ���ɣ���������ڿ���Ʒ���󣬱�ʾ���ܽ�����䣬����true����֮����false</returns>
    public bool Fill()
    {
        //�жϱ�������Ƿ���ɣ����û��ɣ���false��Ϊtrue�������
        bool filledNotFinished = false;

        //���������������μ�С����һ��Ϊ0�У����һ��ΪyRow-1��
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                //�õ���ǰλ����Ʒ����
                GameSweet sweet = sweets[x, y];

                
                if (sweet.CanMove())
                {
                    //��������ƶ������ж��費��Ҫ�������
                    GameSweet sweetBelow = sweets[x, y + 1];

                    //��ֱ��䣺
                    if (sweetBelow.SweetType == SweetType.EMPTY)
                    {
                        //�����Ʒ�����ǿ���Ʒ����
                        //ɾ��Ҫ���λ�õĿ���Ʒ����
                        Destroy(sweetBelow.gameObject);
                        //�ı���Ʒλ�ã��������;������������Ʒ����Ϣ���µ���ά������
                        sweet.MoveComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        //�����ƶ�����ԭ��λ������һ������Ʒ����
                        CreateNewSweet(x, y, SweetType.EMPTY);
                        //���ڿ���Ʒ����˵�������δ���
                        filledNotFinished = true;
                    }
                    else
                    {
                        //б����䣺
                        //��ǰλ����Ʒ���󲻿����ƶ�
                        //-1�������£�0�������£�1�������£�ͨ����-1��1�������£����º���������λ��
                        for (int down = -1; down <= 1; down++)
                        {
                            //�ų����·���������·�����������if��������
                            if (down != 0)
                            {
                                int downX = x + down;

                                //�ų������е����º������е��������
                                if (downX >= 0 && downX < xColumn)
                                {
                                    //��ȡ���º�������Ϸ����
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.SweetType == SweetType.EMPTY)
                                    {
                                        //�����ȡ�������·���Ϸ����Ϊ�գ����жϸÿն����Ϸ��ǲ��ǲ����Զ����ϰ������ǣ�б�����
                                        bool canVerticalFill = true;//�ж��Ƿ���Դ�ֱ���
                                        //��Ȼ�˴���forѭ�������������if����������break�����Դ�forѭ��ֻ��ѭ��һ�Σ���һ�ξ��������·���Ϸ������Ϸ�����һ����Ϸ����
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbovw = sweets[downX, aboveY];
                                            if (sweetAbovw.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbovw.CanMove() && sweetAbovw.SweetType != SweetType.EMPTY)
                                            {
                                                //���ܴ�ֱ�����
                                                canVerticalFill = false;
                                                break;
                                            }
                                        }

                                        if (!canVerticalFill)
                                        {
                                            //ɾ��Ҫ���λ�õĿ���Ʒ����
                                            Destroy(downSweet.gameObject);
                                            //б�����
                                            sweet.MoveComponent.Move(downX, y + 1, fillTime);
                                            //����Ϣ���µ���Ʒ��ά����
                                            sweets[downX, y + 1] = sweet;
                                            //����ճ�����ԭ��Ϊֹ�ÿ���Ʒ�������
                                            CreateNewSweet(x, y, SweetType.EMPTY);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
        }

        //�����������������һ��Ϊ����Ʒ����ʱ����Ҫ�Ӳ��ɼ���-1�����������Ʒ���䵽��һ�У�
        for (int x = 0; x < xColumn; x++)
        {
            //�õ���һ�е���Ʒ����
            GameSweet sweet = sweets[x, 0];
            if (sweet.SweetType == SweetType.EMPTY)
            {
                //�ڵ�һ������ʵ����һ����Ʒ����
                GameObject newSweet = Instantiate(sweetPrefabDictionary[SweetType.NORMAL], CorrectGridPosition(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;

                //��ʵ��������Ʒ������µ�����ά�����һ��λ�ã��������ƶ�����һ�У�����ı�����ɫ�����ࣩ
                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetType.NORMAL);
                sweets[x, 0].MoveComponent.Move(x, 0, fillTime);
                sweets[x, 0].ColorComponent.SetColor((ColorSweet.ColorType)(Random.Range(0, sweets[x, 0].ColorComponent.NumColors)));
                //��һ�л�����䣬��û��������
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }
    #endregion

    //��Ʒ��������
    #region
    /// <summary>
    /// �ж���Ʒ�Ƿ����ڵķ���
    /// </summary>
    /// <returns>����true��ʾ��������</returns>
    private bool isFriend(GameSweet sweet1,GameSweet sweet2)
    {
        //X�������ʱY�������1��Y�������ʱX�������1����������
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    /// <summary>
    /// ������Ʒλ�÷���
    /// </summary>
    /// <param name="sweet1"></param>
    /// <param name="sweet2"></param>
    private void ExchangeSweets(GameSweet sweet1,GameSweet sweet2)
    {
        //�ж��Ƿ���ƶ�
        if(sweet1.CanMove() && sweet2.CanMove())
        {
            //���¶�ά������Ϣ
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            //ע�⣺��Ʒ1������Ʒ2��λ�ý�����Ʒ1�б�����λ��Ҫ����Ʒ2��λ�ã���Ϊ����ȷ���ܲ��ܽ������ܽ����ŻὫ��Ʒ2��λ�ø��µ���Ʒ1��λ��
            if(MatchSweets(sweet1,sweet2.X,sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null || sweet1.SweetType == SweetType.RAINBOWCANDY || sweet2.SweetType == SweetType.RAINBOWCANDY)
            {
                //���ƥ�䷽�����ص���������б�Ϊ�գ�˵�����Խ���ƥ������
                int tempX = sweet1.X;
                int tempY = sweet1.Y;
                //��λ��
                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.MoveComponent.Move(tempX, tempY, fillTime);

                //�ж�������������Ʒ��û�вʺ���
                if(sweet1.SweetType == SweetType.RAINBOWCANDY && sweet1.CanClear() && sweet2.CanClear())
                {
                    ClearColor clearColor = sweet1.GetComponent<ClearColor>();
                    if(clearColor != null)
                    {
                        clearColor.ClearColorSweet = sweet2.ColorComponent.Color;
                    }
                    ClearSweet(sweet1.X, sweet1.Y);
                }
                if (sweet2.SweetType == SweetType.RAINBOWCANDY && sweet2.CanColor() && sweet1.CanColor())
                {
                    ClearColor clearColor = sweet2.GetComponent<ClearColor>();
                    if (clearColor != null)
                    {
                        clearColor.ClearColorSweet = sweet1.ColorComponent.Color;
                    }
                    ClearSweet(sweet2.X, sweet2.Y);
                }

                //��ΪMatchSweets����ֵ��Ϊnull��˵�����ڿ��������Ķ��󣬵�����������
                ClearAllMatchedSweet();
                //�����Ʒ���������Э������λ
                StartCoroutine(AllFill());
            }
            else
            {
                //���ܽ���ƥ�䣬��ԭ��ά�����е���Ʒλ��
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
            
        }
    }
    #endregion

    //��������ק������Ʒ��������
    #region
    /// <summary>
    /// ������Ʒ�ķ���
    /// </summary>
    /// <param name="sweet">��갴�µ���Ʒ����</param>
    public void PressSweet(GameSweet sweet)
    {
        //��Ϸ�������������
        if (gameOver)
        {
            return;
        }
        pressedSweet = sweet;
    }

    /// <summary>
    /// ������Ʒ�ķ�������갴����Ʒ�������һ����Ʒ���н���
    /// </summary>
    /// <param name="sweet">����������ڵ���Ʒ����</param>
    public void EnterSweet(GameSweet sweet)
    {
        //��Ϸ�������������
        if (gameOver)
        {
            return;
        }
        enterSweet = sweet;
    }

    /// <summary>
    /// �ɿ���귽�����ж��Ƿ����ڶ����ǵĻ��������滻
    /// </summary>
    public void ReleaseSweet()
    {
        //��Ϸ�������������
        if (gameOver)
        {
            return;
        }
        if (isFriend(pressedSweet, enterSweet))
        {
            ExchangeSweets(pressedSweet, enterSweet);
        }
    }
    #endregion

    //ƥ�������Ʒ����
    #region
    /// <summary>
    /// ��Ʒƥ�䷽��
    /// </summary>
    /// <param name="sweet">Ҫ����ƥ�����Ʒ����</param>
    /// <param name="newX">�ö��󽻻�λ�ú����X����</param>
    /// <param name="newY">�Ķ��󽻻�λ�ú����Y����</param>
    /// <returns></returns>
    public List<GameSweet> MatchSweets(GameSweet sweet,int newX,int newY)
    {
        if (sweet.CanColor())
        {
            //��ȡ��ǰ��Ϸ�������ɫ�����ͣ�������ƥ��������ߺ��ұ��ǲ�����ͬ��ɫ�����ͣ���Ʒ
            ColorSweet.ColorType color = sweet.ColorComponent.Color;

            //�б����б��б����б����ƥ���б�
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();

            //��ƥ��
            matchRowSweets.Add(sweet);

            //iΪ0��ʾ���������iΪ1��ʾ���ұ���
            for(int i = 0; i <= 1; i++)
            {
                for(int xDistance = 1; xDistance < xColumn; xDistance++)
                {
                    int x;
                    if(i == 0)
                    {
                        //�������
                        x = newX - xDistance;
                    }
                    else
                    {
                        //���ұ���
                        x = newX + xDistance;
                    }
                    //����߽�
                    if(x < 0 || x >= xColumn)
                    {
                        break;
                    }
                    
                    if(sweets[x,newY].CanColor() && sweets[x,newY].ColorComponent.Color == color)
                    {
                        //�����������������Ʒʱͬ��ɫ��ͬ���ͣ�����ô������ӵ������б�����
                        matchRowSweets.Add(sweets[x,newY]);
                    }
                    else
                    {
                        //�������������Ʒ��ɫ�����ͣ���һ�£���ô�˴�ƥ����ֹ
                        break;
                    }
                }
            }

            if(matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }
            

            //LT��ƥ�䣺
            //�����ߵ����˵��������ƥ�䶼������ɣ�����б����б����е�Ԫ�������Ƿ����3������ǣ�������ӵ������ƥ�䡱�б���
            if(matchRowSweets.Count >= 3)
            {
                for(int i = 0; i < matchRowSweets.Count; i++)
                {
                    //��ƥ���б�������ƥ���ÿ��Ԫ��һ�ν����б���
                    //0�����Ϸ�������1�����·�����
                    for(int j = 0; j <= 1; j++)
                    {
                        for(int yDistance = 1; yDistance < yRow; yDistance++)
                        {
                            int y;
                            if(j == 0)
                            {
                                y = newY - yDistance;
                            }
                            else
                            {
                                y = newY + yDistance;
                            }
                            if(y < 0 || y >= yRow)
                            {
                                break;
                            }
                            if (sweets[matchRowSweets[i].X, y].CanColor() && sweets[matchRowSweets[i].X, y].ColorComponent.Color == color)
                            {
                                matchLineSweets.Add(sweets[matchRowSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //�Ե�ǰ��Ԫ�ؽ�����ƥ����ж������Ƿ�С��2�����С��2��������(û�а�������)
                    if(matchLineSweets.Count < 2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for(int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchLineSweets[j]);
                        }
                        //TL��ƥ����ɣ�������ѭ���жϱ����Ϸ������
                        break;
                    }
                }
            }

            //�ж���ƥ���б��Ƿ����3�����ǵĻ�˵����ƥ��ɹ������سɹ�����ƥ���б�
            if(finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
            //�����ƥ��֮�����ƥ�����б�����С��3����ô����ƥ���б���ƥ���б�����ƥ���б���գ�����Ԫ�ص���ƥ��
            matchRowSweets.Clear();
            matchLineSweets.Clear();
            finishedMatchingSweets.Clear();



            //��ƥ��
            matchLineSweets.Add(sweet);

            //iΪ0��ʾ���������iΪ1��ʾ���ұ���
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        //�������
                        y = newY - yDistance;
                    }
                    else
                    {
                        //���ұ���
                        y = newY + yDistance;
                    }
                    //����߽�
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }

                    if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == color)
                    {
                        //�����������������Ʒʱͬ��ɫ��ͬ���ͣ�����ô������ӵ������б�����
                        matchLineSweets.Add(sweets[newX, y]);
                    }
                    else
                    {
                        //�������������Ʒ��ɫ�����ͣ���һ�£���ô�˴�ƥ����ֹ
                        break;
                    }
                }
            }

            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchLineSweets[i]);
                }
            }

            //LT��ƥ�䣺
            //�����ߵ����˵��������ƥ�䶼������ɣ�����б����б����е�Ԫ�������Ƿ����3������ǣ�������ӵ������ƥ�䡱�б���
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //��ƥ���б�������ƥ���ÿ��Ԫ��һ�ν����б���
                    //0�����Ϸ�������1�����·�����
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < xColumn; xDistance++)
                        {
                            int x;
                            if (j == 0)
                            {
                                x = newX - xDistance;
                            }
                            else
                            {
                                x = newX + xDistance;
                            }
                            if (x < 0 || x >= xColumn)
                            {
                                break;
                            }
                            if (sweets[x, matchLineSweets[i].Y].CanColor() && sweets[x, matchLineSweets[i].Y].ColorComponent.Color == color)
                            {
                                matchRowSweets.Add(sweets[x, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //�Ե�ǰ��Ԫ�ؽ�����ƥ����ж������Ƿ�С��2�����С��2��������(û�а�������)
                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchRowSweets[j]);
                        }
                        //TL��ƥ����ɣ�������ѭ���жϱ����Ϸ������
                        break;
                    }
                }
            }

            //�ж��б����Ƿ���ɣ������ɵ��˽��������û��ɽ����б���
            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }

        //�������ƥ�䶼������
        return null;
    }
    
    /// <summary>
    /// �����Ʒ����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ClearSweet(int x,int y)
    {
        //�����ǰ��Ʒ����������Ҳ������״̬������Э�̶���������
        if (sweets[x, y].CanClear() && !sweets[x,y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            //��������λ�ô���һ������Ʒ����
            CreateNewSweet(x, y, SweetType.EMPTY);
            //���ñ�������������ж��������Ʒ��Χ��û�б���
            ClearBarrier(x, y);
            return true;
        }

        return false;
    }

    /// <summary>
    /// ����з���
    /// </summary>
    /// <param name="row">Ҫ�������</param>
    public void ClearRow(int row)
    {
        for(int x = 0; x < xColumn; x++)
        {
            ClearSweet(x, row);
        }
    }

    /// <summary>
    /// ����з���
    /// </summary>
    /// <param name="column">Ҫ�������</param>
    public void ClearColumn(int column)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(column, y);
        }
    }

    /// <summary>
    /// ���ͬ����ɫ����
    /// </summary>
    /// <param name="color">Ҫ�������ɫ����</param>
    public void ClearColor(ColorSweet.ColorType color)
    {
        for(int x = 0; x < xColumn; x++)
        {
            for(int y = 0; y < yRow; y++)
            {
                //���������ʺ���ʱ���������Ʒ����
                if(sweets[x,y].CanColor() && (sweets[x,y].ColorComponent.Color == color || ColorSweet.ColorType.COLORS == color))
                {
                    ClearSweet(x, y);
                }
            }
        }
    }

    /// <summary>
    /// �������ɵķ���
    /// ����������������Ʒ�����꣬��������Ʒ��������������Ƿ��б��ɶ����еĻ���������
    /// </summary>
    /// <param name="x">������������Ʒ��x����</param>
    /// <param name="y">������������Ʒ��y����</param>
    private void ClearBarrier(int x,int y)
    {
        //����������������Ʒ����ߺ��ұ�
        for(int friendX = x - 1; friendX <= x + 1; friendX++)
        {
            //�ų�������߽�
            if(friendX != x && friendX >= 0 && friendX < xColumn)
            {
                //�������Ϊ���ɲ��ҿ�����������
                if (sweets[friendX, y].SweetType == SweetType.BARRIER && sweets[friendX, y].CanClear())
                {
                    sweets[friendX, y].ClearComponent.Clear();
                    CreateNewSweet(friendX,y,SweetType.EMPTY);
                }
            }
        }

        //����������������Ʒ���ϱߺ��±�
        for (int friendY = y - 1; friendY <= y + 1; friendY++)
        {
            //�ų�������߽�
            if (friendY != y && friendY >= 0 && friendY < yRow)
            {
                //�������Ϊ���ɲ��ҿ�����������
                if (sweets[x, friendY].SweetType == SweetType.BARRIER && sweets[x, friendY].CanClear())
                {
                    sweets[x, friendY].ClearComponent.Clear();
                    CreateNewSweet(x, friendY, SweetType.EMPTY);
                }
            }
        }
    }

    /// <summary>
    /// �������ƥ��õ���Ʒ
    /// </summary>
    /// <returns></returns>
    private bool ClearAllMatchedSweet()
    {
        //�������Ʒ֮������Ҫ��������
        bool needRefill = false;

        //�����������������������Ʒ����
        for(int y = 0; y < yRow; y++)
        {
            for(int x = 0; x < xColumn; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<GameSweet> matchList = MatchSweets(sweets[x, y], x, y);
                    if(matchList != null)
                    {
                        //��������������Ʒ
                        //������λ
                        SweetType specialSweetsType = SweetType.COUNT;//��������Ƿ����������Ʒ���������������Ʒ���������͸�ֵ��COUNT���ͣ��൱��һ���м����temp��
                        GameSweet randomSweet = matchList[Random.Range(0, matchList.Count)];//�����ȡ������һ����Ʒ���󣬽�����ȡ�Ķ����λ��
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if(matchList.Count == 4)
                        {
                            specialSweetsType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.COLUMN_CLEAR);
                        }
                        else if(matchList.Count >= 5)
                        {
                            //�����ʺ���
                            specialSweetsType = SweetType.RAINBOWCANDY;
                        }


                        //�����Ʒ
                        for(int i = 0; i < matchList.Count; i++)
                        {
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }

                        if(specialSweetsType != SweetType.COUNT)
                        {
                            //���specialSweetsType��ΪCOUNT��˵��������������һ��
                            Destroy(sweets[specialSweetX, specialSweetY]);
                            GameSweet newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetsType);
                            //�ж����Ͳ����а�ȫУ��
                            if((specialSweetsType == SweetType.ROW_CLEAR || specialSweetsType == SweetType.COLUMN_CLEAR) && newSweet.CanColor() && matchList[0].CanColor())
                            {
                                //����������������ɫ�����ͣ�
                                newSweet.ColorComponent.SetColor(matchList[0].ColorComponent.Color);
                            }
                            else if(specialSweetsType == SweetType.RAINBOWCANDY && newSweet.CanColor())
                            {
                                //���òʺ�������
                                newSweet.ColorComponent.SetColor(ColorSweet.ColorType.COLORS);
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
    
    #endregion

    //��Ϸ�������淽��
    #region
    /// <summary>
    /// ���ز˵�����
    /// </summary>
    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// ���淽��
    /// </summary>
    public void RePlay()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
