using UnityEngine;
using UnityEngine.UI;
using NCMB;
using UnityEngine.SceneManagement;

public class SendToDatabase : MonoBehaviour
{

    [SerializeField]
    public int resultScore;

    public Text resultText;

    public InputField inputField;

    //スコア保存場所Key
    private string scoreKey = "totalScore";

    LocalRanking localRanking;
    GameObject getResultBoard;

    // Start is called before the first frame update
    void Start()
    {
        getResultBoard = GameObject.Find("ResultBoard");
        localRanking = getResultBoard.GetComponent<LocalRanking>();

        //inputField = GetComponent<InputField>();

        resultScore = PlayerPrefs.GetInt(scoreKey);
        resultText.text = resultScore.ToString("000000");


        InitInputField();
    }

    public void InputLogger()
    {

        string inputValue = inputField.text;

        //クラスのNCMBObjectを作成
        NCMBObject rankingdata = new NCMBObject("RankingData");
        //RankingData = データベースの名前

        if (inputValue.Length >= 1)
        {
            //オブジェクトに値を設定
            rankingdata["name"] = inputValue;
            rankingdata["score"] = resultScore;
            // name,score は　データベースのレコード
            //データストアへの登録
            rankingdata.SaveAsync();


            localRanking.saveRanking(resultScore.ToString(), inputValue);
        }


        InitInputField();

        SceneManager.LoadScene("Title");
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
