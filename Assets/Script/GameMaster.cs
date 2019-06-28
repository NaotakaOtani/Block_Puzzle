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
    
    // Rayが衝突した対象がブロックかどうか
    private bool rayHitBlk = false;

    // ブロックの初期位置用
    private Vector3 initialPos;

    // Ray
    private Ray ray;
    // Raycas による取得した情報を得る構造体（ブロック、）
    private RaycastHit hitBlkInfo;
    
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

        Generate();
    }

    // 更新はフレームごとに1回呼び出されます
    void Update()
    {

        // 左ボタンが押されたなら（タップ...）
        if (Input.GetMouseButtonDown(0))
        {
            rayHitBlk = Check();
        }
        // ブロックを掴んでいる間
        if (rayHitBlk)
        {
            Movement();
        }
        // 左ボタンが離されたなら（タップ...）かつ、ブロックを掴んでいるなら
        if (Input.GetMouseButtonUp(0) && rayHitBlk)
        {
            rayHitBlk = false;
        }
        
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
            // Centerの座標を求める
            Vector3 centerPos = GetCenterPosition(prefabBlocks[ran]);
            // Center から Pivot までどのくらいの距離があるか
            Vector3 distanceFromCtrToPvt = prefabBlocks[ran].position - centerPos;
            // プレハブブロックのインスタンスを生成
            Transform pb = Instantiate(prefabBlocks[ran].gameObject.transform);
            // 生成場所の座標と distanceFromCtrToPvt を足す
            pb.position = generationFrame[i].position + distanceFromCtrToPvt;

            // 求めた座標をそれぞれ代入
            float x = prefabBlocks[ran].position.x;
            float y = 0.5f;
            float z = prefabBlocks[ran].position.z;

            // インスタンスの座標を生成場所の中心に移動
            pb.position += new Vector3(x, y, z);

        }





    }

    /// <summary>
    /// ブロックの中心を求める
    /// </summary>
    /// <param name="blk">各ブロック</param>
    /// <returns>中心</returns>
    private Vector3 GetCenterPosition(Transform blk)
    {
        // ブロックの親、子 GameObject 全ての Collider と Renderer を取得し、存在する方を使用する
        Collider[] cols = blk.GetComponentsInChildren<Collider>(true);
        Renderer[] rens = blk.GetComponentsInChildren<Renderer>(true);
        // どちらも存在しない場合、blk.position を中心として返す
        if (cols.Length == 0 && rens.Length == 0)
        {
            return blk.position;
        }
        // 対角線の始点(minPos)と終点(maxPos)
        Vector3 minPos = Vector3.zero;
        Vector3 maxPos = Vector3.zero;

        bool isInit = true;

        for (int i = 0; i < cols.Length; i++)
        {
            var bounds = cols[i].bounds;
            var center = bounds.center;
            var size = bounds.size / 2;

            //最初の１度だけ通って、minPosとmaxPosを初期化する
            if (isInit)
            {
                minPos.x = center.x - size.x;
                minPos.y = center.y - size.y;
                minPos.z = center.z - size.z;
                maxPos.x = center.x + size.x;
                maxPos.y = center.y + size.y;
                maxPos.z = center.z + size.z;

                isInit = false;
                continue;
            }

            if (minPos.x > center.x - size.x) minPos.x = center.x - size.x;
            if (minPos.y > center.y - size.y) minPos.y = center.y - size.y;
            if (minPos.z > center.z - size.z) minPos.z = center.z - size.z;
            if (maxPos.x < center.x + size.x) maxPos.x = center.x + size.x;
            if (maxPos.y < center.y + size.y) maxPos.y = center.y + size.y;
            if (maxPos.z < center.z + size.z) maxPos.z = center.z + size.z;
        }

        for (int i = 0; i < rens.Length; i++)
        {
            var bounds = rens[i].bounds;
            var center = bounds.center;
            var size = bounds.size / 2;

            // Collider が１つもなければ１度だけ通って、minPosとmaxPosを初期化する
            if (isInit)
            {
                minPos.x = center.x - size.x;
                minPos.y = center.y - size.y;
                minPos.z = center.z - size.z;
                maxPos.x = center.x + size.x;
                maxPos.y = center.y + size.y;
                maxPos.z = center.z + size.z;

                isInit = false;
                continue;
            }

            if (minPos.x > center.x - size.x) minPos.x = center.x - size.x;
            if (minPos.y > center.y - size.y) minPos.y = center.y - size.y;
            if (minPos.z > center.z - size.z) minPos.z = center.z - size.z;
            if (maxPos.x < center.x + size.x) maxPos.x = center.x + size.x;
            if (maxPos.y < center.y + size.y) maxPos.y = center.y + size.y;
            if (maxPos.z < center.z + size.z) maxPos.z = center.z + size.z;
        }
        // 対角線の中心を返す
        return (minPos + maxPos) / 2;

    }

    /// <summary>
    /// クリック（タップ）した場所にブロックが存在するか判定する
    /// </summary>
    private bool Check()
    {
        // MainCamera からクリック（タップ）した場所に向かって光線を撃つ
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Rayが衝突した対象がブロックなら（Raycast(始点、方向、情報を保存する構造体、最大距離、レイヤー)）
        if (Physics.Raycast(ray.origin, ray.direction, out hitBlkInfo, 20.0f, LayerMask.GetMask("Block")))
        {
            // Ray が得た情報から初期位置を代入
            initialPos = hitBlkInfo.collider.gameObject.transform.position;
            // ブロックの拡大
            hitBlkInfo.collider.gameObject.transform.localScale = new Vector3(1, 1, 1);
            // ブロックが存在
            return true;
        }
        return false;
    }

    /// <summary>
    /// 掴んだブロックを移動させる
    /// </summary>
    private void Movement()
    {
        // マウスの座標を取得する
        Vector3 mousePos = Input.mousePosition;
        //
        mousePos.z = 14.5f;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        hitBlkInfo.collider.gameObject.transform.position = screenPos;
    }


    private void Put()
    {

    }


}
