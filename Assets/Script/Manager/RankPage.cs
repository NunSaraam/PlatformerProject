using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;  // 정렬 해주는거
using TMPro;
using static StageDataManager;
using System;

public class RankPage : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] GameObject rowPrefab;

    StageResultList allData;

    int changeRank = 1;

    private void Awake()
    {
        allData = StageResultSaver.LoadRank();
        RefreshRankList();
    }

    public void ChangeRank(int index)
    {
        changeRank = index;

        RefreshRankList();
    }

    void RefreshRankList()
    {
        foreach(Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        var sortedData = allData.results.Where(r => r.stage == changeRank).OrderByDescending(x => x.score).ToList();


        for (int i = 0; i < sortedData.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, contentRoot);
            TMP_Text rankText = row.GetComponentInChildren<TMP_Text>();
            rankText.text = $"{i + 1}. {sortedData[i].playerName} - {sortedData[i].score}";
        }
    }
}
