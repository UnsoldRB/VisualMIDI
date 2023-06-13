//Addressableを使いたくなかったのでこのクラスをデータの格納庫として使用する。
//もしかしたらなんかあんまりよくないことをやっているのかもしれないが会社とかで経験積んでるわけじゃないからよくわからん...
//あと、後々使うかもしれないからっていう理由で、参照してるクラスが一つしかない定数があるけど...ﾕﾙｼﾃ...

public static class DataBox
{

    //白鍵(W)・黒鍵(B)の画像素材の大きさ。[単位: Unit]
    //動的に画像サイズを拡縮しても画像の位置がズレないように、画像自体に余白を入れてあるため、画像本来のサイズとは異なる。
    //1pixel = 100ppu = 1unit
    public const float XSIZE_TEX_W = 2.0f;
    public const float XSIZE_TEX_B = 1.0f;
    public const float YSIZE_TEX_W = 5.7f;
    public const float YSIZE_TEX_B = 3.6f;

    //レールの一辺の長さ(レールは正方形)[単位: Unit]
    public const float SIZE_TEX_RAIL = XSIZE_TEX_W;

    //ゲームに使用できる鍵盤の最大数。
    public const byte MAX_KEY_AMOUNT = 97;

    //コードのリスト(raw)。「_」で各コードが区切られている。
    private const string CODELIST_RAW = "C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B_C";
    
    //コードのリスト(実際に他クラスから参照するのはこっち。)
    public static string[] CODELIST = null;






    //コンストラクター。変数の初期化など。
    static DataBox()
    {
        CODELIST = CODELIST_RAW.Split("_");
    }



    //使用する鍵盤数における最低音を返す。
    public static byte getLowestCode(byte keyAmount)
    {
        return (byte)(MAX_KEY_AMOUNT - keyAmount + 1);
    }



    //CODELIST内の指定されたindexに対応している鍵盤が白鍵であるかどうかを返す。
    public static bool isWhiteKey(byte index)
    {
        return !CODELIST[index - 1].Contains("/");
    }



    //使用する鍵盤の総数の内、黒鍵がいくつあるかを返す。
    public static byte blackKeyAmount(byte keyAmount)
    {
        //使用する鍵盤の数に応じて最低音を調節する。
        byte LOWEST_KEY = getLowestCode(keyAmount);
        byte count = 0;
        for (byte i = LOWEST_KEY; i <= MAX_KEY_AMOUNT; i++)
        {
            if (!isWhiteKey(i))
            {
                count++;
            }
        }
        return count;
    }
}