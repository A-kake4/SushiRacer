using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Exploded_Castle_Tsuji : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 1.0f; // �U���A�h��̕�
    [SerializeField]
    private float frequency = 1.0f; // ���g���A�h��̑���
    Vector3 startPosition;

    [SerializeField]
    private float fallSpeed = 5.0f; // �������x

    [SerializeField]
    private GoalAction_Tsuji goalAction = null;

    [SerializeField]
    SceneAsset sceneAsset = null;   // �V�[���A�Z�b�g���C���X�y�N�^�[�Őݒ�

    [SerializeField, HideInInspector] private string sceneName; // ���s���Ɏg���V�[����

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (goalAction == null) return;

        //if (goalAction.GetGoalFlag() == true)
        //{
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;
            float fallOffset = Time.time * fallSpeed;

            transform.position = startPosition + new Vector3(0, -fallOffset, offset);
       // }

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //if (sceneAsset != null)
        //{
        //    sceneName = sceneAsset.name; // �G�f�B�^��ŃV�[�����������擾
        //}
    }
#endif

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // ���s���͕�����Ń��[�h
        }
    }

}