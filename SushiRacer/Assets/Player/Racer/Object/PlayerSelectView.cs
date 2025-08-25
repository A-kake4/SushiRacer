using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviour
{
    [SerializeField, Header( "プレイヤーデバイス" )]
    private int playerNumber = 0;
    [SerializeField, Header( "選択している寿司のインデックス" )]
    private int sushiIndex = 0;
    [SerializeField, Header("準備完了")]
    private bool isReady = false;
    public bool IsReady => isReady;

    [SerializeField, Header( "完了用オブジェクト" )]
    private GameObject readyObject;

    [SerializeField,Header("選択中の寿司アイコン")]
    private Image  sushiIconImage;

    [SerializeField]
    private SushiDataScriptableObject sushiDatas;

    [SerializeField, Header( "説明文を入れるText" )]
    private TMP_Text[] descriptionText;

    [SerializeField, Header( "左位置" )]
    private Transform leftPosition;
    [SerializeField, Header( "中央位置" )]
    private Transform centerPosition;
    [SerializeField, Header( "右位置" )]
    private Transform rightPosition;

    [SerializeField,Header("決定音")]
    private AudioComponent decideSound;
    [SerializeField,Header("キャンセル音")]
    private AudioComponent cancelSound;
    [SerializeField,Header("カーソル移動音")]
    private AudioComponent moveSound;

    // 直前に生成したオブジェクトを保持する変数
    private GameObject previousObject;
    private GameObject currentObject;

    private readonly int inputDelay = 8;
    private int inputDelayCount = 0;

    private bool isSubmit = false;
    private bool oldSubmit = false;
    private bool isCancel = false;
    private bool oldCancel = false;

    private void Start()
    {
        sushiIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex( playerNumber );

        currentObject = SpownSushi( sushiIndex, centerPosition.position );

        SetTextDescriptionText( sushiIndex );

        SelectOk.Instance.SetSelectView( this );
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
        sushiIconImage.sprite = sushiData.sushiIcon;
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

        var sinMoveCurrent = currentObject.GetComponent<SinMovePosition>();
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
        if (!currentObject.TryGetComponent<SinMovePosition>( out var sinMoveNext ))
        {
            Debug.LogError( "SinMovePosition component is missing on the sushi preview object." );
        }
        sinMoveNext.SetMove( centerPosition.position );
    }

    private void OnReady(bool ready)
    {
        isReady = ready;
        readyObject.SetActive( ready );

        currentObject.GetComponent<RotationObject>().IsRotate = !ready;
        currentObject.GetComponent<RotateSet>().IsRotate = ready;
    }

    private void FixedUpdate()
    {
        if(inputDelayCount < inputDelay)
        {
            inputDelayCount++;
            return;
        }

        oldSubmit = isSubmit;

        var inputSubmit = InputManager.Instance.GetActionPhase( playerNumber, "UI", "Submit" );
        isSubmit = inputSubmit == InputActionPhase.Performed;

        if (isSubmit && !oldSubmit && !isReady)
        {
            OnReady( true );
            decideSound.Play();
            inputDelayCount = 0;
        }

        if (isReady)
        {
            oldCancel = isCancel;

            var inputCancel = InputManager.Instance.GetActionPhase( playerNumber, "UI", "Cancel" );
            isCancel = inputCancel == InputActionPhase.Performed;

            if (isCancel && !oldCancel)
            {
                OnReady( false );
                cancelSound.Play();
                inputDelayCount = 0;
            }
            return;
        }

        var inputNavigateX = InputManager.Instance.GetActionValue<Vector2>( playerNumber, "UI", "Navigate" ).x;

        // X入力どちらかが閾値を超えた場合に処理を行う
        if (Mathf.Abs( inputNavigateX ) > 0.5f)
        {
            // 入力を受け付けたのでカウントリセット
            inputDelayCount = 0;

            moveSound.Play();

            NavigateX( inputNavigateX );
            SetSelectPlayer( playerNumber );
        }
    }
}
