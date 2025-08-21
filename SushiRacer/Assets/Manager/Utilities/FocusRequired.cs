using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusRequired : MonoBehaviour
{
    /// <summary>
    /// Selectable をフックするクラスです。
    /// </summary>
    private class SelectionHooker : MonoBehaviour, IDeselectHandler
    {
        /// <summary>親コンポーネント</summary>
        public FocusRequired Restrictor;

        /// <summary>
        /// 選択解除時に直前の選択を記録します。
        /// </summary>
        public void OnDeselect( BaseEventData eventData )
        {
            Restrictor.PreviousSelection = eventData.selectedObject;
        }
    }

    /// <summary>選択させないオブジェクト一覧</summary>
    [SerializeField] private GameObject[] NotSelectables;

    /// <summary>直前まで選択されていたオブジェクト</summary>
    private GameObject PreviousSelection = null;

    /// <summary>選択対象のオブジェクト一覧</summary>
    private GameObject[] _selectables;

    /// <summary>フォーカス制御用のコルーチンの参照</summary>
    private Coroutine _restrictCoroutine;

    private void Awake()
    {
        // すべての Selectable を取得する（ジェネリック版を使用）
        //List<Selectable> selectableList = FindObjectsOfType<Selectable>().ToList();
        List<Selectable> selectableList = FindObjectsByType<Selectable>( FindObjectsSortMode.None ).ToList();

        // 選択除外がある場合はリストから削除
        if (NotSelectables != null && NotSelectables.Length > 0)
        {
            foreach (var item in NotSelectables)
            {
                if (item == null)
                    continue;

                var sel = item.GetComponent<Selectable>();
                if (sel != null)
                    selectableList.Remove( sel );
            }
        }

        _selectables = selectableList.Select( x => x.gameObject ).ToArray();

        // 各 selectable に SelectionHooker をアタッチ
        foreach (var selectable in _selectables)
        {
            var hooker = selectable.AddComponent<SelectionHooker>();
            hooker.Restrictor = this;
        }

        // フォーカス制御コルーチンをスタート
        _restrictCoroutine = StartCoroutine( RestrictSelection() );
    }

    /// <summary>
    /// フォーカス制御処理
    /// </summary>
    private IEnumerator RestrictSelection()
    {
        while (true)
        {
            // イベントシステムが存在し、かつ選択が変化するまで待機
            yield return new WaitUntil( () =>
                EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject != PreviousSelection );

            var currentSelected = EventSystem.current.currentSelectedGameObject;

            // まだオブジェクトが未選択、または許可リスト内なら何もしない
            if (( PreviousSelection == null ) || _selectables.Contains( currentSelected ))
            {
                continue;
            }

            // 許可されていないオブジェクトが選択された場合、直前の選択に戻す
            EventSystem.current.SetSelectedGameObject( PreviousSelection );
        }
    }

    private void OnDestroy()
    {
        // シーン破棄時にコルーチンを停止してリソースリークを防止
        if (_restrictCoroutine != null)
        {
            StopCoroutine( _restrictCoroutine );
        }
    }
}