using UnityEngine;
using UnityEngine.UI;

public class LiquidFill_Tsuji : MonoBehaviour
{
    //public RectTransform liquid;
    //public float fillAmount = 0.5f; // 0�`1�͈̔�

    //void Update()
    //{
    //    float maxHeight = 200f; // �t�̂̍ő卂���i�s�N�Z���j
    //    liquid.sizeDelta = new Vector2(liquid.sizeDelta.x, fillAmount * maxHeight);
    //}

    //public RectTransform liquidRect; // �t�̂�Image�I�u�W�F�N�g
    //public float amplitude = 50f;     // �����̐U�ꕝ
    //public float frequency = 1f;      // �g�̑���
    //public float baseHeight = 100f;   // ��̍����i��ɂ�����㉺����j

    //void Update()
    //{
    //    float newHeight = baseHeight + Mathf.Sin(Time.time * frequency) * amplitude;
    //    liquidRect.sizeDelta = new Vector2(liquidRect.sizeDelta.x, newHeight);
    //}

    public Image liquidImage;
    public float frequency = 1f;
    public float amplitude = 0.3f;
    public float baseFill = 0.5f;

    void Update()
    {
        float fill = baseFill + Mathf.Sin(Time.time * frequency) * amplitude;
        fill = Mathf.Clamp01(fill);
        liquidImage.fillAmount = fill;
    }

}
