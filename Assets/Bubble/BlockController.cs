
using UnityEngine;
using System.Collections;

public class BlockController:MonoBehaviour{
    /*
     �A�j���[�V�����̃X�N���v�g
     */
	private Animation anim;
    //�ǉ����Ă���A�j���[�V�������Ƃ��Ă���
    public void Start(){
		anim=GetComponent<Animation>();
	}

  public void Update(){

   /*
    �A�j���[�V������Block���X�y�[�X������������s����B 
   */
	if(Input.GetKeyDown(KeyCode.Space)){
        anim.Play("Bubble");
    }
  }
	
}
