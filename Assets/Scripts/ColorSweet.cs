using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʒ��ɫ�����ࣩ�ű�������Ʒ��ɫ������ͬ����Ʒ����
/// </summary>
public class ColorSweet : MonoBehaviour
{
    //��ɫ�����ͣ�ö��
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        COLORS,//�ʺ���
        COUNT//���λ
    }
    //��ɫ�ֵ�:�Լ�ֵ�Է�ʽ������Ʒ��ɫ���Ӧ�ľ���
    private Dictionary<ColorType, Sprite> colorSpriteDictionary;
    //��ɫ�ṹ�壬�ṹ���ڱ�����ɫ�;������ԣ�Ȼ�����е���Ʒ��ɫ�Ͷ�Ӧ�ľ��鶼�Ƴɽṹ�屣�浽�ṹ�������ڣ������ṹ�����飬����ȡ����Ʒ��ɫ�Ͷ�Ӧ�ľ��鱣�浽�ֵ���
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    //��ɫ�ṹ������
    public ColorSprite[] colorSprites;



    //����
    //������������ɫ������ֵ�����������ʾ��Ʒ��������
    public int NumColors
    {
        get { return colorSprites.Length; }
    }
    //���嵱ǰ��ɫ����
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


    //����
    //��ȡ������Ⱦ�������ڸ���������Ⱦ������ľ���ͼƬ
    private SpriteRenderer sprite;



    private void Awake()
    {
        //��ȡ��Ⱦ��(���б��ű��Ķ���transform��һ����������󣬱���û����Ⱦ������Ⱦ���ڿ����������Ӷ���Sweet���ϣ�����Ҫʹ��Find�����õ�Sweet����)
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();

        //ʵ�����ֵ�
        colorSpriteDictionary = new Dictionary<ColorType, Sprite>();
        for(int i = 0; i < colorSprites.Length; i++)
        {
            //����ֵ������û�а�����Ʒ����ṹ�������ܵ�ĳһ���ṹ������࣬˵���ֵ���û�и���Ʒ���࣬�ӽṹ�������н��Ľṹ�������;�����ӵ��ֵ���
            if (!colorSpriteDictionary.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDictionary.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }


    /// <summary>
    /// ������ɫ���飨ͼƬ�������Ҫ������ͼƬ����newColor�������ֵ䣬��ô���ݸ�ͼƬ���ʹ��ֵ���ȡ����Ӧ�ľ��鸳�辫����Ⱦ��
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
