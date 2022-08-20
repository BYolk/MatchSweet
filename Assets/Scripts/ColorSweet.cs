using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 甜品颜色（种类）脚本，以甜品颜色来代表不同的甜品类型
/// </summary>
public class ColorSweet : MonoBehaviour
{
    //颜色（类型）枚举
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        COLORS,//彩虹糖
        COUNT//标记位
    }
    //颜色字典:以键值对方式保存甜品颜色与对应的精灵
    private Dictionary<ColorType, Sprite> colorSpriteDictionary;
    //颜色结构体，结构体内保存颜色和精灵属性，然后将所有的甜品颜色和对应的精灵都制成结构体保存到结构体数组内，遍历结构体数组，从中取出甜品颜色和对应的精灵保存到字典内
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    //颜色结构体数组
    public ColorSprite[] colorSprites;



    //变量
    //定义后期随机颜色的上限值，用随机数表示甜品种类的随机
    public int NumColors
    {
        get { return colorSprites.Length; }
    }
    //定义当前颜色类型
    private ColorType color;
    public ColorType Color
    {
        get
        {
            return color;
        }
        set
        {
            SetColor(value);
        }
    }


    //引用
    //获取精灵渲染器，用于更换精灵渲染器里面的精灵图片
    private SpriteRenderer sprite;



    private void Awake()
    {
        //获取渲染器(挂有本脚本的对象transform是一个空物体对象，本身没有渲染器，渲染器在空物体对象的子对象Sweet身上，所以要使用Find方法得到Sweet对象)
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();

        //实例化字典
        colorSpriteDictionary = new Dictionary<ColorType, Sprite>();
        for(int i = 0; i < colorSprites.Length; i++)
        {
            //如果字典对象中没有包含甜品种类结构体数组总的某一个结构体的种类，说明字典中没有改甜品种类，从结构体数组中将改结构体的种类和精灵添加到字典内
            if (!colorSpriteDictionary.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDictionary.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }


    /// <summary>
    /// 更换颜色精灵（图片），如果要更换的图片类型newColor存在于字典，那么根据该图片类型从字典内取出对应的精灵赋予精灵渲染器
    /// </summary>
    /// <param name="newColor"></param>
    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDictionary.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDictionary[newColor];
        }
    }
}
