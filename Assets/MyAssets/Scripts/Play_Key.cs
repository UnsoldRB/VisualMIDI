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
        Sprite TEX_WHITE_D;         //�����ꂽ��Ԃ̌���
        [SerializeField]
        Sprite TEX_BLACK_D;         //�����ꂽ��Ԃ̌���
        [SerializeField]
        AudioClip SE_KEY_PLACE;     //���Ղ��ݒu����n�߂��Ƃ��̉�


        private AudioSource myAudioSource = null;            //�e���՗p��AudioSource�B�S�Ă̌��ՂɕK�v�Ȃ킯�ł͂Ȃ��̂ŁA���I�ɍ쐬����B
        private byte keyNum;                                 //�̌��Ղ�MIDI�m�[�g�i���o�[�B
        private bool isWhite;                                //���̌��Ղ������ł��邩�ǂ����B
        private byte isSpawned = 0;                          //���̌��Ղ̏o���G�t�F�N�g�������������ǂ����B(0=�ҋ@��, 1=���o��, 2=����)
        private bool isPressed = false;                      //���̌��Ղ�������Ă��邩�ǂ����B






        async void Start()
        {
            //Prefab�̖��O��ύX���Ă���̂ŁA�������甒�����ǂ������ׂ�B
            keyNum = Byte.Parse(this.name.Replace("key_", ""));
            isWhite = GS.isWhiteKey(keyNum);

            //�e�N�X�`��(�I�u�W�F�N�g)���쐬����B
            KEY_RENDERER.sprite = (isWhite) ? TEX_WHITE : TEX_BLACK;
            KEY_RENDERER_D.sprite = (isWhite) ? TEX_WHITE_D : TEX_BLACK_D;

            //Play_Input�̃C�x���g�Ƀ��\�b�h��o�^�B
            IP.OnPressKey += ReceiveMsg;

            //���Ղ��o��������B
            await keySpawnDelay();
        }



        // Update is called once per frame
        void Update()
        {
            //�o�����o�̐i�s�x�����ɉ����Ď��s����֐����ς��B�B
            if (isSpawned == 1)
            {
                keySpawnEffect();
            }
            else if (isSpawned == 2)
            {
                keyPressEffect();
            }
        }



        //���Ղ̓��͂��󂯎�����Ƃ��B
        public void ReceiveMsg(byte receivedNum)
        {
            if (receivedNum != keyNum) return;  //���̌��ՂƎ󂯎�������b�Z�[�W�̌��Ղ����ꂩ�ǂ����B

            isPressed = !isPressed;
        }



        //���Ղ̏o���̒x��
        private async Task keySpawnDelay()
        {
            //���Ղ̑����̒����l���o���āA�����l����ǂꂾ���߂����ŏo������^�C�~���O��x�点��B
            // �������܂ŉ��o�Ȃ̂ŁA��̂ō\��Ȃ��B�R�[�h�����G�ɂȂ���܂����낤�B
            float keyNum = this.keyNum - GS.lowestKey + 1f;
            float midKeyNum = GS.numKey / 2f;
            float distanceByMid = Math.Abs(midKeyNum - keyNum);
            //�ꎟ�֐��𗘗p����B
            float x = distanceByMid;
            float a = DB.KEYS_SPAWN_TIME / midKeyNum;
            float b = DB.KEYS_SPAWN_TIME;

            float delay = -a * x + b;
            await Task.Delay((int)Math.Round(delay));

            //���Ղ̔z�u�����Đ�����B
            //distanceByMid��DB.MIN_KEY_AMOUNT�̔{�����AkeyNum�����Ղ̑����̒����l�ȉ��̎��̂݁A���s����B(�������Ȃ��Ɖ������W����B)
            if (distanceByMid % DB.MIN_KEY_AMOUNT == 0 && keyNum < midKeyNum)
            {
                myAudioSource = this.AddComponent<AudioSource>();           //AudioSource�쐬
                float volume = distanceByMid / midKeyNum;
                GS.playSound(SE_KEY_PLACE, myAudioSource, volume, 1f);
            }

            //�o�����o���J�n����B
            isSpawned = 1;
        }



        //���Ղ̏o�����̉��o
        private void keySpawnEffect()
        {
            //���Ղ̓����x�������Ă����B
            float addAlpha = Time.deltaTime * DB.KEY_SPAWN_SPEED;
            changeAlpha(KEY_RENDERER, addAlpha);

            //���S�ɕs�����ɂȂ����牉�o����������B
            if (KEY_RENDERER.color.a == 1f)
            {
                isSpawned = 2;

                //���̂���ɂ͌��ʉ��̍Đ����I�����Ă���̂ŁA���̃^�C�~���O��AudioSource���폜����B(�����g��Ȃ�)
                if (myAudioSource != null)
                {
                    Destroy(myAudioSource);
                }

                //�������ꂪ�����̌��Ղł���Ȃ�΁A�S���Ղ̏o���������������Ƃɂ���B
                float midKeyNoteNum = (GS.numKey + GS.lowestKey) / 2f;
                if (keyNum == Math.Truncate(midKeyNoteNum))         //������������ꍇ�͐؂�̂Ă����ő�p����B
                {
                    GS.playState = 1;

                    //���̌�̏����́A�܂�Play_Builder.cs�ɖ߂�B
                }
            }
        }



        //���Ղ̓��͂ɑ΂��鉉�o
        private void keyPressEffect()
        {
            float keyD_alpha = KEY_RENDERER_D.color.a;

            //alpha��0~1�̏ꍇ�ɂ̂ݓ��삷��B
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



        //SpriteRenderer�̓����x��ύX����B
        private void changeAlpha(SpriteRenderer sprite, float a)
        {
            float nowAlpha = sprite.color.a;
            float newAlpha = nowAlpha + a;

            //�Valpha�l���͈͊O�̏ꍇ�́A�͈͓��Ɏ��߂�B
            if (newAlpha <= 0f || 1f <= newAlpha)
            {
                newAlpha = Math.Clamp(newAlpha, 0f, 1f);
            }

            //���s���ʂ��ς��Ȃ��ꍇ�͂Ȃɂ����Ȃ��B
            if (nowAlpha == newAlpha) return;

            sprite.color = new Color(1f, 1f, 1f, newAlpha);
        }
    }
}