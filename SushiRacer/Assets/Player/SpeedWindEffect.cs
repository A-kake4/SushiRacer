using UnityEngine;

public class SpeedWindEffect : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb = null; // �Ώۂ�Rigidbody

    [SerializeField, Header("���̃G�t�F�N�g")]
    private ParticleSystem windEffect = null; // ���̃G�t�F�N�g

    [SerializeField,Header("�ő呬�x")]
    private float maxSpeed = 100f; // �ő呬�x�̊�l

    [SerializeField, Header("���̃G�t�F�N�g�̗ʂ𒲐����邽�߂̃p�����[�^")]
    private float maxRateOverTime = 50f; // �G�t�F�N�g�ʂ̍ŏ��l�ƍő�l

    private void FixedUpdate()
    {
        // ���ݑ��x���擾
        float speedRate = rb.linearVelocity.sqrMagnitude / ( maxSpeed * maxSpeed );

        //Debug.Log($"Speed Rate: {rb.linearVelocity.magnitude}");

        var emission = windEffect.emission;
        emission.rateOverTime = speedRate * maxRateOverTime;
    }
}
