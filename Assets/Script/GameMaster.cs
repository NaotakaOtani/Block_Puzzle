//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{

    // --------------------------------------------------
    //  Score

    // スコア表示
    private int score;
    // スコア表示用テキスト
    public Text scoreText;
    // スコア保存場所キー
    private string scoreKey = "totalScore";

    // --------------------------------------------------
    // Timer

    // 分
    private int minute;
    // 秒
    private float seconds;
    // 前のUpdate時の秒数
    private float oldSeconds;
    // タイマー表示用テキスト
    public Text timerText;

    // --------------------------------------------------
    // Pause

    // ポーズパネル
    public GameObject pausePanel;


    // --------------------------------------------------


    // 盤の一辺の長さ（1マスを1とする）
    private const int boardLength = 8;
    // 乱数用
    private int ran;
    // 使用済みブロックの数
    private int numberOfUsedBlks = 0;

    // Rayが衝突した対象がブロックかどうか
    private bool rayHitBlk = false;
    // 置きフラグ
    private bool putFlg = false;
    // タイムアップフラグ
    private bool timeUpflg = false;
    // ゲームオーバーフラグ
    private bool gameOverFlg = false;

    // ブロックの初期位置用
    private Vector3 initialPos;

    // Ray
    private Ray ray;
    // Raycas による取得した情報を得る構造体（ブロック、盤、盤上のPiece）
    private RaycastHit hitBlkInfo, hitBdInfo, hitPieceOnBd;
    // Raycas による取得した情報を得る構造体配列（子オブジェクト、X軸、Z軸）
    private RaycastHit[] hitObjs, hitXAxisObjs, hitZAxisObjs;

    // 生成されるブロック番号（３つ）
    private int[] blockNums　= new int[3];
    // ブロックの使用状況
    private bool[] usedBlks = new bool[3];

    // ブロックの生成位置用の枠
    public Transform[] generationFrame;
    // ブロックプレハブ用
    public Transform[] prefabBlocks;
    // ブロックごとの全gameObjectのTransform収納用
    private Transform[] blockObjs;
    // 掴んでいるブロック情報格納用
    private Transform[] currentBlk;

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

        ScoreInit();

        TimerInit();

        // Pauseパネルを非表示にする
        pausePanel.SetActive(false);
    }

    // 更新はフレームごとに1回呼び出されます
    void Update()
    {
        Timer();
        // ブロックを置かれた直後なら
        if (putFlg)
        {
            GetBoardSituation();

            GetUsedBlocks();

            gameOverFlg = GameOver();

            // 置き判定をfalseに変更する
            putFlg = false;
        }
        // GameOver
        if (gameOverFlg)
        {
            //リザルト画面へ
            Debug.Log("---------- Game Over ----------");
        }

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
            Put();

            rayHitBlk = false;
            // ブロックを置いたとき
            if (putFlg)
            {
                Delete();
            }
        }
        // スコアを表示する
        scoreText.text = score.ToString("000000");

        if (numberOfUsedBlks >= 3)
        {
            Generate();
            // 使用済みブロック数初期化
            numberOfUsedBlks = 0;
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
            blockNums[i] = ran;
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
    /// スコアの初期設定
    /// </summary>
    private void ScoreInit()
    {
        // scoreを0に戻す
        score = 0;
    }

    /// <summary>
    /// タイマーの初期設定
    /// </summary>
    private void TimerInit()
    {
        // １秒ずつ
        Time.timeScale = 1f;
        // ３分
        minute = 3;
        // ０秒
        seconds = 0f;
        oldSeconds = 0f;
    }

    /// <summary>
    /// タイマー（3分）
    /// </summary>
    private void Timer()
    {
        // 0秒のとき、分を１つ減らし、60秒に戻す
        if (seconds <= 0f)
        {
            minute--;
            seconds += 60;
        }
        // タイマーが0:01以上のときカウントダウン、又は0:00のときスコアを保存
        if ((minute) >= 0 && (seconds != 0f))
        {
            seconds -= Time.deltaTime;
        }
        else if ((minute == 0) && (seconds <= 0f))
        {
            // タイムアップ判定（true）
            timeUpflg = true;

            ScoreSave();

        }
        // 値が変わったときテキストUIを更新
        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minute.ToString("0") + ":" + ((int)seconds).ToString("00");
        }
        // 次のフレームで使うために現在の秒を代入
        oldSeconds = seconds;

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

    /// <summary>
    /// 掴んだブロックを盤に置く
    /// </summary>
    private void Put()
    {
        // 掴んでいるブロックの全オブジェクトのTransformを取得する
        currentBlk = hitBlkInfo.collider.gameObject.GetComponentsInChildren<Transform>();
        // 盤オブジェクトに衝突sしたBlockCube数
        int cubeCount = 0;

        foreach (Transform cb in currentBlk.Skip(1))
        {
            // 掴んでいるブロックの各子オブジェクトから光線を撃つ（子オブジェクトの少し手前から）
            ray = new Ray(cb.position + new Vector3(0, 1.1f, 0), cb.TransformDirection(new Vector3(0, -1, 0)));
            Debug.DrawRay(cb.position + new Vector3(0, 1.1f, 0), ray.direction * 5, Color.blue, 5);
            // ブロックから盤に向けて光線を撃ち、衝突した全てのオブジェクトを取得する
            hitObjs = Physics.RaycastAll(ray.origin, ray.direction, 5.0f);
            // Layer: Board
            int layerBoard = LayerMask.NameToLayer("Board");
            // 盤オブジェクトのときにカウントを１つ増やし、他のブロックピースが存在するならカウントを１つ減らす
            foreach (RaycastHit hos in hitObjs)
            {              
                if (hos.collider.gameObject.layer == layerBoard )
                {
                    cubeCount++;
                }
                else if (hos.collider.gameObject.tag == "Piece")
                {
                    cubeCount--;
                }
            }      
        }
        // 盤に掴んでいるブロックを置ける空きがあるなら
        if (cubeCount == currentBlk.Length - 1)
        {
            // 親オブジェクトの中心からの光線
            ray = new Ray(currentBlk[0].gameObject.transform.position, currentBlk[0].TransformDirection(new Vector3(0, -1, 0)));
            // 取得した盤オブジェクトの座標をブロックオブジェクトに代入する（盤に置く）
            if (Physics.Raycast(ray.origin, ray.direction, out hitBdInfo, 5.0f, LayerMask.GetMask("Board")))
            {     
                hitBlkInfo.collider.gameObject.transform.position = hitBdInfo.collider.gameObject.transform.position + new Vector3(0, 1, 0);
            }
            // ブロックを置いた後の子オブジェクトの処理
            foreach (Transform cb in currentBlk)
            {
                // 親オブジェクトから抜け出す
                cb.parent = null;
                // 子オブジェクトにタグ（Piece）を付ける
                cb.tag = "Piece";
            }

            // 親オブジェクトを即時削除する（Destroyの場合１フレームの間は存在しているので注意）
            DestroyImmediate(hitBlkInfo.collider.gameObject);
            // 使用済みブロック数を１つ増やす
            numberOfUsedBlks++;
            // 置き判定（true）
            putFlg = true;
            // ブロックを置いた時のポイント加算
            AddPoint(0);
        }
        else
        {
            // ブロックの縮小
            hitBlkInfo.collider.gameObject.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            // 掴んだブロックを初期位置に戻す
            hitBlkInfo.collider.gameObject.transform.position = initialPos;
        }

    }

    /// <summary>
    /// 一列揃ったPieceを削除する
    /// </summary>
    private void Delete()
    {
        // 揃った列の数
        int lineCount = 0;

        // 左から右へ↑方向にRayを撃つ
        for (int x = 0; x < boardLength; x++)
        {
            ray = new Ray(new Vector3(0.5f + x, 0.5f, -1), new Vector3(0, 0, 1));            
            hitXAxisObjs = Physics.RaycastAll(ray.origin, ray.direction, 10.0f);
            // 一列揃っているなら
            if (hitXAxisObjs.Length == 8)
            {
                foreach (RaycastHit ho in hitXAxisObjs)
                {
                    // 1つずつPieceを削除（1フレーム中は存在する）
                    Destroy(ho.collider.gameObject);
                }
                // ライン数を１つ増やす
                lineCount++;
            }
        }
        //　下から上へ→方向にRayを撃つ
        for (int z = 0; z < boardLength; z++)
        {
            ray = new Ray(new Vector3(-1, 0.5f, 0.5f + z), new Vector3(1, 0));
            hitZAxisObjs = Physics.RaycastAll(ray.origin, ray.direction, 10.0f);
            // 一列揃っているなら
            if (hitZAxisObjs.Length == 8)
            {
                foreach (RaycastHit ho in hitZAxisObjs)
                {
                    // 1つずつPieceを削除（1フレーム中は存在する）
                    Destroy(ho.collider.gameObject);
                }
                // ライン数を１つ増やす
                lineCount++;
            }
        }
        // ブロックが揃ったときのポイント加算
        AddPoint(lineCount);        
    }

    /// <summary>
    /// スコアのポイント加算
    /// </summary>
    public void AddPoint(int line)
    {
        // line: 0(ブロックが置かれたとき) line: 1～6(列が揃ったとき)
        if (line == 0)
        {
            score += 100;
        }
        else if ( 0 < line && line < 7 )
        {
            int point = 100 * line * 2;
            score += point;
        }
        
    }

    /// <summary>
    /// スコアデータをResultシーンへ持っていくための保存
    /// </summary>
    public void ScoreSave()
    {
        // 一時的にスコアデータをscoreKeyで用意した場所に保存する
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 盤面の状況を取得する
    /// </summary>
    private void GetBoardSituation()
    {
        // 盤配列の要素の値を初期化
        int x = 0, z = 0;
        
        for (float f = 0.5f; f < boardLength; f++)
        {
            for (float g = 0.5f; g < boardLength; g++)
            {
                // x:z = 0:0 マスの中心から盤面に向かって
                ray = new Ray(new Vector3(g, 1.1f, f), new Vector3(0, -1, 0));
                Physics.Raycast(ray.origin, ray.direction, out hitPieceOnBd, 1.5f);
                // 盤上にPieceがあるならtrue、そうでないならfalse
                if (hitPieceOnBd.collider.gameObject.tag == "Piece")
                {
                    board[z][x] = true;
                }
                else
                {
                    board[z][x] = false;
                }
                // xの値を１つ増やす
                x++;
            }
            // xの値を初期化
            x = 0;
            // zの値を１つ増やす
            z++;
        }
    }

    /// <summary>
    /// どのブロックが使われたかを取得する
    /// </summary>
    private void GetUsedBlocks()
    {
        int next = 0;

        for (int i = 0; i < 3; i++)
        {
            // 一番左のブロック生成位置の中心から、右のブロック生成位置の中心へ
            ray = new Ray(new Vector3(1 + next, 2f, -3), new Vector3(0, -1, 0));
            Debug.DrawRay(ray.origin, ray.direction * 2, Color.white, 3);
            // ブロックの生成位置に、まだ未使用のブロックがあるならtrue、そうでないならfalse
            if (Physics.Raycast(ray.origin, ray.direction, out hitBdInfo, 2.0f, LayerMask.GetMask("Block")))
            {
                usedBlks[i] = true;
            }
            else
            {
                usedBlks[i] = false;
            }
            // nextの値を３つ増やす
            next += 3;
        }
    }

    /// <summary>
    /// 盤にブロックがおけるかどうか１つずつ調べる
    /// </summary>
    /// <returns>空き有り or 無し</returns>
    private bool CanPutBlock(int blockNum)
    {
        // ブロックの形で空きのあるマス数
        int pieceCount = 0;
        
        // 盤のZ軸方向
        for (int i = 0; i < boardLength - block[blockNum].GetLength(1) + 1; i++)
        {
            // 盤のX軸方向
            for (int j = 0; j < boardLength - block[blockNum].GetLength(0) + 1; j++)
            {
                // ブロックのZ軸方向
                for (int z = 0; z < block[blockNum].GetLength(1); z++)
                {
                    // ブロックのX軸方向
                    for (int x = 0; x < block[blockNum].GetLength(0); x++)
                    {
                        // ブロック配列のｘ*ｚの範囲中、ブロックのない場所は飛ばす。そうではなく盤に空きがあるならカウントの値を１つ増やす                        
                        if (block[blockNum][x, z] == false)
                        {
                            continue;
                        }
                        else if (board[i + z][j + x] == false)
                        {
                            pieceCount++;
                        }
                        if (pieceCount == numberOfChildObjsOfblk[blockNum])
                        {
                            return true;
                        }
                    }                    
                }
                // pieceCountを初期化
                pieceCount = 0;
            }
        }
        // 盤にブロックを置ける場所が存在しない
        return false;
    }

    /// <summary>
    /// GameOver判定
    /// </summary>
    /// <returns>終了判定 or 続行判定</returns>
    private bool GameOver()
    {
        // 未使用ブロック数、置けないブロック数
        int usedBlkCount = 0, cannotPutCount = 0;
        
                
        for (int i = 0; i < 3; i++)
        {
            // 未使用ブロックのとき
            if (usedBlks[i] == true)
            {
                // 未使用ブロックを数える
                usedBlkCount++;
                Debug.Log("use:" + usedBlks[i]);
                Debug.Log("Emp:" + CanPutBlock(blockNums[i]));
                // 置き場所のないブロックを数える
                if (!(CanPutBlock(blockNums[i])))
                {
                    cannotPutCount++;
                }
            }                                         
        }
        Debug.Log("ubc:" + usedBlkCount);
        Debug.Log("cpc:" + cannotPutCount);
        // 未使用ブロック数を置き場所の無いブロック数が一致したなら
        if (usedBlkCount == cannotPutCount)
        {
            // GameOver
            return true;
        }
        // Game続行
        return false;
    }





    // BlockInit 確認用
    private void blkShow()
    {

        string s = "";

        for (int i = 0; i < 30; i++)
        {
            for (int x = 0; x < block[i].GetLength(0); x++)
            {
                for (int z = 0; z < block[i].GetLength(1); z++)
                {
                    s += block[i][x, z];
                }
                s += "\r\n";
            }
        }

        Debug.Log(s);
        
    }

}
