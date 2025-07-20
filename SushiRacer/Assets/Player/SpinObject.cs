using UnityEngine;
//-------------------------------------
// オブジェクトを回転させるためのスクリプト
//--------------------------------------
// SpinInputの回転を利用して毎フレーム回転を行う

public class SpinObject : MonoBehaviour
{
    [SerializeField]
    private SpinImput spinInput; // SpinInputの参照

    [SerializeField]
    private Transform spinObject; // 回転させるオブジェクト

    [SerializeField]
    private float rotationSpeed = 1f; // 回転速度の倍率

    private void FixedUpdate()
    {
        var spinSpeed = spinInput.NowSpinSpeed;
        var maxSpinSpeed = spinInput.MaxSpinSpeed;

        // 最大速度を1とした場合の回転速度を計算
        float rotationAmount = spinSpeed / (float)maxSpinSpeed;

        // 回転量を計算
        float rotationAngle = rotationAmount * rotationSpeed * Time.fixedDeltaTime;
        // 回転方向を計算
        Vector3 rotationDirection = Vector3.up * rotationAngle;
        // オブジェクトを回転
        if ( spinObject != null )
        {
            spinObject.Rotate( rotationDirection, Space.World );
        }
        else
        {
            Debug.LogWarning( "SpinObject: spinObject is not assigned." );
        }
    }
}
