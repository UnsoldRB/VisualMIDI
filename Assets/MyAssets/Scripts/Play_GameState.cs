//�Q�[���v���C���Ɏg�p�����f�[�^�̊i�[�ɁBDataBox.cs�ɑ΂��āA������͑��̏ꍇ�Q�[�����Ƀf�[�^���A�b�v�f�[�g�����B
//�܂��A�Q�[�����ŕ��L���g�p�����֐���������ɓ����Ă���B

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DB = RiceBall.Datas.Play_DataBox;

namespace RiceBall.Datas
{
    public static class Play_GameState
    {
        public static byte numKey { get; set; } = 0;                    //���Ղ̐��B
        public static byte highestKey { get; set; } = 0;                //�ō�����MIDI�m�[�g�i���o�[
        public static byte lowestKey { get; set; } = 0;                 //�Œቹ��MIDI�m�[�g�i���o�[

        public static float keyXSize_W { get; set; } = 0.0f;            //����1�Ɏg����X���W(��)
        public static float keyXSize_B { get; set; } = 0.0f;            //����1�Ɏg����X���W(��)
        public const float keyHeight = 0.2f;                            //��ʂ̍����ɑ΂���Key�̍���
        public const float splitterThick = 0.035f;                      //�����̕��ɑ΂���SPLITTER�̑���

        public static byte playState { get; set; } = 0;                 //�Q�[���̐i�s�x�����B(0=���Քz�u�����O, 1=���Քz�u����, 2=�v���C��, 3=�|�[�Y��, 4=���ʔ��\)







        //�R���X�g���N�^�[�B�ϐ��̏������ȂǁB
        static Play_GameState()
        {
            
        }



        //�Ώۂ�Hierarchy���̃p�X��Ԃ��B
        public static string getPath(GameObject target)
        {
            List<GameObject> objects = new List<GameObject> { target };
            for (int i = 0; objects[i].transform.parent != null; i++)               //�e�����Ȃ��Ȃ�܂ŃI�u�W�F�N�g�̐e��ϐ��Ɋi�[����B
            {
                objects.Add(objects[i].transform.parent.gameObject);
            }
            objects.Reverse();                                                      //�e���珇�ԂɃ��[�v���������̂Ŕ��]������B
            StringBuilder path = new StringBuilder("", 60);
            foreach (GameObject obj in objects)
            {
                path.Append(obj.name + "/");
            }
            return path.ToString().Substring(0, path.Length - 1);                   //�Ō��"/"����菜����return
        }



        //MIDI�m�[�g�i���o�[���特�K��Ԃ��B
        public static string getCode(byte noteNum)
        {
            return DB.CODELIST[(byte)noteNum - DB.LOWEST_NOTE_NUM];
        }



        //CODELIST���̎w�肳�ꂽindex�ɑΉ����Ă��錮�Ղ������ł��邩�ǂ�����Ԃ��B
        public static bool isWhiteKey(byte index)
        {
            return !getCode(index).Contains("/");
        }



        //�g�p���錮�Ղ̑����̓��A�������������邩��Ԃ��B
        public static byte blackKeyAmount(byte lowestKey, byte highestKey)
        {
            //�g�p���錮�Ղ̐��ɉ����čŒቹ�𒲐߂���B
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



        //�w�肵�������̃I�u�W�F�N�g�ɂ����鍶���[�̃��[�J�����W��Ԃ��B
        public static Vector2 getBottomLeft(Vector2 size, Vector2 scale)
        {
            float x = size.x * scale.x / 2;
            float y = size.y * scale.y / 2;
            return new Vector2(-x, -y);
        }



        //���ʉ����Đ�����B(MainThreadOnly)
        public static void playSound(AudioClip sound, AudioSource source, float volume, float pitch)
        {
            if (source == null) return;         //AudioSource�����݂��Ȃ��ꍇ�͉������Ȃ��B

            source.volume = volume;
            source.pitch = pitch;
            source.PlayOneShot(sound);
        }
    }
}