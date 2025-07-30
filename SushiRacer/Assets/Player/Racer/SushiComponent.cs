#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
[CustomEditor( typeof( BaseComponent<SushiItem, SushiDataScriptableObject> ), true )]
public class SushiComponentEditor : BaseComponentEditor{}
#endif


public class SushiComponent : BaseComponent<SushiItem, SushiDataScriptableObject>
{
    public SushiItem GetSushiData()
    {
        return dataSource.GetItem(selectedItemNumber);
    }

    private void Start()
    {
        // キーボードの入力設定
        //InputManager.Instance.RegisterPlayerDevice(1,Keyboard.current);
        //InputManager.Instance.RegisterPlayerDevice(1,Mouse.current);

        // ゲームパッドの入力設定
        InputManager.Instance.RegisterPlayerDevice( 1, Gamepad.current );
    }
}