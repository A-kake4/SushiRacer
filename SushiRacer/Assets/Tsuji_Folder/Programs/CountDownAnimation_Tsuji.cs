using UnityEngine;
using UnityEngine.UI;

public class CountDownAnimation_Tsuji : MonoBehaviour
{
    public static CountDownAnimation_Tsuji instance;

    private void Awake()
    {
        // シングルトンパターンの実装
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
    private Image countdownImage;    // UIのImage

    [SerializeField]
    private Sprite[] numberSprites;  // ４枚のSprite

    [SerializeField]
    private float rotationSpeed = 90f; // 回転速度、１秒で90度回転

    [SerializeField]
    private float subtractionSize = 1.0f;   // サイズを減少させる量

    private Vector2 initialSize; // 初期サイズを保存する変数

    private int countNumber = 0;

    private float timer = 0.0f; // タイマーの初期化

    [SerializeField]
    private float startWaitTime = 0.5f; // 開始待機時間

    private float startTimeCount = 0.0f; // 開始待機時間のカウント

    private bool isStarted = false; // 開始フラグ

    private bool isAudioPlayed = false; // 音が再生されたかどうかのフラグ

    [SerializeField]
    private AudioComponent countDownSound; // カウントダウン音の参照
    [SerializeField]
    private AudioComponent startSound; // スタート音の参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSize = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        startTimeCount += Time.fixedDeltaTime; // 開始待機時間をカウント

        if (startWaitTime < startTimeCount) {
            transform.localPosition = new Vector2(0.0f, 0.0f); // 画面の中心に配置

            if (countNumber < numberSprites.Length)
            {
                timer += Time.fixedDeltaTime; // タイマーを更新
                if (timer > 0.5f && timer < 0.8f)
                {
                    if (countNumber == 3 && !isStarted)
                    {
                        startSound.Play();
                        isStarted = true; // タイマーが0.5秒を超えたら開始フラグを立てる
                    }
                    else if(!isAudioPlayed)
                    {
                        countDownSound.Play();
                        isAudioPlayed = true; // 音が再生されたことを記録
                    }
                    return;
                }
                else
                {
                    isAudioPlayed = false; // 音が再生されていないことを記録
                }

                    countdownImage.transform.localScale = new Vector2( transform.localScale.x - subtractionSize, transform.localScale.y - subtractionSize ); // 画面サイズに合わせてImageのサイズを変更

                Rolling(); // 回転処理を呼び出す

                if (countdownImage.transform.localScale.x <= 0.0f || countdownImage.transform.localScale.y <= 0.0f)
                {
                    countNumber++;

                    if (countNumber < numberSprites.Length)
                    {
                        timer = 0.0f; // タイマーをリセット
                        countdownImage.transform.localScale = initialSize; // サイズが0以下になったら初期サイズに戻す
                        countdownImage.transform.localRotation = Quaternion.identity; // 回転をリセット
                        countdownImage.sprite = numberSprites[countNumber]; // スプライトをクリア
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