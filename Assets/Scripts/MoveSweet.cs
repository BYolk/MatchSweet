using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制甜品移动脚本：不是每个甜品都能移动，如饼干和空白甜品种类是不能移动的，相当于障碍物
/// </summary>
public class MoveSweet : MonoBehaviour
{
    //属性
    //得到其他指令时终止这个协程
    private IEnumerator moveCoroutine;
    
    //引用
    private GameSweet sweet;


    private void Awake()
    {
        //获取甜品游戏对象
        sweet = GetComponent<GameSweet>();
    }

    /// <summary>
    /// 开启或者结束协程，控制甜品移动方法
    /// </summary>
    /// <param name="moveToX">甜品新的X位置</param>
    /// <param name="moveToY">甜品新的Y位置</param>
    public void Move(int newX,int newY,float time)
    {
        if(moveCoroutine != null)
        {
            //如果协程部位空，将协程停掉（为什么要关闭旧协程：如果甜品开启协程往下移动之后，发现它下面还可以继续往下移动，也就是需要开一个新协程，如果此时旧协程不关闭，新旧协程同时工作，会造成逻辑混乱，甜品移动会不规律）
            StopCoroutine(moveCoroutine);
        }

        //开启负责移动的协程程序
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    /// <summary>
    /// 甜品移动协程程序
    /// 方法学习：
    /// 线性插值函数Lerp()：unity3D中经常用线性插值函数Lerp()来在两者之间插值，两者之间可以是两个材质之间、两个向量之间、两个浮点数之间、两个颜色之间
    /// 第三个参数为间断值（几分之几，范围是0-1），具体计算得到返回值也就是 a + （b - a） * t
    /// Lerp方法会让“变化”变得很平滑
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX,int newY,float time)
    {
        sweet.X = newX;
        sweet.Y = newY;
        //每一帧移动一点点
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CorrectGridPosition(newX, newY);

        for(float t = 0; t < time; t += Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            //等待一帧
            yield return 0;
        }

        //如果协程结束甜品位置还没到指定位置，那么将甜品位置强行移动到目标位置
        sweet.transform.position = endPos;
    }
}
