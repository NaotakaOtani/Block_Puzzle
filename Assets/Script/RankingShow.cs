using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;

public class RankingShow : MonoBehaviour
{

    public GameObject score_object = null;
    public Transform score_content = null;

    //ランキング保存場所Key
    private string RANKING_PREF_KEY = "ranking";

    // Start is called before the first frame update
    void Start()
    {

        //WiFi でアクセス可能
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("RankingData");

            //Scoreフィールドの降順でデータを取得
            query.OrderByDescending("score");

            //検索件数100件に設定
            query.Limit = 100;

            //データストアでの検索を行う
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if (e != null)
                {

                }
                else
                {

                    List<string> nameList = new List<string>(); // 名前のリスト
                    List<int> scoreList = new List<int>(); // スコアのリスト
                    int j = 1;
                    for (int i = 0; i < objList.Count; i++)
                    {
                        var item = GameObject.Instantiate(score_object.GetComponent<RectTransform>()) as RectTransform;
                        item.SetParent(score_content, false);

                        string s = System.Convert.ToString(objList[i]["name"]); // 名前を取得
                        int n = System.Convert.ToInt32(objList[i]["score"]); // スコアを取得

                        Text[] rankingText = item.GetComponentsInChildren<Text>();
                        
                        rankingText[0].text = j.ToString();
                        rankingText[1].text = s;
                        rankingText[2].text = n.ToString();
                        j++;
                    }
                }
            });
            Debug.Log("ネットワークある");
        }
        //WiFi アクセス不可
        else if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("ネットワークが使えない");
            string _ranking = PlayerPrefs.GetString(RANKING_PREF_KEY);
            if (_ranking.Length > 0)
            {
                //stringをstring[]に
                string[] _rankingList = _ranking.Split(","[0]);
                int j = 1;

                Debug.Log("ranking:" + _rankingList.Length);
                for (int i = 0; i < _rankingList.Length; i++)
                {
                    Debug.Log("roopstr");

                    var item = GameObject.Instantiate(score_object.GetComponent<RectTransform>()) as RectTransform;
                    item.SetParent(score_content, false);

                    string[] _rankName_Score = _rankingList[i].Split("."[0]);

                    string _name = System.Convert.ToString(_rankName_Score[1]); // 名前を取得
                    string _point_score = System.Convert.ToString(_rankName_Score[0]); // スコアを取得


                    //文字列ごとに空白
                    for (int z = (_name.Length - 1); z < 10; z++)
                    {
                        Debug.Log("cnt;" + z);
                        _name += " ";
                    }

                    Text score_text = item.GetComponentInChildren<Text>();
                    //順位の桁ごとに空白追加
                    if (j < 10)
                    {
                        score_text.text += j + " " + _name + " " + _point_score + "\n";

                    }
                    else if (j >= 10 && j < 100)
                    {
                        score_text.text += j + "  " + _name + " " + _point_score + "\n";
                    }
                    j++;
                    Debug.Log(score_text.text);
                }

            }
            //ランキングデータがない時（オフライン）
            else if (_ranking.Length == 0)
            {
                var item = GameObject.Instantiate(score_object.GetComponent<RectTransform>()) as RectTransform;
                item.SetParent(score_content, false);

                Text score_text = item.GetComponentInChildren<Text>();
                score_text.text += "No Score";
            }
        }

    }
}
