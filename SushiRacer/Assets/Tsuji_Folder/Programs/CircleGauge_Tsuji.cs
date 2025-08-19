using UnityEngine;
using UnityEngine.UI;

public class CircleGauge_Tsuji : MonoBehaviour
{
    private const float MaxValue = 0.75f; // �Q�[�W�̍ő�l
    private float nowValue = 0f; // ���݂̃Q�[�W�l
    [SerializeField]
    private Image gaugeImage; // �Q�[�W��Image�R���|�[�l���g

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gaugeImage != null)
        {
            if (nowValue > MaxValue)
            {
                nowValue = MaxValue; // �ő�l�𒴂��Ȃ��悤�ɐ���
            }
            else if (nowValue < 0f)
            {
                nowValue = 0f; // �ŏ��l�������Ȃ��悤�ɐ���
                               // �Q�[�W�̒l���X�V
            }
            // �Q�[�W��fillAmount���X�V
            gaugeImage.fillAmount = nowValue;
        }
    }

    public void SetGaugeValue(float value)
    {
        nowValue = value; // �Q�[�W�̒l��ݒ�
    }
}
