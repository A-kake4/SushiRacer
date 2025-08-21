using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SealPercent
{
    None = -1,
    Percent_30,
    Percent_50,
    Percent_100
}

public class SequentialOperation_Tsuji : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> childList = new List<RectTransform>();

    private SealPercent nowPasteSeal = SealPercent.None;

    public void SequentialOperationChildren()
    {
        if (childList.Count >= 3)
        {
            foreach (RectTransform img in childList)
            {
                ShrinkAndMove_Tsuji sm = img.GetComponent<ShrinkAndMove_Tsuji>();
                if (sm.GetMySeal() == nowPasteSeal)
                {
                    img.SetAsLastSibling(); // 最前面に
                }
            }
        }
    }

    public void SetPasteSeal(SealPercent sealType)
    {
        if (childList.Count >= 3)
        {
            string sealName = "";
            switch (sealType)
            {
                case SealPercent.Percent_30:
                    // 30%のシール処理
                    sealName = "WasabiSeal_30";
                    break;
                case SealPercent.Percent_50:
                    // 50%のシール処理
                    sealName = "WasabiSeal_50";
                    break;
                case SealPercent.Percent_100:
                    // 100%のシール処理
                    sealName = "WasabiSeal_100";
                    break;
                default:
                    break;
            }

            if (sealName == "")
            {
                foreach (RectTransform img in childList)
                {
                    ShrinkAndMove_Tsuji sm = img.GetComponent<ShrinkAndMove_Tsuji>();
                    sm.TransformReset();
                }
                return;
            }
            for (int i = 0; i < childList.Count; ++i)
            {
                if (childList[i].name == sealName)
                {
                    ShrinkAndMove_Tsuji shm = childList[i].GetComponent<ShrinkAndMove_Tsuji>();
                    if (shm != null)
                    {
                        shm.SetMovingFlag(true);
                    }
                }
            }
        }
    }

    public void SetNowPasteSeal(SealPercent sealType)
    {
        nowPasteSeal = sealType;
    }
}
