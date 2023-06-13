using UnityEngine;
using DB = DataBox;

public class PlayerBuilder : MonoBehaviour
{
    [SerializeField]
    Camera CAM;
    [SerializeField]
    GameObject RAIL;
    [SerializeField]
    GameObject NOTE;
    [SerializeField]
    GameObject KEY_WHITE;
    [SerializeField]
    GameObject KEY_BLACK;


    //keyの数。
    const byte numKey = 88;
    //画面の高さに対するKeyの高さ
    const float keyHeight = 0.2f;

    //各素材のZ座標(奥行き)
    const sbyte Z_RAIL = 0;
    const sbyte Z_KEY_W = -1;
    const sbyte Z_KEY_B = -2;
    const sbyte Z_CATHCER = -3;
    const sbyte Z_NOTE = -4;


    //白鍵・黒鍵1つに使えるX座標(幅)
    float keyXSize_W = 0.0f;
    float keyXSize_B = 0.0f;





    void Start()
    {
        buildRails();
    }



    void Update()
    {
        
    }



    //画面にレールを並べる関数
    private void buildRails()
    {
        //ゲーム内に適用する画像のサイズを計算するために各端の座標を取得する...
        Vector2 startVec = CAM.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector2 endVec = CAM.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float leftX = startVec.x;
        float bottomY = startVec.y;
        float rightX = endVec.x;
        float upperY = endVec.y;


        //Key一つに使うX座標(幅)を白鍵黒鍵それぞれ計算する...
        float windowSize_X = rightX - leftX; //画面の幅 [単位: Unit]
        byte numKey_W = (byte)(numKey - DB.blackKeyAmount(numKey)); //白鍵の数
        float scaleMultiplier = windowSize_X / (numKey_W * DB.XSIZE_TEX_W); //白鍵をそのまま全て並べた場合に対する画面の幅の割合(レールの幅にもこれを用いる)

        //Key一つに使うX座標(幅) [単位: Unit]
        keyXSize_W = DB.XSIZE_TEX_W * scaleMultiplier; //レールにも使う。
        keyXSize_B = DB.XSIZE_TEX_B * scaleMultiplier;


        //Key一つに使うY方向のScale(高さ)をそれぞれ計算する...
        float windowSize_Y = upperY - bottomY; //画面の高さ [単位: Unit]
        float idealKeyHeight = windowSize_Y * keyHeight;
        float keyHeightRatio = DB.YSIZE_TEX_B / DB.YSIZE_TEX_W;

        //Key一つに使うY方向のScale(高さ)
        float keyYSize_W = idealKeyHeight / DB.YSIZE_TEX_W;
        float keyYSize_B = (idealKeyHeight * keyHeightRatio) / DB.YSIZE_TEX_B;


        //黒鍵に加算するXY座標
        float keySizeDifference = DB.XSIZE_TEX_W - DB.XSIZE_TEX_B;
        float AddCoord_X_B = keySizeDifference * scaleMultiplier;
        float AddCoord_Y_B = keySizeDifference * scaleMultiplier;

        //レールを縦方向に画面いっぱいに伸ばす時に必要な係数の大きさ
        float railYScaleMultiplier = windowSize_Y / DB.SIZE_TEX_RAIL;

        //NUM_KEYの数だけ実際にレールとキーを並べて、同時に大きさも調整する...
        GameObject for_Obj; //生成したobjectが一時的に入る。
        Vector3 for_Scale; //生成したobjectの大きさが一時的に入る。
        byte for_WKeyCount = 0; //この値に合わせて鍵盤を設置するX座標をずらしていく。
        byte lowestKey = DB.getLowestCode(numKey); //使用する鍵盤の数に応じて最低音が変わる。


        for (byte i = lowestKey; i < DB.MAX_KEY_AMOUNT; i++)
        {
            if (DB.isWhiteKey((byte)(i))) //白鍵なら...
            {
                //レールを並べて大きさを調整する
                for_Obj = Instantiate(RAIL, new Vector3(leftX + (keyXSize_W * for_WKeyCount), bottomY, Z_RAIL), Quaternion.identity);
                for_Scale = for_Obj.transform.localScale;
                for_Obj.transform.localScale = new Vector3(for_Scale.x * scaleMultiplier, for_Scale.y * railYScaleMultiplier, 1);

                //白鍵を並べる
                for_Obj = Instantiate(KEY_WHITE, new Vector3(leftX + (keyXSize_W * for_WKeyCount), bottomY, Z_KEY_W), Quaternion.identity);
                for_Scale = for_Obj.transform.localScale;
                for_Obj.transform.localScale = new Vector3(for_Scale.x * scaleMultiplier, keyYSize_W, 1);

                for_WKeyCount++;
            }
            else
            {
                //黒鍵を並べる
                for_Obj = Instantiate(KEY_BLACK, new Vector3(leftX + (keyXSize_B * for_WKeyCount) + AddCoord_X_B, bottomY + AddCoord_Y_B, Z_KEY_B), Quaternion.identity);
                for_Scale = for_Obj.transform.localScale;
                for_Obj.transform.localScale = new Vector3(for_Scale.x * scaleMultiplier, keyYSize_B, 1);
            }
        }
    }
}
