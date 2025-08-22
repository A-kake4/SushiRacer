using UnityEngine;

public class ChaseIconPlayer2_Tsuji : MonoBehaviour
{
    [SerializeField]
    private PlayerKeeper_Tsuji playerKeeper; // PlayerKeeper�̎Q��

    [SerializeField]
    private float offsetHeight = 0.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerKeeper != null)
        {
            GameObject player2 = playerKeeper.GetPlayer2(); // PlayerKeeper����v���C���[2��GameObject���擾
            if (player2 != null)
            {
                // �v���C���[2�̈ʒu�ɃA�C�R����Ǐ]������
                transform.position = player2.transform.position + Vector3.up * offsetHeight;
            }
        }
    }
}
