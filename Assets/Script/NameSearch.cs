using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using UnityEngine.UI;

public class NameSearch : MonoBehaviour
{
    public InputField inputField;
    NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("RankingData");


    public GameObject cleanPanel;
    public GameObject Score_Text = null;

    private string RANKING_PREF_KEY = "ranking";

    void Start()
    {
        cleanPanel.SetActive(false);
    }

    public void Click()
    {

        //入力
        string inputValue = inputField.text;
        cleanPanel.SetActive(true);

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            //Scoreフィールドの降順でデータを取得
            query.OrderByDescending("score");

            query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
            {
               
                if (e != null)
                {
                    //検索失敗時の処理
                }
                else
                {
                    int j = 1;

                    for (int i = 0; i < objList.Count; i++)
                    {

                        string _name = System.Convert.ToString(objList[i]["name"]); // 名前を取得
                        int _point_score = System.Convert.ToInt32(objList[i]["score"]); // スコアを取得
                        Text score_text = Score_Text.GetComponentInChildren<Text>();

                        //名前検索
                        if (inputValue == _name)
                        {
                            score_text.text = j + " " + _name + " " + _point_score + "\n";
                            break;
                        }
                        else
                        {
                            score_text.text = "その名前は登録されていません";
                        }
                        j++;
                    }
                }
            });
        }
        else if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            string _ranking = PlayerPrefs.GetString(RANKING_PREF_KEY);

            if (_ranking.Length > 0)
            {
                //stringをstring[]に
                string[] _rankingList = _ranking.Split(","[0]);

                for (int i = 0; i < _rankingList.Length; i++)
                {
                    string[] _rankName_Score = _rankingList[i].Split("."[0]);

                    string _name = System.Convert.ToString(_rankName_Score[1]); // 名前を取得
                    string _point_score = System.Convert.ToString(_rankName_Score[0]); // スコアを取得

                    Text score_text = Score_Text.GetComponentInChildren<Text>();
                    Debug.Log(_name.Contains(inputValue));

                    //名前検索
                    if (_name == inputValue)
                    {

                        score_text.text = i + 1 + " " + _name + " " + _point_score + "\n";
                        break;
                    }
                    else
                    {
                        score_text.text = "その名前は登録されていません";
                    }
                }
            }

        }
        InitInputField();
    }

    //戻るボタン、パネルを消す。
    public void DontClick()
    {
        cleanPanel.SetActive(false);
    }

    /// <summary>
    /// InputFieldの初期化用メソッド
    /// 入力値をリセットして、フィールドにフォーカスする
    /// </summary>
    void InitInputField()
    {
        //値をリセット
        inputField.text = "";

        //フォーカス
        inputField.ActivateInputField();

    }
}
