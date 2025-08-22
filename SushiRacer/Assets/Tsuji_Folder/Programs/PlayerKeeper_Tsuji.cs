using UnityEngine;
using UnityEngine.UI;

public class PlayerKeeper_Tsuji : MonoBehaviour
{
    private GameObject player1 = null; // �v���C���[1��GameObject
    private GameObject player2 = null; // �v���C���[2��GameObject

    [SerializeField]
    private Image player1RankImage; // �v���C���[���ʗp��UI�C���[�W

    [SerializeField]
    private Image player2RankImage; // �v���C���[���ʗp��UI�C���[�W

    [SerializeField]
    private Sprite rank1Sprite; // �P�ʂ̃X�v���C�g

    [SerializeField]
    private Sprite rank2Sprite; // �Q�ʂ̃X�v���C�g

    [SerializeField]
    private CircleGauge_Tsuji circle1;
    public CircleGauge_Tsuji Circle1 => circle1;

    [SerializeField]
    private CircleGauge_Tsuji circle2;
    public CircleGauge_Tsuji Circle2 => circle2;


    private Camera camera1 = null; // �v���C���[1�̃J����
    private Camera camera2 = null; // �v���C���[2�̃J����

    public static PlayerKeeper_Tsuji  instance = null; // �V���O���g���C���X�^���X

    private void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    //private void FixedUpdate()
    //{
    //    if(player1 == null || player2 == null)
    //    {
    //        player1 = GameObject.Find("Player1"); // �^�O�Ńv���C���[1������
    //        player2 = GameObject.Find("Player2"); // �^�O�Ńv���C���[2������

    //        player1.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player1RankImage, rank1Sprite, rank2Sprite);
    //        player2.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player2RankImage, rank1Sprite, rank2Sprite);

    //    }

    //    if (player1 == null)
    //    {
    //        Debug.Log("�v���C���[��1");
    //    }

    //    if(player2 == null)
    //    {
    //        Debug.Log("�v���C���[��2");
    //    }
    //}

    public void SetPlayer1(GameObject player)
    {
        player1 = player; // �v���C���[1��GameObject��ݒ�
        player1.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player1RankImage,rank1Sprite,rank2Sprite);
    }

    public void SetCamera1(Camera cameraObject)
    {
        camera1 = cameraObject; // �v���C���[1�̃J������ݒ�
    }

    public Camera GetCamera1()
    {
        return camera1; // �v���C���[1�̃J�������擾
    }

    public void SetPlayer2(GameObject player)
    {
        player2 = player; // �v���C���[2��GameObject��ݒ�
        player2.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player2RankImage, rank1Sprite, rank2Sprite);
    }
    public void SetCamera2(Camera cameraObject)
    {
        camera2 = cameraObject; // �v���C���[2�̃J������ݒ�
    }

    public Camera GetCamera2()
    {
        return camera2; // �v���C���[2�̃J�������擾
    }

    public GameObject GetPlayer1()
    {
        return player1; // �v���C���[1��GameObject���擾
    }

    public GameObject GetPlayer2()
    {
        return player2; // �v���C���[2��GameObject���擾
    }
}
