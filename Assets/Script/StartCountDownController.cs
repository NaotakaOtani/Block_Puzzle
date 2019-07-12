using UnityEngine;

public class StartCountDownController : MonoBehaviour
{
    public bool flag;
    void Start()
    {
        flag = false;
    }
    // Update is called once per frame
    public void StartCountMethod(bool flag)
    {
        this.flag = flag;
        if (this.flag)
        {
            GetComponent<Animator>().SetTrigger("StartCountDownTrigger");
            this.flag = false;
        }
    }
}
