using UnityEngine;
using System.Collections.Generic;

public class RaceManager_Tsuji : MonoBehaviour
{
    [SerializeField]
    private Transform[] waypoints; // すべてのウェイポイント
    [SerializeField]
    private List<RacerProgress_Tsuji> racers; // すべてのレーサーの進捗

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var racer in racers)
        {
            racer.UpdateProgress(waypoints);
        }

        racers.Sort((a, b) => b.GetTotalDistance().CompareTo(a.GetTotalDistance()));

        for(int i = 0; i < racers.Count; i++)
        {
            racers[i].SetRank(i + 1); // ランキングを更新
        }

    }
}
