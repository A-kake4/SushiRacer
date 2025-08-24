using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class SelectView : MonoBehaviour
{
    [SerializeField, Header( "�v���C���[�f�o�C�X" )]
    private int playerNumber = 0;

    [SerializeField, Header("��������")]
    private bool isReady = false;

    [SerializeField, Header( "�����p�I�u�W�F�N�g" )]
    private GameObject readyObject;

    [SerializeField]
    private SushiDataScriptableObject sushiDatas;

    [SerializeField, Header( "�I�����Ă�����i�̃C���f�b�N�X" )]
    private int sushiIndex = 0;

    [SerializeField, Header( "������������Text" )]
    private TMP_Text[] descriptionText;

    [SerializeField, Header( "���ʒu" )]
    private Transform leftPosition;
    [SerializeField, Header( "�����ʒu" )]
    private Transform centerPosition;
    [SerializeField, Header( "�E�ʒu" )]
    private Transform rightPosition;

    // ���O�ɐ��������I�u�W�F�N�g��ێ�����ϐ�
    private GameObject previousObject;
    private GameObject currentObject;

    private readonly int inputDelay = 8;
    private int inputDelayCount = 0;

    void Start()
    {
        sushiIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex( playerNumber );

        currentObject = SpownSushi( sushiIndex, centerPosition.position );

        SetTextDescriptionText( sushiIndex );
    }

    private GameObject SpownSushi( int index, Vector3 position )
    {
        var pre = sushiDatas.items[index].selectData.previewObject;
        var obj = Instantiate( pre, position, Quaternion.identity );
        return obj;
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

    public void SetSelectPlayer(int number)
    {
        PlayerSelectManager.Instance.SetSelectedCharacterIndex( number, sushiIndex );
    }

    private void NavigateX(float addNum)
    {
        Vector3 startPos;
        Vector3 finishPos;
        if ( addNum > 0f )
        {
            sushiIndex += 1;
            startPos = leftPosition.position;
            finishPos = rightPosition.position;
        }
        else
        {
            sushiIndex -= 1;
            startPos = rightPosition.position;
            finishPos = leftPosition.position;
        }

        var sinMoveCurrent = currentObject.AddComponent<SinMovePosition>();
        sinMoveCurrent.SetMove( finishPos );
        sinMoveCurrent.DelayDestroy = true;

        if ( previousObject != null )
        {
            Destroy( previousObject );
        }
        previousObject = currentObject;

        sushiIndex = ( sushiIndex + sushiDatas.items.Length ) % sushiDatas.items.Length;
        SetTextDescriptionText( sushiIndex );


        currentObject = SpownSushi( sushiIndex, startPos );
        var sinMoveNext = currentObject.AddComponent<SinMovePosition>();
        sinMoveNext.SetMove( centerPosition.position );
    }

    private void OnReady(bool ready)
    {
        isReady = ready;
        readyObject.SetActive( ready );
    }

    private void FixedUpdate()
    {
        var inputSubmit = InputManager.Instance.GetActionValue<bool>( playerNumber, "UI", "Submit" );
        if (inputSubmit && !isReady)
        {
            OnReady( true );
        }

        if (isReady)
        {
            var inputCancel = InputManager.Instance.GetActionValue<bool>( playerNumber, "UI", "Cancel" );
            if (inputCancel)
            {
                OnReady( false );
            }
            return;
        }

        var inputNavigateX = InputManager.Instance.GetActionValue<Vector2>( playerNumber, "UI", "Navigate" ).x;

        // X���͂ǂ��炩��臒l�𒴂����ꍇ�ɏ������s��
        if (Mathf.Abs( inputNavigateX ) > 0.5f && inputDelayCount > inputDelay)
        {
            // ���͂��󂯕t�����̂ŃJ�E���g���Z�b�g
            inputDelayCount = 0;

            NavigateX( inputNavigateX );
            SetSelectPlayer( playerNumber );
        }
        else if (Mathf.Abs( inputNavigateX ) <= 0.5f)
        {
            // 臒l�ȉ��Ȃ炷���ɓ��͂��󂯕t����悤�ɂ���
            inputDelayCount = inputDelay;
        }
        else if (inputDelayCount <= inputDelay)
        {
            // ���͂��󂯕t������A��莞�ԓ��͂𖳎�����
            inputDelayCount++;
        }
    }
}
