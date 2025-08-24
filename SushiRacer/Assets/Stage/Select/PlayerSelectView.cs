using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectView : MonoBehaviour
{
    [SerializeField]
    private int playerIndex = 0;

    [SerializeField]
    private SushiDataScriptableObject sushiDatas;

    [SerializeField, Header( "�I�����Ă�����i�̃C���f�b�N�X" )]
    private int sushiIndex = 0;

    [SerializeField, Header( "���[����e�I�u�W�F�N�g" )]
    private Transform parentTransform;

    [SerializeField, Header( "������������Text" )]
    private TMP_Text[] descriptionText;

    [SerializeField, Header( "�I���ł���I�u�W�F�N�g" )]
    private Dictionary<int,List<Transform>> selectTransforms;

    private Vector2Int selectNavigate = Vector2Int.zero;
    private Vector2Int maxSelectNavigate;

    private readonly int inputDelay = 8;

    void Start()
    {
        for ( int i = 0; i < sushiDatas.items.Length; i++ )
        {
            var sushiData = sushiDatas.items[i].selectData;
        }
    }

    public void SetTextDescriptionText(int index)
    {
        var sushiData = sushiDatas.items[index].selectData;

        for (int j = 0; j < descriptionText.Length; j++)
        {
            if (j == 0)
                descriptionText[j].text = sushiData.speedText;
            if (j == 1)
                descriptionText[j].text = sushiData.accelText;
            if (j == 2)
                descriptionText[j].text = sushiData.rotationText;
            if (j == 3)
                descriptionText[j].text = sushiData.descriptionText;
        }
    }

    public void SetSelectPlayer()
    {
        PlayerSelectManager.Instance.SetSelectedCharacterIndex( playerIndex, sushiIndex );
    }

    private void FixedUpdate()
    {
        var inputNavigate = InputManager.Instance.GetActionValue<Vector2>( playerIndex, "UI", "Navigate" );

        // X���͂�Y���͂̂ǂ��炩��臒l�𒴂����ꍇ�ɏ������s��
        if ( Mathf.Abs( inputNavigate.x ) > 0.5f )
        {
            NavigateX( inputNavigate.x );
        }
        else if ( Mathf.Abs( inputNavigate.y ) > 0.5f )
        {
            NavigateY( inputNavigate.y );
        }
    }

    private void NavigateX(float addNum)
    {
        if ( addNum > 0f )
        {
            selectNavigate.x += 1;
        }
        else if ( addNum < 0f )
        {
            selectNavigate.x -= 1;
        }
    }

    private void NavigateY( float addNum )
    {
        if (addNum > 0f)
        {
            selectNavigate.y += 1;
        }
        else if (addNum < 0f)
        {
            selectNavigate.y -= 1;
        }
    }
}
