using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʒ���󣬴�����Ʒ�ƶ���������������������Ʒ��������
/// </summary>
public class GameSweet : MonoBehaviour
{
    //���ԣ�
    //��Ʒλ�����ԣ�
    private int x;
    private int y;
    //��Ʒ�������ԣ����ö�ٶ����������棬Ҫ��ȡ����������ԣ���Ҫͨ������ȥ��ȡö��
    private GameManager.SweetType sweetType;

    //����
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
        //��Ʒ������ʵ����֮��Ͳ���ı䣬���Բ���Ҫset����
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
        //ʵ����moveSweetComponent����ȡMoveSweet�ű�
        moveComponent = GetComponent<MoveSweet>();
        //ʵ����colorComponent����ȡColorSweet�ű�����
        colorComponent = GetComponent<ColorSweet>();
        clearComponent = GetComponent<ClearSweet>();
    }






    /// <summary>
    /// ��ʼ��GameSweets����
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
    /// �ж���Ʒ�Ƿ���ƶ�
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        //�����ǰ��Ϸ������MoveSweet�ű�����ô�����Ʒ���ǿ����ƶ���
        return moveComponent != null;
    }

    /// <summary>
    /// �ж���Ʒ�Ƿ����ɫ���ܹ���������ͼƬ��
    /// </summary>
    /// <returns></returns>
    public bool CanColor()
    {
        //�����ǰ��Ϸ������MoveSweet�ű�����ô�����Ʒ���ǿ����ƶ���
        return colorComponent != null;
    }

    /// <summary>
    /// �жϵ�ǰ��Ʒ�����ܷ�����
    /// </summary>
    /// <returns></returns>
    public bool CanClear()
    {
        return clearComponent != null;
    }


    /// <summary>
    /// ��������ͨ��Ʒ
    /// </summary>
    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }


    /// <summary>
    /// ��갴����ͨ��Ʒ
    /// </summary>
    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }


    /// <summary>
    /// ����뿪��ͨ��Ʒ
    /// </summary>
    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
}
