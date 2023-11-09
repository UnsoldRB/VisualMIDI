//Addressableを使いたくなかったのでこのクラスをデータの格納庫として使用する。
//もしかしたらなんかあんまりよくないことをやっているのかもしれないが会社とかで経験積んでるわけじゃないからよくわからん...
//あと、後々使うかもしれないからっていう理由で、参照してるクラスが一つしかない定数がある...

namespace RiceBall.Datas
{
    public static class Play_DataBox
    {

        //画像素材の大きさ。[単位: Unit]
        //白鍵のXSIZEは、大抵の飾りに使う画像素材と等しい。
        //1pixel = 100ppu = 1unit
        public const float XSIZE_TEX_WKEY = 2.0f;
        public const float XSIZE_TEX_BKEY = 1.0f;
        public const float YSIZE_TEX_WKEY = 5.7f;
        public const float YSIZE_TEX_BKEY = 3.6f;
        public const float YSIZE_TEX_CHATCHER = 0.2f;

        //ゲームに使用できる鍵盤の最大・最小数。
        public const byte MAX_KEY_AMOUNT = 97;
        public const byte MIN_KEY_AMOUNT = 12;              //ドからシまでの間にある鍵盤の数と等しい。

        //ゲームに使用できる最低音のMIDIノートナンバー。
        public const byte LOWEST_NOTE_NUM = 12;

        //「ドレミファソラシ」のコード。
        private const string CODELIST_RAW = "C_C#/D♭_D_D#/E♭_E_F_F#/G♭_G_G#/A♭_A_A#/B♭_B";

        //コードのリスト(実際に他クラスから関数を通して参照するのはこっち。)
        public static string[] CODELIST { get; private set; } = new string[MAX_KEY_AMOUNT];

        //各素材のZ座標(奥行き)
        public const sbyte Z_BACKGROUND = 0;
        public const sbyte Z_RAIL = -1;
        public const sbyte Z_KEY_W = -2;
        public const sbyte Z_KEY_B = -3;
        public const sbyte Z_CATHCER = -4;
        public const sbyte Z_SPLITTER = -5;
        public const sbyte Z_NOTE = -6;

        public const float KEYS_SPAWN_TIME = 450f;          //全鍵盤の出現完了に要する時間
        public const float KEY_SPAWN_SPEED = 2f;            //鍵盤一つの出現に要する時間
        public const float KEY_EFFECT_SPEED = 12f;          //鍵盤が押されたときの演出の完了速度




        //コンストラクター。変数の初期化など。
        static Play_DataBox()
        {
            //CODELISTを組み立てる。
            string[] baseCode = CODELIST_RAW.Split("_");
            for (byte i = 0; i < MAX_KEY_AMOUNT; i++)
            {
                CODELIST[i] = baseCode[i % 12];
            }
        }
    }
}