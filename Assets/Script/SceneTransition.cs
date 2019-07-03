using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Homeへ遷移
    public void HomeOnClick()
    {
        SceneManager.LoadScene("Home");
    }
    // Ruleへ遷移
    public void RuleOnClick()
    {
        SceneManager.LoadScene("Rule");
    }
    // Gameへ遷移
    public void GameOnClick()
    {
        SceneManager.LoadScene("Game");
    }
    // Rankingへ遷移
    public void RankingOnClick()
    {
        SceneManager.LoadScene("Ranking");
    }
}
