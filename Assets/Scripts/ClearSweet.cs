using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 甜品消除脚本
/// </summary>
public class ClearSweet : MonoBehaviour
{
    //变量：
    //判断是否正在清除（正在播放清除动画）
    private bool isClearing;

    public AudioClip destroyAudio;
    public bool IsClearing
    {
        get
        {
            return isClearing;
        }
    }

    //引用
    //获取甜品消除动画
    public AnimationClip clearAnimation;
    //甜品对象（后续还有整行整列等拓展消除，所以将变量定义为protected类型方便拓展）
    protected GameSweet sweet;

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }

    /// <summary>
    /// 后期需要对清除方法进行扩充，将方法定义为virtual虚方法
    /// </summary>
    public virtual void Clear()
    {
        //状态为正在清除
        isClearing = true;
        //开启清除协程
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if(animator != null)
        {
            animator.Play(clearAnimation.name);
            //玩家得分，播放清除声音等
            GameManager.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(destroyAudio, transform.position);
            //等待动画播放完成
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);//销毁当前对象
        }
    }
}
