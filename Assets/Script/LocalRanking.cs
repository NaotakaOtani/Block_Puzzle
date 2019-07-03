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
        string[] rankingList = new string[ranking.Count];
        if (ranking.Count > 0)
        {
            //降順に並べる
            var renkingDec = ranking.OrderByDescending(value => value);
            //Listを配列に
            rankingList = renkingDec.ToArray();
        }
        string ranking_string = string.Join(",", rankingList);
        PlayerPrefs.SetString(RANKING_PREF_KEY, ranking_string);

    }

    //ランキングデータを消去するときに
    public void deleteRanking()
    {
        PlayerPrefs.DeleteKey(RANKING_PREF_KEY);
        string rank = PlayerPrefs.GetString(RANKING_PREF_KEY);
        Debug.Log("rank:" + rank);
    }

 
}
