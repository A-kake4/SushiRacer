using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusRequired : MonoBehaviour
{
    /// <summary>
    /// Selectable ���t�b�N����N���X�ł��B
    /// </summary>
    private class SelectionHooker : MonoBehaviour, IDeselectHandler
    {
        /// <summary>�e�R���|�[�l���g</summary>
        public FocusRequired Restrictor;

        /// <summary>
        /// �I���������ɒ��O�̑I�����L�^���܂��B
        /// </summary>
        public void OnDeselect( BaseEventData eventData )
        {
            Restrictor.PreviousSelection = eventData.selectedObject;
        }
    }

    /// <summary>�I�������Ȃ��I�u�W�F�N�g�ꗗ</summary>
    [SerializeField] private GameObject[] NotSelectables;

    /// <summary>���O�܂őI������Ă����I�u�W�F�N�g</summary>
    private GameObject PreviousSelection = null;

    /// <summary>�I��Ώۂ̃I�u�W�F�N�g�ꗗ</summary>
    private GameObject[] _selectables;

    /// <summary>�t�H�[�J�X����p�̃R���[�`���̎Q��</summary>
    private Coroutine _restrictCoroutine;

    private void Awake()
    {
        // ���ׂĂ� Selectable ���擾����i�W�F�l���b�N�ł��g�p�j
        //List<Selectable> selectableList = FindObjectsOfType<Selectable>().ToList();
        List<Selectable> selectableList = FindObjectsByType<Selectable>( FindObjectsSortMode.None ).ToList();

        // �I�����O������ꍇ�̓��X�g����폜
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

        // �e selectable �� SelectionHooker ���A�^�b�`
        foreach (var selectable in _selectables)
        {
            var hooker = selectable.AddComponent<SelectionHooker>();
            hooker.Restrictor = this;
        }

        // �t�H�[�J�X����R���[�`�����X�^�[�g
        _restrictCoroutine = StartCoroutine( RestrictSelection() );
    }

    /// <summary>
    /// �t�H�[�J�X���䏈��
    /// </summary>
    private IEnumerator RestrictSelection()
    {
        while (true)
        {
            // �C�x���g�V�X�e�������݂��A���I�����ω�����܂őҋ@
            yield return new WaitUntil( () =>
                EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject != PreviousSelection );

            var currentSelected = EventSystem.current.currentSelectedGameObject;

            // �܂��I�u�W�F�N�g�����I���A�܂��͋����X�g���Ȃ牽�����Ȃ�
            if (( PreviousSelection == null ) || _selectables.Contains( currentSelected ))
            {
                continue;
            }

            // ������Ă��Ȃ��I�u�W�F�N�g���I�����ꂽ�ꍇ�A���O�̑I���ɖ߂�
            EventSystem.current.SetSelectedGameObject( PreviousSelection );
        }
    }

    private void OnDestroy()
    {
        // �V�[���j�����ɃR���[�`�����~���ă��\�[�X���[�N��h�~
        if (_restrictCoroutine != null)
        {
            StopCoroutine( _restrictCoroutine );
        }
    }
}