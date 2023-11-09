//このシーン(Play)では、まずはこのスクリプトが動作し始める。

using System;
using System.Collections.Generic;
using UnityEngine;
using DB = RiceBall.Datas.Play_DataBox;
using GS = RiceBall.Datas.Play_GameState;
using IP = RiceBall.Input.Play_Input;

namespace RiceBall.Mains
{
    public class Play_Builder : MonoBehaviour
    {
        [SerializeField]
        Camera CAM; //root
        [SerializeField]
        GameObject BACKGROUND;
        [SerializeField]
        GameObject RAIL;
        [SerializeField]
        GameObject KEY;
        [SerializeField]
        GameObject CATHCER;
        [SerializeField]
        GameObject SPLITTER;
        [SerializeField]
        GameObject NOTE;
        [SerializeField]
        AudioSource MY_AUDIOSOURCE;                                 //このスクリプトが再生する全ての音の再生を担う。
        [SerializeField]
        AudioClip KEYS_PLACED;
        [SerializeField]
        GameObject MIDI_READER;

        public bool isBuilt { get; set; } = false;                  //鍵盤を生成済みであるかどうか。
        private List<byte> pressedKeys = new List<byte>();          //現在入力されている鍵盤一覧。


        private const float BKEY_MODIFYFACTOR = 3f;
        private const string BKEY_LEFT = "C#/D♭, F#/G♭";          //この文字列に含まれるコードの鍵盤は若干左寄りに配置される。
        private const string BKEY_RIGHT = "D#/E♭, A#/B♭";         //この文字列に含まれるコードの鍵盤は若干右寄りに配置される。







        void Start()
        {
            IP.getDevices();
            IP.OnPressKey += ReceiveMsg;        //Play_Inputのイベントにメソッドを登録。
        }



        void Update()
        {
            if (!isBuilt)                                   //鍵盤がまだ生成されていない場合。
            {
                if (GS.numKey != 0 && pressedKeys.Count == 0)       //鍵盤の生成が完了しておらず、どの鍵盤も押されていないとき。
                {
                    buildRails();
                    isBuilt = true;
                }
                else if (GS.numKey == 0)                            //鍵盤の生成が完了していない場合。
                {
                    //現在押されている鍵盤が2つ、かつ必要最低限の鍵盤数がその2つの間に存在する場合。
                    if (pressedKeys.Count != 2) return;
                    if (Math.Abs(pressedKeys[0] - pressedKeys[1] + 1) < DB.MIN_KEY_AMOUNT) return;

                    pressedKeys.Sort();                                 //List内の値が昇順になる。

                    //情報を格納する。
                    GS.lowestKey = pressedKeys[0];
                    GS.highestKey = pressedKeys[1];
                    GS.numKey = (byte)(GS.highestKey - GS.lowestKey + 1);
                }
            }
            else if (GS.playState == 1)                     //鍵盤が生成された直後。
            {
                GS.playSound(KEYS_PLACED, MY_AUDIOSOURCE, 1f, 1f);

                //MIDIの読み取りを行うprefabを生成する。
                Instantiate(MIDI_READER);

                GS.playState = 2;

                //この後の処理は生成したprefabのスクリプトで行われる。
            }
        }



        private void OnApplicationQuit()
        {
            IP.disconnectDevice();
        }



        //鍵盤が押されたとき。
        private void ReceiveMsg(byte num)
        {
            //鍵盤が生成済みであるなら動作しない。
            if (!isBuilt)
            {
                //入力されている鍵盤のMIDIノートナンバーを格納する。
                if (pressedKeys.Contains(num))
                {
                    pressedKeys.Remove(num);
                }
                else
                {
                    pressedKeys.Add(num);
                }
            }
           
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

            float windowSize_X = rightX - leftX;                                                //画面の幅 [単位: Unit]
            float windowSize_Y = upperY - bottomY;                                              //画面の高さ [単位: Unit]


            //Key一つに使うX座標(幅)を白鍵黒鍵それぞれ計算する...
            byte numKey_W = (byte)(GS.numKey - GS.blackKeyAmount(GS.lowestKey, GS.highestKey)); //白鍵の数
            float scaleMultiplier = windowSize_X / (numKey_W * DB.XSIZE_TEX_WKEY);                 //白鍵をそのまま全て並べた場合に対する画面の幅の割合(レールの幅にもこれを用いる)
            //Key一つに使うX座標(幅) [単位: Unit]
            GS.keyXSize_W = DB.XSIZE_TEX_WKEY * scaleMultiplier; //レールにも使う。
            GS.keyXSize_B = DB.XSIZE_TEX_BKEY * scaleMultiplier;


            //Key一つに使うY座標(高さ)をそれぞれ計算する...
            float keyYSize_W = windowSize_Y * GS.keyHeight;
            float keyHeightRatio = DB.YSIZE_TEX_BKEY / DB.YSIZE_TEX_WKEY;
            //Key一つに使うY方向のScale(高さ)
            float keyYScale_W = keyYSize_W / DB.YSIZE_TEX_WKEY;
            float keyYScale_B = keyHeightRatio * keyYSize_W / DB.YSIZE_TEX_BKEY;


            //黒鍵に加減算するXY座標。白鍵に対してXは右端に半分はみ出る位置、Yは上端にピッタリになる位置にするために使う。
            float RemoveCoord_X_B = (DB.XSIZE_TEX_BKEY * scaleMultiplier) / 2;
            float AddCoord_Y_B = keyYSize_W - (DB.YSIZE_TEX_BKEY * keyYScale_B);

            //レールを縦方向に画面いっぱいに伸ばす時に必要な係数の大きさ
            float railYScaleMultiplier = windowSize_Y / DB.XSIZE_TEX_WKEY;                         //レールは正方形の画像

            //作成したオブジェクトを整理目的で格納するためのEmptyを作成する。
            GameObject E_KEYS = new GameObject("Keys");
            GameObject E_NOTES = new GameObject("Notes");
            GameObject E_ACCESSORIES = new GameObject("Accessories");
            GameObject E_RAILS = new GameObject("Rails");
            E_KEYS.transform.parent = CAM.transform;
            E_NOTES.transform.parent = CAM.transform;
            E_ACCESSORIES.transform.parent = CAM.transform;
            E_RAILS.transform.parent = E_ACCESSORIES.transform;

            //NUM_KEYの数だけ実際にレールとキーを並べて、同時に大きさも調整する...
            byte for_WKeyCount = 0;                                                             //この値に合わせて鍵盤を設置するX座標をずらしていく。
            string code;                                                                        //並べている鍵盤のコードが入る。
            float bKey_XModify = 0f;                                                                 //黒鍵の位置調整用の変数。

            for (byte i = GS.lowestKey; i <= GS.highestKey; i++)
            {
                code = GS.getCode(i);

                if (GS.isWhiteKey((byte)(i))) //白鍵なら...
                {
                    //レールを並べる。
                    instantiateForBuild
                        (
                            RAIL,
                            E_RAILS,
                            leftX + (GS.keyXSize_W * for_WKeyCount),
                            bottomY,
                            DB.Z_RAIL,
                            DB.XSIZE_TEX_WKEY,
                            DB.XSIZE_TEX_WKEY,
                            scaleMultiplier,
                            railYScaleMultiplier,
                            "rail",
                            i
                        );

                    //白鍵を並べる
                    instantiateForBuild
                        (
                            KEY,
                            E_KEYS,
                            leftX + (GS.keyXSize_W * for_WKeyCount),
                            bottomY,
                            DB.Z_KEY_W,
                            DB.XSIZE_TEX_WKEY,
                            DB.YSIZE_TEX_WKEY,
                            scaleMultiplier,
                            keyYScale_W,
                            "key",
                            i
                        );

                    //CODELISTがCならSPLITTER(飾り)を配置する。
                    if (code == "C")
                    {
                        instantiateForBuild
                            (
                                SPLITTER,
                                E_RAILS,
                                leftX + (GS.keyXSize_W * for_WKeyCount) - ((DB.XSIZE_TEX_WKEY * GS.splitterThick)),
                                bottomY + keyYSize_W,
                                DB.Z_SPLITTER,
                                DB.XSIZE_TEX_WKEY,
                                DB.XSIZE_TEX_WKEY,
                                GS.splitterThick,                                                               //SPLITTERは正方形の画像
                                (windowSize_Y - (DB.YSIZE_TEX_WKEY * keyYScale_W)) / DB.XSIZE_TEX_WKEY,         //SPLITTERは正方形の画像
                                "splitter",
                                i
                            );   
                    }

                    for_WKeyCount++;
                }
                else
                {

                    //黒鍵を並べる

                    //コードにごとに微妙に黒鍵のX座標は異なる。
                    if (BKEY_LEFT.Contains(code))
                    {
                        bKey_XModify = -(RemoveCoord_X_B / BKEY_MODIFYFACTOR);
                    }
                    else if (BKEY_RIGHT.Contains(code))
                    {
                        bKey_XModify = RemoveCoord_X_B / BKEY_MODIFYFACTOR;
                    }
                    else
                    {
                        bKey_XModify = 0f;
                    }

                    instantiateForBuild
                        (
                            KEY,
                            E_KEYS,
                            leftX + (GS.keyXSize_W * for_WKeyCount) - RemoveCoord_X_B + bKey_XModify,
                            bottomY + AddCoord_Y_B,
                            DB.Z_KEY_B,
                            DB.XSIZE_TEX_BKEY,
                            DB.YSIZE_TEX_BKEY,
                            scaleMultiplier,
                            keyYScale_B,
                            "key",
                            i
                        );
                }
            }

            //判定ライン(飾り)を設置する。
            instantiateForBuild
                (
                    CATHCER,
                    E_ACCESSORIES,
                    leftX,
                    bottomY + keyYSize_W,
                    DB.Z_CATHCER,
                    DB.XSIZE_TEX_WKEY,
                    DB.YSIZE_TEX_CHATCHER,
                    windowSize_X / DB.XSIZE_TEX_WKEY,
                    keyYScale_W,
                    "cathcer",
                    0
                );

            //背景を設置する。
            instantiateForBuild
                (
                    BACKGROUND,
                    E_ACCESSORIES,
                    leftX,
                    bottomY,
                    DB.Z_BACKGROUND,
                    DB.XSIZE_TEX_WKEY,
                    DB.XSIZE_TEX_WKEY,
                    windowSize_X / DB.XSIZE_TEX_WKEY,
                    windowSize_Y / keyYScale_W,
                    "background",
                    0
                );

            //この後は配置された各鍵盤のスクリプトで処理が進行する。
        }



        //鍵盤生成用のInstantiate関数。
        //与えられたtranslateが画像の左下端に来るように調節してくれる。
        private void instantiateForBuild(GameObject target, GameObject parentObj, float posX, float posY, float posZ, float sizeX, float sizeY, float scaleX, float scaleY, string name, byte objIndex)
        {
            Vector2 blPos = GS.getBottomLeft(new Vector2(sizeX, sizeY), new Vector2(scaleX, scaleY));
            Vector3 pos = new Vector3(posX - blPos.x, posY - blPos.y, posZ);
            GameObject obj = Instantiate(target, pos, Quaternion.identity);
            obj.transform.parent = parentObj.transform;
            obj.transform.localScale = new Vector3(scaleX, scaleY, 1);

            //オブジェクトに対して番号が設定されていた場合、名前の後ろにその番号を付ける。
            obj.name = (objIndex == 0) ? name : (name + "_" + objIndex);
        }
    }
}