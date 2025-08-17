using UnityEngine;

[CreateAssetMenu( fileName = "New Sushi Data", menuName = "Sushi/SushiData" )]
public class SushiDataScriptableObject : BaseDataScriptableObject<SushiItem>
{

}

[System.Serializable]
public class SushiItem : BaseItem
{
    [Header( "スシのプレハブ" ), Tooltip( "プレイヤーが使用するオブジェクト" )]
    public GameObject sushiPrefub;
    [Header( "前方移動　最大速度" ), Tooltip( "前方向に進む通常時の最大速度" )]
    public int maxFrontSpeed = 5000;
    [Header( "前方移動　加速度" ), Tooltip( "前方向に進む・オブジェクトの回転時に使う加速度" )]
    public int accelSpinRate = 20;
    [Header( "前方移動　減速値" ), Tooltip( "回していない時の減速" )]
    public int decaySpinRate = 1;
    [Header( "スシ回転　最大速度" ), Tooltip( "オブジェクトが回る速さ。速すぎると逆回転してるように見える" )]
    public float rotationSpeed = 3800f; // 回転速度の倍率
    [Header( "入力の閾値" ), Tooltip( "スティックのデッドゾーン" )]
    public float inputLimit = 0.1f;
    [Header( "角度差分の閾値" ), Tooltip( "スティックのデッドゾーン" )]
    public float angleLimit = 5f;

    [Header( "横移動　最大速度" ), Tooltip( "横入力をした時の横滑り。最大速度" )]
    public float maxSideSpeed = 50f;            // プレイヤーの最大速度
    [Header( "横移動　加速度" ), Tooltip( "横入力をした時の横滑り。加速度" )]
    public float accelSideRate = 5f;        // プレイヤーの加速度
    [Header( "カメラ旋回速度　最大速度時" ), Tooltip( "スシが停止時の旋回能力" )]
    public float rotationMinSpeed = 10f;
    [Header( "カメラ旋回速度　停止時" ), Tooltip( "スシが最大回転の時の旋回能力" )]
    public float rotationMaxSpeed = 60f;     // プレイヤーの回転速度

    [Header( "ブレーキ速度" ), Tooltip( "スシがブレーキをかける速度。0で急停止" ), Range(0.1f, 1f)]
    public float brakeSpeed = 0.5f; // ブレーキ速度

    [Header( "ギア比" ), Tooltip( "壁ドリフト時の速度。1で通常時と同じ、2で2倍、0.5で半分の速度" )]
    public float gierRatio = 1f; // ギア比。1で通常、2で2倍、0.5で半分の速度
}

