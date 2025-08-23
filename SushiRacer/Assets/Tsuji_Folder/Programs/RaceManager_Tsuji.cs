using UnityEngine;
using System.Collections.Generic;

public class RaceManager_Tsuji : MonoBehaviour
{
    [SerializeField]
    private Transform[] checkPoints; // ���ׂẴE�F�C�|�C���g
    [SerializeField]
    private List<RacerProgress_Tsuji> racers; // ���ׂẴ��[�T�[�̐i��

    [SerializeField]
    private PlayerKeeper_Tsuji playerKeeper; // �v���C���[�̊Ǘ�

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject p1 = playerKeeper.GetPlayer1();
        GameObject p2 = playerKeeper.GetPlayer2();

        if (p1 != null && p2 != null)
        {
            RacerProgress_Tsuji rp1 = p1.GetComponent<RacerProgress_Tsuji>();
            RacerProgress_Tsuji rp2 = p2.GetComponent<RacerProgress_Tsuji>();
            if (rp1 != null && rp2 != null)
            {
                if (racers.Count < 2)
                {
                    racers.Add(rp1);
                    racers.Add(rp2);
                }
            }
            else
            {
              //  return;
            }
        }
        else
        {
          //  return; // �v���C���[���ݒ肳��Ă��Ȃ��ꍇ�͉������Ȃ�
        }

        if (racers.Count >= 2)
        {
            foreach (var racer in racers)
            {
                racer.UpdateProgress(checkPoints);
            }

            //racers.Sort((a, b) => b.GetTotalDistance().CompareTo(a.GetTotalDistance()));

            // �ʉߐ� �� ���� �̏��Ŕ�r
            racers.Sort((a, b) =>
            {
                int passedCompare = b.GetCheckPointCount().CompareTo(a.GetCheckPointCount());
                if (passedCompare != 0)
                    return passedCompare; // �ʉߐ��������������

                // �ʉߐ��������Ȃ狗���Ŕ�r
                return a.GetTotalDistance().CompareTo(b.GetTotalDistance());
            });

            for (int i = 0; i < racers.Count; i++)
            {
                racers[i].SetRank(i + 1); // �����L���O���X�V
            }
        }
    }
}
