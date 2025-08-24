using UnityEngine;
using UnityEngine.UI;

public class CountDownAnimation_Tsuji : MonoBehaviour
{
    public static CountDownAnimation_Tsuji instance;

    private void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if ( instance == null )
        {
            instance = this;
        }
    }


    private void OnDestroy()
    {
        instance = null;
    }

    [SerializeField]
    private Image countdownImage;    // UI��Image

    [SerializeField]
    private Sprite[] numberSprites;  // �S����Sprite

    [SerializeField]
    private float rotationSpeed = 90f; // ��]���x�A�P�b��90�x��]

    [SerializeField]
    private float subtractionSize = 1.0f;   // �T�C�Y�������������

    private Vector2 initialSize; // �����T�C�Y��ۑ�����ϐ�

    private int countNumber = 0;

    private float timer = 0.0f; // �^�C�}�[�̏�����

    [SerializeField]
    private float startWaitTime = 0.5f; // �J�n�ҋ@����

    private float startTimeCount = 0.0f; // �J�n�ҋ@���Ԃ̃J�E���g

    private bool isStarted = false; // �J�n�t���O

    private bool isAudioPlayed = false; // �����Đ����ꂽ���ǂ����̃t���O

    [SerializeField]
    private AudioComponent countDownSound; // �J�E���g�_�E�����̎Q��
    [SerializeField]
    private AudioComponent startSound; // �X�^�[�g���̎Q��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSize = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        startTimeCount += Time.fixedDeltaTime; // �J�n�ҋ@���Ԃ��J�E���g

        if (startWaitTime < startTimeCount) {
            transform.localPosition = new Vector2(0.0f, 0.0f); // ��ʂ̒��S�ɔz�u

            if (countNumber < numberSprites.Length)
            {
                timer += Time.fixedDeltaTime; // �^�C�}�[���X�V
                if (timer > 0.5f && timer < 0.8f)
                {
                    if (countNumber == 3 && !isStarted)
                    {
                        startSound.Play();
                        isStarted = true; // �^�C�}�[��0.5�b�𒴂�����J�n�t���O�𗧂Ă�
                    }
                    else if(!isAudioPlayed)
                    {
                        countDownSound.Play();
                        isAudioPlayed = true; // �����Đ����ꂽ���Ƃ��L�^
                    }
                    return;
                }
                else
                {
                    isAudioPlayed = false; // �����Đ�����Ă��Ȃ����Ƃ��L�^
                }

                    countdownImage.transform.localScale = new Vector2( transform.localScale.x - subtractionSize, transform.localScale.y - subtractionSize ); // ��ʃT�C�Y�ɍ��킹��Image�̃T�C�Y��ύX

                Rolling(); // ��]�������Ăяo��

                if (countdownImage.transform.localScale.x <= 0.0f || countdownImage.transform.localScale.y <= 0.0f)
                {
                    countNumber++;

                    if (countNumber < numberSprites.Length)
                    {
                        timer = 0.0f; // �^�C�}�[�����Z�b�g
                        countdownImage.transform.localScale = initialSize; // �T�C�Y��0�ȉ��ɂȂ����珉���T�C�Y�ɖ߂�
                        countdownImage.transform.localRotation = Quaternion.identity; // ��]�����Z�b�g
                        countdownImage.sprite = numberSprites[countNumber]; // �X�v���C�g���N���A
                    }
                    else
                    {
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }


    }

    void Rolling()
    {
        transform.Rotate(0.0f, 0.0f, rotationSpeed * Time.fixedDeltaTime);
    }

    public bool GetIsStarted()
    {
        return isStarted;
    }
}