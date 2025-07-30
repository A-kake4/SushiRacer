using UnityEngine;

[CreateAssetMenu( fileName = "New Sushi Data", menuName = "Sushi/SushiData" )]
public class SushiDataScriptableObject : BaseDataScriptableObject<SushiItem>
{

}

[System.Serializable]
public class SushiItem : BaseItem
{
    [Tooltip( "スシのプレハブ" )]
    public GameObject sushiPrefub;
    [Tooltip( "前方移動　最大速度" )]
    public int maxFrontSpeed = 5000;
    [Tooltip( "前方移動　加速度" )]
    public int accelSpinRate = 20;
    [Tooltip( "前方移動　減速値" )]
    public int decaySpinRate = 1;
    [Tooltip( "スシ回転　最大速度" )]
    public float rotationSpeed = 3800f; // 回転速度の倍率
    [Tooltip( "入力の閾値" )]
    public float inputLimit = 0.1f;
    [Tooltip( "角度差分の閾値" )]
    public float angleLimit = 5f;

    [Tooltip( "横移動　最大速度" )]
    public float maxSideSpeed = 50f;            // プレイヤーの最大速度
    [Tooltip( "横移動　加速度" )]
    public float accelSideRate = 5f;        // プレイヤーの加速度
    [Tooltip( "カメラ旋回速度　最大速度時" )]
    public float rotationMinSpeed = 10f;
    [Tooltip( "カメラ旋回速度　停止時" )]
    public float rotationMaxSpeed = 60f;     // プレイヤーの回転速度
}

