
﻿using System.Collections;
using UnityEngine;

public class BubbleController:MonoBehaviour{

  private ParticleSystem particle;
    /*
    泡のスクリプト
    */
    public void Start(){
    //追加しているパーティクルシステムをとってくる
    particle=GetComponent<ParticleSystem>();
   
    particle.Stop();
  }

  public void Update(){
    //パーティクルシステムをスペースを押したら実行する。
	if(Input.GetKeyDown(KeyCode.Space)){
      particle.Play();
    }
  }

}
