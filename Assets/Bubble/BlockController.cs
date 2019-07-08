
using UnityEngine;
using System.Collections;

public class BlockController:MonoBehaviour{
    /*
     アニメーションのスクリプト
     */
	private Animation anim;
    //追加しているアニメーションをとってくる
    public void Start(){
		anim=GetComponent<Animation>();
	}

  public void Update(){

   /*
    アニメーションのBlockをスペースを押したら実行する。 
   */
	if(Input.GetKeyDown(KeyCode.Space)){
        anim.Play("Bubble");
    }
  }
	
}
