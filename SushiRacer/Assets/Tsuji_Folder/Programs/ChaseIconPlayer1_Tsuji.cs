using UnityEngine;
using UnityEngine.Rendering;

public class ChaseIconPlayer1_Tsuji : MonoBehaviour
{
    //[SerializeField]
    //private PlayerKeeper_Tsuji playerKeeper; // PlayerKeeper�̎Q��

    [SerializeField]
    private float offsetHeight = 0.0f;
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private SushiDataScriptableObject sushi;

    private bool setUp = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerKeeper_Tsuji playerKeeper = PlayerKeeper_Tsuji.instance; // PlayerKeeper�̃C���X�^���X���擾
        if (playerKeeper != null)
        {
            GameObject player1 = playerKeeper.GetPlayer1(); // PlayerKeeper����v���C���[1��GameObject���擾
            if (player1 != null)
            {
                // �v���C���[1�̈ʒu�ɃA�C�R����Ǐ]������
                transform.position = player1.transform.position + Vector3.up * offsetHeight;


                if ( !setUp )
                {
                    int charIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex(0);
                    meshRenderer.material = sushi.items[charIndex].minimapMaterial;
                    setUp = true;
                }
            }
        }

    }
}
