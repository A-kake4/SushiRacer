using UnityEngine;

public class SushiButton : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0; // �v���C���[�̃C���f�b�N�X
    public int PlayerIndex
    {
        get => playerIndex;
        set => playerIndex = value;
    }

    [SerializeField] private int sushiIndex = 0; // ���i�̃C���f�b�N�X
    public int SushiIndex
    {
        get => sushiIndex;
        set => sushiIndex = value;
    }

    // �X�e�[�W�Z���N�g�}�l�[�W���[�̎Q��
    private void OnMouseDown()
    {
        SetPlayerSelect(  );
    }

    public void SetPlayerSelect(  )
    {
        PlayerSelectManager.Instance.SetSelectedCharacterIndex( playerIndex, sushiIndex );
    }
}
