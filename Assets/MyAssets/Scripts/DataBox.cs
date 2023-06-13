//Addressable���g�������Ȃ������̂ł��̃N���X���f�[�^�̊i�[�ɂƂ��Ďg�p����B
//������������Ȃ񂩂���܂�悭�Ȃ����Ƃ�����Ă���̂�������Ȃ�����ЂƂ��Ōo���ς�ł�킯����Ȃ�����悭�킩���...
//���ƁA��X�g����������Ȃ�������Ă������R�ŁA�Q�Ƃ��Ă�N���X��������Ȃ��萔�����邯��...�ټ�...

public static class DataBox
{

    //����(W)�E����(B)�̉摜�f�ނ̑傫���B[�P��: Unit]
    //���I�ɉ摜�T�C�Y���g�k���Ă��摜�̈ʒu���Y���Ȃ��悤�ɁA�摜���̂ɗ]�������Ă��邽�߁A�摜�{���̃T�C�Y�Ƃ͈قȂ�B
    //1pixel = 100ppu = 1unit
    public const float XSIZE_TEX_W = 2.0f;
    public const float XSIZE_TEX_B = 1.0f;
    public const float YSIZE_TEX_W = 5.7f;
    public const float YSIZE_TEX_B = 3.6f;

    //���[���̈�ӂ̒���(���[���͐����`)[�P��: Unit]
    public const float SIZE_TEX_RAIL = XSIZE_TEX_W;

    //�Q�[���Ɏg�p�ł��錮�Ղ̍ő吔�B
    public const byte MAX_KEY_AMOUNT = 97;

    //�R�[�h�̃��X�g(raw)�B�u_�v�Ŋe�R�[�h����؂��Ă���B
    private const string CODELIST_RAW = "C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C_C#/D��_D_D#/E��_E_F_F#/G��_G_G#/A��_A_A#/B��_B_C";
    
    //�R�[�h�̃��X�g(���ۂɑ��N���X����Q�Ƃ���̂͂������B)
    public static string[] CODELIST = null;






    //�R���X�g���N�^�[�B�ϐ��̏������ȂǁB
    static DataBox()
    {
        CODELIST = CODELIST_RAW.Split("_");
    }



    //�g�p���錮�Ր��ɂ�����Œቹ��Ԃ��B
    public static byte getLowestCode(byte keyAmount)
    {
        return (byte)(MAX_KEY_AMOUNT - keyAmount + 1);
    }



    //CODELIST���̎w�肳�ꂽindex�ɑΉ����Ă��錮�Ղ������ł��邩�ǂ�����Ԃ��B
    public static bool isWhiteKey(byte index)
    {
        return !CODELIST[index - 1].Contains("/");
    }



    //�g�p���錮�Ղ̑����̓��A�������������邩��Ԃ��B
    public static byte blackKeyAmount(byte keyAmount)
    {
        //�g�p���錮�Ղ̐��ɉ����čŒቹ�𒲐߂���B
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