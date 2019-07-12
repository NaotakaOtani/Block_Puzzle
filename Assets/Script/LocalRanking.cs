using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalRanking : MonoBehaviour
{

    //ランキング保存場所Key
    private string RANKING_PREF_KEY = "ranking";
    private int RANKING_NUM = 10;
    List<string> ranking = new List<string>();

    //スコア一時保存場所Key(Debugのみの使用)
    private string scoreKey = "totalScore";
    private int resultScore;

    private string _new_score;
    private string _new_name;

    //新しいデータを最後尾に追加
    public void getRanking()
    {
        string _ranking = PlayerPrefs.GetString(RANKING_PREF_KEY);
        //ランキングデータがあれば新しいデータを最後に挿入
        if (_ranking.Length > 0)
        {
            string[] _score = _ranking.Split(","[0]);
            ranking.AddRange(_score);
            //Listの最後に新しいデータを挿入
            ranking.Add(_new_score + "." + _new_name);
            Debug.Log("ranking:" + ranking.Count);

        }
        else
        {
            ranking.Add(_new_score + "." + _new_name);
            Debug.Log("ranking:" + ranking.Count);
        }
    }

    //ランキングデータを並び替えてセーブ
    public void saveRanking(string new_score, string new_name)
    {
        Debug.Log("new_score:" + new_score);
        Debug.Log("new_score:" + new_name);

        //引数を変数に
        _new_score = new_score.ToString();
        _new_name = new_name;

        //ランキングデータを取ってくる
        getRanking();
        string[] rankingList = ranking.ToArray();
        if (rankingList.Length > 0)
        {
            // 降順に並べる
            BubbleSort(rankingList);
        }
        string ranking_string = string.Join(",", rankingList);
        PlayerPrefs.SetString(RANKING_PREF_KEY, ranking_string);

    }
    // バブルソート
    public string[] BubbleSort(string[] array)
    {
        int ranking_Length = array.Length;
        // 最終要素を除いて全て比較する
        for (int i = 0; i < ranking_Length - 1; i++)
        {
            // 最終要素から、現在比較中の要素までループ
            // このループが終われば array[i] にはソート済のデータが入っている
            for (int j = ranking_Length - 1; i < j; j--)
            {
                string[] right_tmp = array[j].Split("."[0]);
                int right = int.Parse(right_tmp[0]);

                string[] left_tmp = array[j - 1].Split("."[0]);
                int left = int.Parse(left_tmp[0]);

                // j番目の要素が一つ前の要素より大きいならばスワップ
                if (right.CompareTo(left) > 0)
                {
                    Swap(ref array[j], ref array[j - 1]);
                }
            }
        }
        return array;
    }

    public void Swap<T>(ref T a, ref T b)
    {
        var tmp = a;
        a = b;
        b = tmp;
    }

    //ランキングデータを消去するときに
    public void deleteRanking()
    {
        PlayerPrefs.DeleteKey(RANKING_PREF_KEY);
        string rank = PlayerPrefs.GetString(RANKING_PREF_KEY);
        Debug.Log("rank:" + rank);
    }

 
}
