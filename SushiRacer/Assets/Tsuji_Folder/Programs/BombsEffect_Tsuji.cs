using UnityEngine;
using System.Collections.Generic;

public class BombsEffect_Tsuji : MonoBehaviour
{
    [SerializeField]
    List<GameObject> boms = new List<GameObject>();
    [SerializeField]
    private Exploded_Castle_Tsuji explodedCastle;

    private float coolTimer = 0.0f;
    private bool isCoolOn = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(explodedCastle == null) return;

        if(explodedCastle.GetExplodedFlag() == false)
            return;

        if(explodedCastle.GetFinishFlag() == true)
            return;

        if (boms.Count == 0)
        {
            return;
        }

        if (isCoolOn)
        {
            // クールダウン
            coolTimer += Time.fixedDeltaTime;
            if(coolTimer > 0.1f)
            {
                coolTimer = 0.0f;
                isCoolOn = false;
            }
        }
        else
        {
            int activateCount = 2; // 一度に有効化する数（例:3つ）
            int maxTries = 100;

            for (int j = 0; j < activateCount; j++)
            {
                for (int i = 0; i < maxTries; i++)
                {
                    int randomIndex = Random.Range(0, boms.Count);
                    if (!boms[randomIndex].activeSelf)
                    {
                        boms[randomIndex].SetActive(true);
                        break; // 次のbomb探しへ
                    }
                }
            }

            isCoolOn = true;
            return;
        }
    }
}
