//MIDI�f�o�C�X�Ƃ̂����𒇉��C���^�[�t�F�C�X�I�ȃN���X�B

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

using GS = RiceBall.Datas.Play_GameState;

namespace RiceBall.Input
{
    public static class Play_Input
    {
        private const int MIDI_OPEN = 0x3C1;
        private const int MIDI_CLOSE = 0x3C2;
        private const int MIDI_DATA = 0x3C3;
        private const int CALLBACK_FUNCTION = 0x30000;
        private static IntPtr midi_DevPtr;                          //midiInOpen()�����s�����肵�����Ƀf�o�C�X�̃|�C���^�[������B

        //���Ղ������ꂽ�Ƃ��ɔ���������C�x���g�̒�`
        public delegate void OnPressKeyHandler(byte num);
        public static event OnPressKeyHandler OnPressKey;


        //���̍\���̂ɁA�w�肵�����ʎq�̃f�o�C�X��񂪓���B
        [StructLayout(LayoutKind.Sequential)]
        public struct midi_Info
        {
            public ushort wMid;                                     //�h���C�o�̐��������ʎq�B�g��Ȃ��B
            public ushort wPid;                                     //�f�o�C�X�̐��������ʎq�B�g��Ȃ��B
            public uint vDriverVersion;                             //�h���C�o�̃o�[�W�����B�g��Ȃ��B
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string deviceName;                               //�h���C�o�̖��́B
            public uint dwSupport;                                  //�Ȃ�Ȃ񂾂낤����...0����Ȃ��Ƃ����Ȃ��炵���B
        }



        [DllImport("winmm.dll")]
        extern static uint midiInGetNumDevs();                                                      //�L����MIDI�f�o�C�X�̐���Ԃ��B
        [DllImport("winmm.dll")]
        extern static uint midiInGetDevCaps(uint uDevID, out midi_Info pmic, int cbmic);            //�L����MIDI�f�o�C�X�̏���Ԃ�(�H)
        [DllImport("winmm.dll")]
        private static extern int midiInOpen(out IntPtr lphMidiIn, int uDeviceID, MidiInProc dwCallback, IntPtr dwInstance, int dwFlags);    //�f�o�C�X�ɑ΂��ē��͂̎�t�������s���B
        [DllImport("winmm.dll")]
        private static extern int midiInStart(IntPtr hMidiIn);                                      //�f�o�C�X����̓��͂��󂯕t����B
        [DllImport("winmm.dll")]
        private static extern int midiInStop(IntPtr hMidiIn);                                       //�f�o�C�X����̓��͎�t���~����B
        [DllImport("winmm.dll")]
        private static extern int midiInClose(IntPtr hMidiIn);                                      //�f�o�C�X�����(�v���O��������؂藣���B)
        [DllImport("winmm.dll")]
        private static extern int midiInMessage(IntPtr hMidiIn, int uMsg, IntPtr dw1, IntPtr dw2);  //MIDI���b�Z�[�W����M���Ă����B
        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern int midiInGetErrorTextA(int errCode, StringBuilder errMsg, int size);         //�G���[�̓��e��Ԃ��Ă����B�f�o�b�O�p


        public delegate void MidiInProc(IntPtr hMidiIn, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);    //MIDI���͂̃R�[���o�b�N�֐�(��ޒ�`)






        //MIDI���͂̃R�[���o�b�N�֐�
        private static void MIDI_Callback(IntPtr hMidiIn, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
        {
            if (uMsg == MIDI_OPEN)
            {
                Debug.Log("MIDI input opened.");
            }
            else if (uMsg == MIDI_CLOSE)
            {
                Debug.Log("MIDI input closed.");
            }
            else if (uMsg == MIDI_DATA)                             //MIDI���͂��󂯕t�����Ă���Ƃ���MIDI���͂��󂯂��Ƃ��B
            {
                int status = dwParam1.ToInt32() & 0xFF;
                byte noteNum = (byte)((dwParam1.ToInt32() >> 8) & 0xFF);
                //int data2 = (dwParam1.ToInt32() >> 16) & 0xFF;    //���͂܂��g���ĂȂ��f�[�^�B

                OnPressKey?.Invoke(noteNum);                        //�C�x���g�𔭉΂�����B
            }
        }



        //�ڑ��ς݂�MIDI�f�o�C�X�ꗗ���擾����B
        public static uint getDevices()
        {
            uint dev_amount = midiInGetNumDevs();
            midi_Info info = new midi_Info();
            for (int i = 0; i < dev_amount; i++)
            {
                midiInGetDevCaps((uint)i, out info, Marshal.SizeOf(typeof(midi_Info)));         //info�Ƀf�o�C�X�̏����i�[����B
                Debug.Log(i + "/" + info.deviceName);
                connectDevice(i);
            }
            return dev_amount;
        }



        public static void connectDevice(int devID)
        {
            int result = midiInOpen(out midi_DevPtr, devID, MIDI_Callback, IntPtr.Zero, CALLBACK_FUNCTION);
            outputError(result);
            result = midiInStart(midi_DevPtr);
            outputError(result);
        }



        //�Q�[���I�����ɂ��Ă΂��B
        public static void disconnectDevice()
        {
            int result = midiInClose(midi_DevPtr);
            outputError(result);
        }



        //�G���[�R�[�h���G���[�̓��e�ɕϊ�����֐��B
        private static string outputError(int errorCode)
        {
            const int MAXERRORLENGTH = 2560;
            StringBuilder result = new StringBuilder(MAXERRORLENGTH);
            midiInGetErrorTextA(errorCode, result, MAXERRORLENGTH);
            Debug.Log(result.ToString().Trim());
            return result.ToString().Trim();
        }
    }
}