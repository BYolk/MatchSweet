using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器脚本
/// </summary>
public class GameManager : MonoBehaviour
{
    //单例
    #region
    //游戏管理器只需要实例化一次，将GameManager做成单例
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

    //属性：
    #region
    //大网格的长宽
    public int xColumn;
    public int yRow;
    //填充动画时间
    public float fillTime;
    //用于显示时间
    public float gameTime = 60;
    private bool gameOver;
    //玩家得分
    public int playerScore;
    //玩家得分动画时间
    private float addScoreTime;
    //当前玩家分数,累加分数到playerScore
    private float currentScore;
    #endregion

    //引用：
    #region
    //获取巧克力块小网格预制件
    public GameObject chocolateGridPrefab;
    //甜品二维数组（用来填满网格，网格是二维的）
    //创建GameSweet对象数组（基础脚本），拿到GameSweet对象后就能拿到当前游戏对象的X，Y位置属性与甜品的类型属性
    //private GameObject[,] sweets:拿到的是甜品对象，如果要拿到对象属性，需要进行sweets[x,y].getComponent，这样在后续的操作需要些很多的getComponent方法
    //private GameSweet[,] sweets; 拿到的是甜品对象的基本属性
    private GameSweet[,] sweets;
    //要交换的两个甜品对象
    private GameSweet pressedSweet;//鼠标按下的甜品
    private GameSweet enterSweet;//与鼠标按下的甜品进行交换的甜品
    //游戏UI显示：
    public Text timeText;//时间Text引用
    public Text playerScoreText;//分数Text引用
    //获取结束界面引用
    public GameObject gameOverPanel;
    //获取游戏结束得分
    public Text gameOverScoreText;

    #endregion

    //甜品种类、字典与结构体
    #region
    //甜品的种类
    public enum SweetType
    {
        EMPTY,//空类型
        NORMAL,//普通类型
        BARRIER,//障碍类型
        ROW_CLEAR,//行消除类型
        COLUMN_CLEAR,//列消除类型
        RAINBOWCANDY,//彩虹堂（全消类型）
        COUNT//标记类型：具体作用用到时候详细讲
    }
    //通过甜品种类得到对应甜品预制件（对于字典类型，即使使用public也无法将其显示在unity的inspector面板）
    public Dictionary<SweetType, GameObject> sweetPrefabDictionary;
    //要将甜品类型和对应的甜品预制件保存到字典，可以使用结构体，因为结构体可以显示在面板中（普通结构体是无法显示在面板中的，需要加上[System.Serializable]可序列化，然后创建一个结构体数组——将“甜品类型+甜品预制体”作为预制体保存到结构体数组中，然后通过甜品类型在甜品结构体数组总找到对应的结构体，再从结构体中获得甜品预制件)
    //甜品结构体
    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType sweetType;
        public GameObject sweetPrefab;
    }
    //定义结构体数组
    public SweetPrefab[] sweetPrefabs;
    #endregion

    //游戏开始时实例化游戏对象
    #region
    /// <summary>
    /// 游戏管理器需要在所有游戏对象实例化前实例化
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 实例化游戏对象
    /// </summary>
    private void Start()
    {
        //实例化字典
        sweetPrefabDictionary = new Dictionary<SweetType, GameObject>();
        for(int i = 0; i < sweetPrefabs.Length; i++)
        {
            //如果从结构体数组中遍历出来的结构体的“甜品类型”不包含在“甜品字典”中，那么将改结构体的值保存到字典内
            if (!sweetPrefabDictionary.ContainsKey(sweetPrefabs[i].sweetType))
            {
                //将甜品类型和对应的预制件作为预制件添加到甜品字典内
                sweetPrefabDictionary.Add(sweetPrefabs[i].sweetType, sweetPrefabs[i].sweetPrefab);
            }
        }

        //1、实例化网格
        //2、每实例化一个网格，为改网格实例化一个基本甜品对象
        //3、获取每一个基本甜品对象，将其基本属性信息保存到基本信息脚本GameSweet中
        //4、用空甜品对象填充网格
        //5、调用AllFill方法填充网格
        sweets = new GameSweet[xColumn, yRow];
        for (int x = 0; x < xColumn; x++)
        {
            for(int y = 0; y < yRow; y++)
            {
                //实例化巧克力小网格
                GameObject chocolateGrid = Instantiate(chocolateGridPrefab, CorrectGridPosition(x, y), Quaternion.identity);
                //将游戏管理器实例化出来的网格对象都作为游戏管理器的子对象，这样游戏运行Hierarchy才能保持整洁
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

        //在填充甜品之前实例化饼干障碍
        //清楚要生成饼干障碍位置的空白甜品对象
        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetType.BARRIER);


        //调用填充甜品方法
        StartCoroutine(AllFill());
    }
    #endregion

    //Update与FixUpdate
    #region
    private void Update()
    {
        if (gameOver)
        {
            //如果游戏结束，后面的方法都不用执行了
            return;
        }
        //倒计时
        gameTime -= Time.deltaTime;
        if(gameTime <= 0)
        {
            gameTime = 0;
            //显示失败面板，播放失败动画
            gameOverPanel.SetActive(true);
            //显示得分
            playerScoreText.text = playerScore.ToString();//将玩家得分直接显示在得分板上，不再像下面那样需要时间增加
            gameOverScoreText.text = playerScore.ToString();
            gameOver = true;
            return;
        }
        //显示时间
        //ToString表示将float类型转化为String类型，参数0表示小数点后面位数为0，即不带小数点，如果是0.0表示保留一个小数，0.00表示保留两个小数
        timeText.text = gameTime.ToString("0");
       
        //累加addScoreTime时间，当达到0.05时并且当前分数小于玩家获得的分数时，以0.05s的间隔增加分数
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

    //生成甜品对象方法并修正甜品生成的网格位置方法
    #region
    /// <summary>
    /// 修正网格生成位置
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    public Vector3 CorrectGridPosition(int x,int y)
    {
        //实际需要实例化巧克力的x位置 = GameManager位置X坐标 - 大网格长度的一半 + 行列对应的X坐标
        //实际需要实例化巧克力的y位置 = GameManager位置Y坐标 + 大网格高度的一半 - 行列对应的Y坐标（甜品从上面掉落，所以甜品实例化时遍历表格是从上往下遍历的）
        return new Vector3(transform.position.x - (xColumn / 2f) + x,
                            transform.position.y + (yRow / 2f) - y,
                            0);
    }

    /// <summary>
    /// 生成甜品对象
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <param name="type">甜品类型</param>
    /// <returns></returns>
    public GameSweet CreateNewSweet(int x,int y,SweetType type)
    {
        //创建一个甜品对象，类型是GameObject，并将其父物体设置为GameManager
        GameObject newSweet = Instantiate(sweetPrefabDictionary[type], CorrectGridPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;

        //将这个甜品对象保存到GameSweet二维数组内
        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);

        return sweets[x, y];
    }
    #endregion

    //甜品填充方法
    #region
    /// <summary>
    /// 全部填充方法
    /// </summary>
    public IEnumerator AllFill()
    {
        bool needRefill = true;
        while (needRefill)
        {
            //等待上一次填充完成
            yield return new WaitForSeconds(fillTime);

            //执行分布填充
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //清除所有已经匹配好的甜品
            needRefill = ClearAllMatchedSweet();
        }
    }

    /// <summary>
    /// 分步填充方法
    /// </summary>
    /// <returns>返回布尔类型，表示当前填充是否完成，如果还存在空甜品对象，表示还能进行填充，返回true，反之返回false</returns>
    public bool Fill()
    {
        //判断本次填充是否完成，如果没完成，将false改为true继续填充
        bool filledNotFinished = false;

        //行数从下往上依次减小，第一行为0行，最后一行为yRow-1行
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                //得到当前位置甜品对象
                GameSweet sweet = sweets[x, y];

                
                if (sweet.CanMove())
                {
                    //如果可以移动，再判断需不需要向下填充
                    GameSweet sweetBelow = sweets[x, y + 1];

                    //垂直填充：
                    if (sweetBelow.SweetType == SweetType.EMPTY)
                    {
                        //如果甜品下面是空甜品对象：
                        //删除要填充位置的空甜品对象
                        Destroy(sweetBelow.gameObject);
                        //改变甜品位置，往下填充;将往下填充的甜品的信息更新到二维数组内
                        sweet.MoveComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        //往下移动后，在原本位置生成一个空甜品对象
                        CreateNewSweet(x, y, SweetType.EMPTY);
                        //存在空甜品对象，说明填充尚未完成
                        filledNotFinished = true;
                    }
                    else
                    {
                        //斜向填充：
                        //当前位置甜品对象不可以移动
                        //-1代表左下，0代表整下，1代表右下，通过从-1到1遍历左下，整下和右下三个位置
                        for (int down = -1; down <= 1; down++)
                        {
                            //排除正下方情况，正下方情况在上面的if条件处理
                            if (down != 0)
                            {
                                int downX = x + down;

                                //排除最左列的左下和最右列的右下情况
                                if (downX >= 0 && downX < xColumn)
                                {
                                    //获取左下和右下游戏对象
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.SweetType == SweetType.EMPTY)
                                    {
                                        //如果获取的左右下方游戏对象为空，在判断该空对象上方是不是不可以动的障碍物，如果是，斜向填充
                                        bool canVerticalFill = true;//判断是否可以垂直填充
                                        //虽然此处是for循环，但是里面的if条件都存在break，所以此for循环只会循环一次，这一次就是左右下方游戏对象的上方的那一个游戏对象
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbovw = sweets[downX, aboveY];
                                            if (sweetAbovw.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbovw.CanMove() && sweetAbovw.SweetType != SweetType.EMPTY)
                                            {
                                                //不能垂直填充了
                                                canVerticalFill = false;
                                                break;
                                            }
                                        }

                                        if (!canVerticalFill)
                                        {
                                            //删除要填充位置的空甜品对象
                                            Destroy(downSweet.gameObject);
                                            //斜向填充
                                            sweet.MoveComponent.Move(downX, y + 1, fillTime);
                                            //将信息更新到甜品二维数组
                                            sweets[downX, y + 1] = sweet;
                                            //填充后空出来的原本为止用空甜品对象填充
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

        //最上排特殊情况（第一排为空甜品对象时，需要从不可见的-1行随机生成甜品补充到第一行）
        for (int x = 0; x < xColumn; x++)
        {
            //拿到第一行的甜品对象
            GameSweet sweet = sweets[x, 0];
            if (sweet.SweetType == SweetType.EMPTY)
            {
                //在第一行上面实例化一个甜品对象
                GameObject newSweet = Instantiate(sweetPrefabDictionary[SweetType.NORMAL], CorrectGridPosition(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;

                //将实例化的甜品对象更新到到二维数组第一行位置，并将其移动到第一行，随机改变其颜色（种类）
                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetType.NORMAL);
                sweets[x, 0].MoveComponent.Move(x, 0, fillTime);
                sweets[x, 0].ColorComponent.SetColor((ColorSweet.ColorType)(Random.Range(0, sweets[x, 0].ColorComponent.NumColors)));
                //第一行还在填充，还没有填充完毕
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }
    #endregion

    //甜品交换方法
    #region
    /// <summary>
    /// 判断甜品是否相邻的方法
    /// </summary>
    /// <returns>返回true表示两者相邻</returns>
    private bool isFriend(GameSweet sweet1,GameSweet sweet2)
    {
        //X坐标相等时Y坐标相差1或Y坐标相等时X坐标相差1则两者相邻
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    /// <summary>
    /// 更换甜品位置方法
    /// </summary>
    /// <param name="sweet1"></param>
    /// <param name="sweet2"></param>
    private void ExchangeSweets(GameSweet sweet1,GameSweet sweet2)
    {
        //判断是否可移动
        if(sweet1.CanMove() && sweet2.CanMove())
        {
            //更新二维数组信息
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            //注意：甜品1换到甜品2的位置进行甜品1行遍历，位置要填甜品2的位置，因为还不确定能不能交换，能交换才会将甜品2的位置更新到甜品1的位置
            if(MatchSweets(sweet1,sweet2.X,sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null || sweet1.SweetType == SweetType.RAINBOWCANDY || sweet2.SweetType == SweetType.RAINBOWCANDY)
            {
                //如果匹配方法返回的完成消除列表不为空，说明可以进行匹配消除
                int tempX = sweet1.X;
                int tempY = sweet1.Y;
                //换位置
                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.MoveComponent.Move(tempX, tempY, fillTime);

                //判断两个交换的甜品有没有彩虹糖
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

                //因为MatchSweets返回值不为null，说明存在可以消除的对象，调用消除方法
                ClearAllMatchedSweet();
                //清除甜品后启动填充协程填充空位
                StartCoroutine(AllFill());
            }
            else
            {
                //不能进行匹配，还原二维数组中的甜品位置
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
            
        }
    }
    #endregion

    //玩家鼠标拖拽操作甜品交换方法
    #region
    /// <summary>
    /// 按下甜品的方法
    /// </summary>
    /// <param name="sweet">鼠标按下的甜品对象</param>
    public void PressSweet(GameSweet sweet)
    {
        //游戏结束，禁用鼠标
        if (gameOver)
        {
            return;
        }
        pressedSweet = sweet;
    }

    /// <summary>
    /// 进入甜品的方法，鼠标按下甜品后进入另一个甜品进行交换
    /// </summary>
    /// <param name="sweet">鼠标进入的相邻的甜品对象</param>
    public void EnterSweet(GameSweet sweet)
    {
        //游戏结束，禁用鼠标
        if (gameOver)
        {
            return;
        }
        enterSweet = sweet;
    }

    /// <summary>
    /// 松开鼠标方法，判断是否相邻对象，是的话并进行替换
    /// </summary>
    public void ReleaseSweet()
    {
        //游戏结束，禁用鼠标
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

    //匹配清除甜品方法
    #region
    /// <summary>
    /// 甜品匹配方法
    /// </summary>
    /// <param name="sweet">要进行匹配的甜品对象</param>
    /// <param name="newX">该对象交换位置后的新X坐标</param>
    /// <param name="newY">改对象交换位置后的新Y坐标</param>
    /// <returns></returns>
    public List<GameSweet> MatchSweets(GameSweet sweet,int newX,int newY)
    {
        if (sweet.CanColor())
        {
            //获取当前游戏对象的颜色（类型），用于匹配它的左边和右边是不是相同颜色（类型）甜品
            ColorSweet.ColorType color = sweet.ColorComponent.Color;

            //行遍历列表，列遍历列表，完成匹配列表
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();

            //行匹配
            matchRowSweets.Add(sweet);

            //i为0表示往左遍历，i为1表示往右遍历
            for(int i = 0; i <= 1; i++)
            {
                for(int xDistance = 1; xDistance < xColumn; xDistance++)
                {
                    int x;
                    if(i == 0)
                    {
                        //往左遍历
                        x = newX - xDistance;
                    }
                    else
                    {
                        //往右遍历
                        x = newX + xDistance;
                    }
                    //到达边界
                    if(x < 0 || x >= xColumn)
                    {
                        break;
                    }
                    
                    if(sweets[x,newY].CanColor() && sweets[x,newY].ColorComponent.Color == color)
                    {
                        //如果遍历到的相邻甜品时同颜色（同类型），那么将其添加到遍历列表里面
                        matchRowSweets.Add(sweets[x,newY]);
                    }
                    else
                    {
                        //如果遍历到的甜品颜色（类型）不一致，那么此次匹配终止
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
            

            //LT型匹配：
            //代码走到这里，说明左右行匹配都遍历完成，检查行遍历列标题中的元素数量是否大于3，如果是，将其添加到“完成匹配”列表中
            if(matchRowSweets.Count >= 3)
            {
                for(int i = 0; i < matchRowSweets.Count; i++)
                {
                    //行匹配列表中满足匹配的每个元素一次进行列遍历
                    //0代表上方遍历，1代表下方遍历
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

                    //对当前行元素进行列匹配后，判断数量是否小于2，如果小于2将其清零(没有包括本身)
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
                        //TL型匹配完成，不用再循环判断别的游戏独享了
                        break;
                    }
                }
            }

            //判断行匹配列表是否大于3个，是的话说明行匹配成功，返回成功的行匹配列表
            if(finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
            //如果行匹配之后，完成匹配列列表数量小于3，那么将行匹配列表，列匹配列表和完成匹配列表都清空，进行元素的列匹配
            matchRowSweets.Clear();
            matchLineSweets.Clear();
            finishedMatchingSweets.Clear();



            //列匹配
            matchLineSweets.Add(sweet);

            //i为0表示往左遍历，i为1表示往右遍历
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        //往左遍历
                        y = newY - yDistance;
                    }
                    else
                    {
                        //往右遍历
                        y = newY + yDistance;
                    }
                    //到达边界
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }

                    if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == color)
                    {
                        //如果遍历到的相邻甜品时同颜色（同类型），那么将其添加到遍历列表里面
                        matchLineSweets.Add(sweets[newX, y]);
                    }
                    else
                    {
                        //如果遍历到的甜品颜色（类型）不一致，那么此次匹配终止
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

            //LT型匹配：
            //代码走到这里，说明左右行匹配都遍历完成，检查行遍历列标题中的元素数量是否大于3，如果是，将其添加到“完成匹配”列表中
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //行匹配列表中满足匹配的每个元素一次进行列遍历
                    //0代表上方遍历，1代表下方遍历
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

                    //对当前列元素进行行匹配后，判断数量是否小于2，如果小于2将其清零(没有包括本身)
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
                        //TL型匹配完成，不用再循环判断别的游戏独享了
                        break;
                    }
                }
            }

            //判断行遍历是否完成，如果完成到此结束，如果没完成进行列遍历
            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }

        //如果行列匹配都不满足
        return null;
    }
    
    /// <summary>
    /// 清除甜品方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ClearSweet(int x,int y)
    {
        //如果当前甜品可以清除并且不在清除状态，则开启协程对其进行清除
        if (sweets[x, y].CanClear() && !sweets[x,y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            //在清除后的位置创建一个空甜品对象
            CreateNewSweet(x, y, SweetType.EMPTY);
            //调用饼干清除方法：判断清除的甜品周围有没有饼干
            ClearBarrier(x, y);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 清除行方法
    /// </summary>
    /// <param name="row">要清除的行</param>
    public void ClearRow(int row)
    {
        for(int x = 0; x < xColumn; x++)
        {
            ClearSweet(x, row);
        }
    }

    /// <summary>
    /// 清除列方法
    /// </summary>
    /// <param name="column">要清除的列</param>
    public void ClearColumn(int column)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(column, y);
        }
    }

    /// <summary>
    /// 清除同种颜色方法
    /// </summary>
    /// <param name="color">要清楚的颜色类型</param>
    public void ClearColor(ColorSweet.ColorType color)
    {
        for(int x = 0; x < xColumn; x++)
        {
            for(int y = 0; y < yRow; y++)
            {
                //交换两个彩虹糖时，会清空甜品对象
                if(sweets[x,y].CanColor() && (sweets[x,y].ColorComponent.Color == color || ColorSweet.ColorType.COLORS == color))
                {
                    ClearSweet(x, y);
                }
            }
        }
    }

    /// <summary>
    /// 消除饼干的方法
    /// 传入正在消除的甜品的坐标，遍历该甜品对象的上下左右是否有饼干对象，有的话消除饼干
    /// </summary>
    /// <param name="x">正在消除的甜品的x坐标</param>
    /// <param name="y">正在消除的甜品的y坐标</param>
    private void ClearBarrier(int x,int y)
    {
        //遍历正在消除的甜品的左边和右边
        for(int friendX = x - 1; friendX <= x + 1; friendX++)
        {
            //排除自身与边界
            if(friendX != x && friendX >= 0 && friendX < xColumn)
            {
                //如果左右为饼干并且可清除，则清除
                if (sweets[friendX, y].SweetType == SweetType.BARRIER && sweets[friendX, y].CanClear())
                {
                    sweets[friendX, y].ClearComponent.Clear();
                    CreateNewSweet(friendX,y,SweetType.EMPTY);
                }
            }
        }

        //遍历正在消除的甜品的上边和下边
        for (int friendY = y - 1; friendY <= y + 1; friendY++)
        {
            //排除自身与边界
            if (friendY != y && friendY >= 0 && friendY < yRow)
            {
                //如果左右为饼干并且可清除，则清除
                if (sweets[x, friendY].SweetType == SweetType.BARRIER && sweets[x, friendY].CanClear())
                {
                    sweets[x, friendY].ClearComponent.Clear();
                    CreateNewSweet(x, friendY, SweetType.EMPTY);
                }
            }
        }
    }

    /// <summary>
    /// 清除所有匹配好的甜品
    /// </summary>
    /// <returns></returns>
    private bool ClearAllMatchedSweet()
    {
        //清除掉甜品之后是需要重新填充的
        bool needRefill = false;

        //遍历所有网格可以消除的甜品对象
        for(int y = 0; y < yRow; y++)
        {
            for(int x = 0; x < xColumn; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<GameSweet> matchList = MatchSweets(sweets[x, y], x, y);
                    if(matchList != null)
                    {
                        //产生行列消除甜品
                        //定义标记位
                        SweetType specialSweetsType = SweetType.COUNT;//标记我们是否产生特殊甜品，如果产生特殊甜品，将其类型赋值给COUNT类型（相当于一个中间变量temp）
                        GameSweet randomSweet = matchList[Random.Range(0, matchList.Count)];//随机获取消除的一个甜品对象，进而获取改对象的位置
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if(matchList.Count == 4)
                        {
                            specialSweetsType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.COLUMN_CLEAR);
                        }
                        else if(matchList.Count >= 5)
                        {
                            //产生彩虹糖
                            specialSweetsType = SweetType.RAINBOWCANDY;
                        }


                        //清除甜品
                        for(int i = 0; i < matchList.Count; i++)
                        {
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }

                        if(specialSweetsType != SweetType.COUNT)
                        {
                            //如果specialSweetsType不为COUNT，说明是行列消除的一种
                            Destroy(sweets[specialSweetX, specialSweetY]);
                            GameSweet newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetsType);
                            //判断类型并进行安全校验
                            if((specialSweetsType == SweetType.ROW_CLEAR || specialSweetsType == SweetType.COLUMN_CLEAR) && newSweet.CanColor() && matchList[0].CanColor())
                            {
                                //设置行列消除的颜色（类型）
                                newSweet.ColorComponent.SetColor(matchList[0].ColorComponent.Color);
                            }
                            else if(specialSweetsType == SweetType.RAINBOWCANDY && newSweet.CanColor())
                            {
                                //设置彩虹糖类型
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

    //游戏结束界面方法
    #region
    /// <summary>
    /// 返回菜单方法
    /// </summary>
    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// 重玩方法
    /// </summary>
    public void RePlay()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
