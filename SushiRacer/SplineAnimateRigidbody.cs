using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class SplineAnimateRigidbody : MonoBehaviour
{
    // �X�v���C���R���e�i�ւ̎Q�Ɓi�C���X�y�N�^�[�Őݒ�j
    public SplineContainer splineContainer;
    // �X�v���C������ړ����鑬��
    public float speed = 1f;
    // �p�X�ɍ��킹�ĉ�]�����邩�ǂ���
    public bool orientToPath = true;
    // �I�u�W�F�N�g�̐i�s�������猩���I�t�Z�b�g
    public Vector3 offset;

    // �X�v���C����̐i�s�p�����[�^�i0�`1�Ƒz��j
    private float distance;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (splineContainer == null || splineContainer.Spline == null)
        {
            Debug.LogError("SplineContainer or Spline is not set.");
            enabled = false; // �X�N���v�g�𖳌���
            return;
        }
        // �����ʒu���X�v���C���̊J�n�ʒu�ɐݒ�
        distance = 0f;
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition(distance);
        Quaternion rot = orientToPath 
            ? Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(distance), Vector3.up) 
            : Quaternion.identity;
        Vector3 initialPosition = splinePos + rot * offset;
        rb.MovePosition(initialPosition);
    }

    private void FixedUpdate()
    {
        if (splineContainer == null || splineContainer.Spline == null)
            return;

        // ���Ԃɉ����Đi�s�p�����[�^���X�V (���[�v����ꍇ�̗�)
        distance += speed * Time.fixedDeltaTime;
        if (distance > 1f)
            distance -= 1f;

        // �X�v���C����̖ڕW�ʒu���擾���A�I�t�Z�b�g��K�p
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition(distance);
        Quaternion currentRot = orientToPath 
            ? Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(distance), Vector3.up) 
            : Quaternion.identity;
        Vector3 targetPosition = splinePos + currentRot * offset;
        rb.MovePosition(targetPosition);

        if (orientToPath)
        {
            // �X�v���C����̐ڐ������𗘗p���Č����𒲐�
            Vector3 forward = splineContainer.Spline.EvaluateTangent(distance);
            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
                rb.MoveRotation(targetRotation);
            }
        }
    }
}
}