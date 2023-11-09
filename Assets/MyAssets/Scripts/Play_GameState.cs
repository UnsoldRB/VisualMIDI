//ゲームプレイ中に使用されるデータの格納庫。DataBox.csに対して、こちらは大抵の場合ゲーム中にデータがアップデートされる。
//また、ゲーム内で幅広く使用される関数もこちらに入っている。

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DB = RiceBall.Datas.Play_DataBox;

namespace RiceBall.Datas
{
    public static class Play_GameState
    {
        public static byte numKey { get; set; } = 0;                    //鍵盤の数。
        public static byte highestKey { get; set; } = 0;                //最高音のMIDIノートナンバー
        public static byte lowestKey { get; set; } = 0;                 //最低音のMIDIノートナンバー

        public static float keyXSize_W { get; set; } = 0.0f;            //白鍵1つに使えるX座標(幅)
        public static float keyXSize_B { get; set; } = 0.0f;            //黒鍵1つに使えるX座標(幅)
        public const float keyHeight = 0.2f;                            //画面の高さに対するKeyの高さ
        public const float splitterThick = 0.035f;                      //白鍵の幅に対するSPLITTERの太さ

        public static byte playState { get; set; } = 0;                 //ゲームの進行度合い。(0=鍵盤配置完了前, 1=鍵盤配置直後, 2=プレイ中, 3=ポーズ中, 4=結果発表)







        //コンストラクター。変数の初期化など。
        static Play_GameState()
        {
            
        }



        //対象のHierarchy内のパスを返す。
        public static string getPath(GameObject target)
        {
            List<GameObject> objects = new List<GameObject> { target };
            for (int i = 0; objects[i].transform.parent != null; i++)               //親がいなくなるまでオブジェクトの親を変数に格納する。
            {
                objects.Add(objects[i].transform.parent.gameObject);
            }
            objects.Reverse();                                                      //親から順番にループさせたいので反転させる。
            StringBuilder path = new StringBuilder("", 60);
            foreach (GameObject obj in objects)
            {
                path.Append(obj.name + "/");
            }
            return path.ToString().Substring(0, path.Length - 1);                   //最後の"/"を取り除いてreturn
        }



        //MIDIノートナンバーから音階を返す。
        public static string getCode(byte noteNum)
        {
            return DB.CODELIST[(byte)noteNum - DB.LOWEST_NOTE_NUM];
        }



        //CODELIST内の指定されたindexに対応している鍵盤が白鍵であるかどうかを返す。
        public static bool isWhiteKey(byte index)
        {
            return !getCode(index).Contains("/");
        }



        //使用する鍵盤の総数の内、黒鍵がいくつあるかを返す。
        public static byte blackKeyAmount(byte lowestKey, byte highestKey)
        {
            //使用する鍵盤の数に応じて最低音を調節する。
            byte count = 0;
            for (byte i = lowestKey; i <= highestKey; i++)
            {
                if (!isWhiteKey(i))
                {
                    count++;
                }
            }
            return count;
        }



        //指定した条件のオブジェクトにおける左下端のローカル座標を返す。
        public static Vector2 getBottomLeft(Vector2 size, Vector2 scale)
        {
            float x = size.x * scale.x / 2;
            float y = size.y * scale.y / 2;
            return new Vector2(-x, -y);
        }



        //効果音を再生する。(MainThreadOnly)
        public static void playSound(AudioClip sound, AudioSource source, float volume, float pitch)
        {
            if (source == null) return;         //AudioSourceが存在しない場合は何もしない。

            source.volume = volume;
            source.pitch = pitch;
            source.PlayOneShot(sound);
        }
    }
}