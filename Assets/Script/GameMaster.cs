//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    // 盤の一辺の長さ（1マスを1とする）
    private const int boardLength = 8;
    // 乱数用
    private int ran;
    
    // 生成されるブロック番号（３つ）
    private int[] blockNum　= new int[3];
    // ブロックの生成位置用の枠
    public Transform[] generationFrame;
    // ブロックプレハブ用
    public Transform[] prefabBlocks;
    // ブロックごとの全gameObjectのTransform収納用
    private Transform[] blockObjs;


    // 盤の現在状況を格納 List<List<bool>>
    private List<List<bool>> board = new List<List<bool>>();
    // 各ブロックの形を格納 List<bool[,]>
    private List<bool[,]> block = new List<bool[,]>();
    // 各ブロックの子オブジェクト数
    private List<int> numberOfChildObjsOfblk = new List<int>();

    // 最初のフレーム更新の前に Start() が呼び出されます
    void Start()
    {
        BoardInit();

        BlockInit();
    }

    // 更新はフレームごとに1回呼び出されます
    void Update()
    {
        
    }


    /* ------------------------------------------------------------ */

    /// <summary>
    /// 盤の初期設定 (true:ブロックが置かれているマス、false:空いているマス)
    /// </summary>
    private void BoardInit()
    {
        // List[縦(z)]ごとに横(x) * 8 の false を追加
        for (int z = 0; z < boardLength; z++)
        {
            board.Add(new List<bool>());
            for (int x = 0; x < boardLength; x++)
            {
                board[z].Add(false);
            }
        }
    }

    /// <summary>
    /// ブロックの初期設定（各ブロックの形を格納する）
    /// </summary>
    private void BlockInit()
    {
        // block[PrefabBlock番号][横軸方向,縦軸]方向
        for (int i = 0; i < prefabBlocks.Length; i++)
        {            
            // 1つずつブロックの全オブジェクトのTransformを格納する
            blockObjs = prefabBlocks[i].gameObject.GetComponentsInChildren<Transform>();
            // 親ObjectのBoxColliderのsizeから x and z の大きさを取得して配列の要素数に使用する
            block.Add(new bool[(int)blockObjs[0].GetComponent<BoxCollider>().size.x, (int)blockObjs[0].GetComponent<BoxCollider>().size.z]);
            // .GetLength(次元数): 指定した配列の次元の長さを取得
            for (int x = 0; x < block[i].GetLength(0); x++)
            {
                for (int z = 0; z < block[i].GetLength(1); z++)
                {
                    // block List の二次元配列の初期化
                    block[i][x, z] = false;
                }
            }
            // ブロックごとに親を除いた子オブジェクト（Transform型）数を格納する
            numberOfChildObjsOfblk.Add(blockObjs.Length - 1);
            // ブロックごとの形を格納する
            foreach (Transform bo in blockObjs.Skip(1))
            {
                int x = 0, z = 0;
                // プレハブブロックの大きさ（x,z）は0.5fなので整数で整える
                x = (int)(bo.position.x * 2);
                z = (int)(bo.position.z * 2);

                block[i][x, z] = true;
            }
        }
    }

    /// <summary>
    /// ブロックの生成（３つ）
    /// </summary>
    private void Generate()
    {
        // 3つのブロックを生成する
        for (int i = 0; i < 3; i++)
        {
            // 乱数生成（0 ～ (PrefabBlock数 - 1)）
            ran = UnityEngine.Random.Range(0, prefabBlocks.Length);
            // 生成されるブロック番号を格納する
            blockNum[i] = ran;

            Vector3 centerPos = GetCenterPosition();
        }





    }

    // ブロックの中心を求める
    private Vector3 GetCenterPosition(Transform )
    {

    }




}
