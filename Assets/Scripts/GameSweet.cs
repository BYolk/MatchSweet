using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 甜品对象，存有甜品移动、消除、动画播放与甜品自身属性
/// </summary>
public class GameSweet : MonoBehaviour
{
    //属性：
    //甜品位置属性：
    private int x;
    private int y;
    //甜品种类属性：如果枚举定义在类里面，要获取类里面的属性，需要通过该类去获取枚举
    private GameManager.SweetType sweetType;

    //引用
    [HideInInspector]
    public GameManager gameManager;
    private MoveSweet moveComponent;
    private ColorSweet colorComponent;
    private ClearSweet clearComponent;

    //getter and setter Method
    public int X
    {
        get
        {
            return x;
        }
        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }
    public GameManager.SweetType SweetType
    {
        get
        {
            return sweetType;
        }
        //甜品类型在实例化之后就不会改变，所以不需要set方法
    }
    public MoveSweet MoveComponent
    {
        get
        {
            return moveComponent;
        }
    }
    public ColorSweet ColorComponent
    {
        get
        {
            return colorComponent;
        }
    }
    public ClearSweet ClearComponent
    {
        get
        {
            return clearComponent;
        }
    }

    private void Awake()
    {
        //实例化moveSweetComponent，获取MoveSweet脚本
        moveComponent = GetComponent<MoveSweet>();
        //实例化colorComponent，获取ColorSweet脚本对象
        colorComponent = GetComponent<ColorSweet>();
        clearComponent = GetComponent<ClearSweet>();
    }






    /// <summary>
    /// 初始化GameSweets属性
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_gameManager"></param>
    /// <param name="_sweetType"></param>
    public void Init(int _x,int _y,GameManager _gameManager,GameManager.SweetType _sweetType)
    {
        x = _x;
        y = _y;
        gameManager = _gameManager;
        sweetType = _sweetType;
    }

    /// <summary>
    /// 判断甜品是否可移动
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        //如果当前游戏对象有MoveSweet脚本，那么这个甜品就是可以移动的
        return moveComponent != null;
    }

    /// <summary>
    /// 判断甜品是否可着色（能够更换精灵图片）
    /// </summary>
    /// <returns></returns>
    public bool CanColor()
    {
        //如果当前游戏对象有MoveSweet脚本，那么这个甜品就是可以移动的
        return colorComponent != null;
    }

    /// <summary>
    /// 判断当前甜品对象能否消除
    /// </summary>
    /// <returns></returns>
    public bool CanClear()
    {
        return clearComponent != null;
    }


    /// <summary>
    /// 鼠标进入普通甜品
    /// </summary>
    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }


    /// <summary>
    /// 鼠标按下普通甜品
    /// </summary>
    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }


    /// <summary>
    /// 鼠标离开普通甜品
    /// </summary>
    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
}
