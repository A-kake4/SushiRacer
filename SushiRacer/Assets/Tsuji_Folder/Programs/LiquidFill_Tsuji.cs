using UnityEngine;
using UnityEngine.UI;

public class LiquidFill_Tsuji : MonoBehaviour
{
    //public RectTransform liquid;
    //public float fillAmount = 0.5f; // 0～1の範囲

    //void Update()
    //{
    //    float maxHeight = 200f; // 液体の最大高さ（ピクセル）
    //    liquid.sizeDelta = new Vector2(liquid.sizeDelta.x, fillAmount * maxHeight);
    //}

    //public RectTransform liquidRect; // 液体のImageオブジェクト
    //public float amplitude = 50f;     // 高さの振れ幅
    //public float frequency = 1f;      // 波の速さ
    //public float baseHeight = 100f;   // 基準の高さ（常にこれより上下する）

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
