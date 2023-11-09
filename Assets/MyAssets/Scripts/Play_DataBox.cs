//Addressable���g�������Ȃ������̂ł��̃N���X���f�[�^�̊i�[�ɂƂ��Ďg�p����B
//������������Ȃ񂩂���܂�悭�Ȃ����Ƃ�����Ă���̂�������Ȃ�����ЂƂ��Ōo���ς�ł�킯����Ȃ�����悭�킩���...
//���ƁA��X�g����������Ȃ�������Ă������R�ŁA�Q�Ƃ��Ă�N���X��������Ȃ��萔������...

namespace RiceBall.Datas
{
    public static class Play_DataBox
    {

        //�摜�f�ނ̑傫���B[�P��: Unit]
        //������XSIZE�́A���̏���Ɏg���摜�f�ނƓ������B
        //1pixel = 100ppu = 1unit
        public const float XSIZE_TEX_WKEY = 2.0f;
        public const float XSIZE_TEX_BKEY = 1.0f;
        public const float YSIZE_TEX_WKEY = 5.7f;
        public const float YSIZE_TEX_BKEY = 3.6f;
        public const float YSIZE_TEX_CHATCHER = 0.2f;

        //�Q�[���Ɏg�p�ł��錮�Ղ̍ő�E�ŏ����B
        public const byte MAX_KEY_AMOUNT = 97;
        public const byte MIN_KEY_AMOUNT = 12;              //�h����V�܂ł̊Ԃɂ��錮�Ղ̐��Ɠ������B

        //�Q�[���Ɏg�p�ł���Œቹ��MIDI�m�[�g�i���o�[�B
        public const byte LOWEST_NOTE_NUM = 12;

        //�u�h���~�t�@�\���V�v�̃R�[�h�B
        private const string CODELIST_RAW = "C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B";

        //�R�[�h�̃��X�g(���ۂɑ��N���X����֐���ʂ��ĎQ�Ƃ���̂͂������B)
        public static string[] CODELIST { get; private set; } = new string[MAX_KEY_AMOUNT];

        //�e�f�ނ�Z���W(���s��)
        public const sbyte Z_BACKGROUND = 0;
        public const sbyte Z_RAIL = -1;
        public const sbyte Z_KEY_W = -2;
        public const sbyte Z_KEY_B = -3;
        public const sbyte Z_CATHCER = -4;
        public const sbyte Z_SPLITTER = -5;
        public const sbyte Z_NOTE = -6;

        public const float KEYS_SPAWN_TIME = 450f;          //�S���Ղ̏o�������ɗv���鎞��
        public const float KEY_SPAWN_SPEED = 2f;            //���Ո�̏o���ɗv���鎞��
        public const float KEY_EFFECT_SPEED = 12f;          //���Ղ������ꂽ�Ƃ��̉��o�̊������x




        //�R���X�g���N�^�[�B�ϐ��̏������ȂǁB
        static Play_DataBox()
        {
            //CODELIST��g�ݗ��Ă�B
            string[] baseCode = CODELIST_RAW.Split("_");
            for (byte i = 0; i < MAX_KEY_AMOUNT; i++)
            {
                CODELIST[i] = baseCode[i % 12];
            }
        }
    }
}