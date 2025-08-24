using UnityEngine;
using UnityEngine.SceneManagement;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class Exploded_Castle_Tsuji : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 1.0f; // �U���A�h��̕�
    [SerializeField]
    private float frequency = 1.0f; // ���g���A�h��̑���
    Vector3 startPosition;

    [SerializeField]
    private float fallSpeed = 5.0f; // �������x
    private float timer = 0.0f;
    private float breakTime = 1.0f; // �h��̎���

    private Vector3 swayDirection = new Vector3(-1.0f, 0.0f, 1.0f).normalized; // �h��̕���

    [SerializeField]
    private GoalAction_Tsuji goalAction = null;

    //[SerializeField]
    //SceneAsset sceneAsset = null;   // �V�[���A�Z�b�g���C���X�y�N�^�[�Őݒ�

    // [SerializeField, HideInInspector] private string sceneName; // ���s���Ɏg���V�[����
    [SerializeField] private string sceneName; // ���s���Ɏg���V�[����

    [SerializeField]
    private float finishHeight = -50.0f;    // �邪����ŏI������������
    private bool isFinish = false;          // �I���t���O

    [SerializeField]
    private float finishWaitTime = 2.0f; // �I����̑ҋ@����
    private float finishTimer = 0.0f;

    private bool isExploded = false;
    [SerializeField]
    private float explodeStartTime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (goalAction == null) return;

        if (goalAction.GetGoalFlag() == true && isFinish == false)
        {
            timer += Time.fixedDeltaTime;
            if (timer > explodeStartTime)
            {
                breakTime += Time.fixedDeltaTime;
                float offset = Mathf.Sin(breakTime * frequency) * amplitude;
                float fallOffset = breakTime * fallSpeed;

                transform.position = startPosition + swayDirection * offset + new Vector3(0, -fallOffset, 0);
                isExploded = true;
            }
        }
        
        if(transform.position.y < finishHeight)
        {
            isFinish = true;
        }


        if(isFinish == true)
        {
            finishTimer += Time.fixedDeltaTime;
            if (finishWaitTime < finishTimer)
            {
                LoadScene();
            }
        }
    }

    //#if UNITY_EDITOR
    //    private void OnValidate()
    //    {
    //        //if (sceneAsset != null)
    //        //{
    //        //    sceneName = sceneAsset.name; // �G�f�B�^��ŃV�[�����������擾
    //        //}
    //    }
    //#endif

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // ���s���͕�����Ń��[�h
        }
    }

    public bool GetExplodedFlag()
    {
        return isExploded;
    }

    public bool GetFinishFlag()
    {
        return isFinish;
    }
}