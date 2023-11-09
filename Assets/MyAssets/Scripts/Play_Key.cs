using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using DB = RiceBall.Datas.Play_DataBox;
using GS = RiceBall.Datas.Play_GameState;
using IP = RiceBall.Input.Play_Input;

namespace RiceBall.Mains
{
    public class Play_Key : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer KEY_RENDERER;
        [SerializeField]
        SpriteRenderer KEY_RENDERER_D;
        [SerializeField]
        Sprite TEX_WHITE;
        [SerializeField]
        Sprite TEX_BLACK;
        [SerializeField]
        Sprite TEX_WHITE_D;         //押された状態の鍵盤
        [SerializeField]
        Sprite TEX_BLACK_D;         //押された状態の鍵盤
        [SerializeField]
        AudioClip SE_KEY_PLACE;     //鍵盤が設置され始めたときの音


        private AudioSource myAudioSource = null;            //各鍵盤用のAudioSource。全ての鍵盤に必要なわけではないので、動的に作成する。
        private byte keyNum;                                 //個の鍵盤のMIDIノートナンバー。
        private bool isWhite;                                //この鍵盤が白鍵であるかどうか。
        private byte isSpawned = 0;                          //この鍵盤の出現エフェクトが完了したかどうか。(0=待機中, 1=演出中, 2=完了)
        private bool isPressed = false;                      //この鍵盤が押されているかどうか。






        async void Start()
        {
            //Prefabの名前を変更してあるので、そこから白鍵かどうか調べる。
            keyNum = Byte.Parse(this.name.Replace("key_", ""));
            isWhite = GS.isWhiteKey(keyNum);

            //テクスチャ(オブジェクト)を作成する。
            KEY_RENDERER.sprite = (isWhite) ? TEX_WHITE : TEX_BLACK;
            KEY_RENDERER_D.sprite = (isWhite) ? TEX_WHITE_D : TEX_BLACK_D;

            //Play_Inputのイベントにメソッドを登録。
            IP.OnPressKey += ReceiveMsg;

            //鍵盤を出現させる。
            await keySpawnDelay();
        }



        // Update is called once per frame
        void Update()
        {
            //出現演出の進行度合いに応じて実行する関数が変わる。。
            if (isSpawned == 1)
            {
                keySpawnEffect();
            }
            else if (isSpawned == 2)
            {
                keyPressEffect();
            }
        }



        //鍵盤の入力を受け取ったとき。
        public void ReceiveMsg(byte receivedNum)
        {
            if (receivedNum != keyNum) return;  //この鍵盤と受け取ったメッセージの鍵盤が同一かどうか。

            isPressed = !isPressed;
        }



        //鍵盤の出現の遅延
        private async Task keySpawnDelay()
        {
            //鍵盤の総数の中央値を出して、中央値からどれだけ近いかで出現するタイミングを遅らせる。
            // ※あくまで演出なので、大体で構わない。コードが複雑になるよりましだろう。
            float keyNum = this.keyNum - GS.lowestKey + 1f;
            float midKeyNum = GS.numKey / 2f;
            float distanceByMid = Math.Abs(midKeyNum - keyNum);
            //一次関数を利用する。
            float x = distanceByMid;
            float a = DB.KEYS_SPAWN_TIME / midKeyNum;
            float b = DB.KEYS_SPAWN_TIME;

            float delay = -a * x + b;
            await Task.Delay((int)Math.Round(delay));

            //鍵盤の配置音を再生する。
            //distanceByMidがDB.MIN_KEY_AMOUNTの倍数かつ、keyNumが鍵盤の総数の中央値以下の時のみ、実行する。(そうしないと音が密集する。)
            if (distanceByMid % DB.MIN_KEY_AMOUNT == 0 && keyNum < midKeyNum)
            {
                myAudioSource = this.AddComponent<AudioSource>();           //AudioSource作成
                float volume = distanceByMid / midKeyNum;
                GS.playSound(SE_KEY_PLACE, myAudioSource, volume, 1f);
            }

            //出現演出を開始する。
            isSpawned = 1;
        }



        //鍵盤の出現時の演出
        private void keySpawnEffect()
        {
            //鍵盤の透明度を下げていく。
            float addAlpha = Time.deltaTime * DB.KEY_SPAWN_SPEED;
            changeAlpha(KEY_RENDERER, addAlpha);

            //完全に不透明になったら演出を完了する。
            if (KEY_RENDERER.color.a == 1f)
            {
                isSpawned = 2;

                //このころには効果音の再生も終了しているので、このタイミングでAudioSourceを削除する。(もう使わない)
                if (myAudioSource != null)
                {
                    Destroy(myAudioSource);
                }

                //もしこれが中央の鍵盤であるならば、全鍵盤の出現が完了したことにする。
                float midKeyNoteNum = (GS.numKey + GS.lowestKey) / 2f;
                if (keyNum == Math.Truncate(midKeyNoteNum))         //もし奇数だった場合は切り捨てた数で代用する。
                {
                    GS.playState = 1;

                    //この後の処理は、またPlay_Builder.csに戻る。
                }
            }
        }



        //鍵盤の入力に対する演出
        private void keyPressEffect()
        {
            float keyD_alpha = KEY_RENDERER_D.color.a;

            //alphaが0~1の場合にのみ動作する。
            if (0f <= keyD_alpha && keyD_alpha <= 1f)
            {
                float addAlpha = Time.deltaTime * DB.KEY_EFFECT_SPEED;

                if (isPressed)
                {
                    changeAlpha(KEY_RENDERER_D, addAlpha);
                }
                else
                {
                    changeAlpha(KEY_RENDERER_D, -(addAlpha));
                }
            }
        }



        //SpriteRendererの透明度を変更する。
        private void changeAlpha(SpriteRenderer sprite, float a)
        {
            float nowAlpha = sprite.color.a;
            float newAlpha = nowAlpha + a;

            //新alpha値が範囲外の場合は、範囲内に収める。
            if (newAlpha <= 0f || 1f <= newAlpha)
            {
                newAlpha = Math.Clamp(newAlpha, 0f, 1f);
            }

            //実行結果が変わらない場合はなにもしない。
            if (nowAlpha == newAlpha) return;

            sprite.color = new Color(1f, 1f, 1f, newAlpha);
        }
    }
}