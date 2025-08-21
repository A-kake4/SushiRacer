using UnityEngine;
using UnityEngine.UI;

public class CircleGauge_Tsuji : MonoBehaviour
{
    private const float MaxValue = 0.75f; // ゲージの最大値
    private float nowValue = 0f; // 現在のゲージ値
    [SerializeField]
    private Image gaugeImage; // ゲージのImageコンポーネント

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gaugeImage != null)
        {
            if (nowValue > MaxValue)
            {
                nowValue = MaxValue; // 最大値を超えないように制限
            }
            else if (nowValue < 0f)
            {
                nowValue = 0f; // 最小値を下回らないように制限
                               // ゲージの値を更新
            }
            // ゲージのfillAmountを更新
            gaugeImage.fillAmount = nowValue;
        }
    }

    public void SetGaugeValue(float value)
    {
        nowValue = value; // ゲージの値を設定
    }
}
