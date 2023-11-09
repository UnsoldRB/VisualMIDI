//MIDIデバイスとのやり取りを仲介するインターフェイス的なクラス。

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
        private static IntPtr midi_DevPtr;                          //midiInOpen()を実行したりした時にデバイスのポインターが入る。

        //鍵盤が押されたときに発生させるイベントの定義
        public delegate void OnPressKeyHandler(byte num);
        public static event OnPressKeyHandler OnPressKey;


        //この構造体に、指定した識別子のデバイス情報が入る。
        [StructLayout(LayoutKind.Sequential)]
        public struct midi_Info
        {
            public ushort wMid;                                     //ドライバの製造元識別子。使わない。
            public ushort wPid;                                     //デバイスの製造元識別子。使わない。
            public uint vDriverVersion;                             //ドライバのバージョン。使わない。
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string deviceName;                               //ドライバの名称。
            public uint dwSupport;                                  //なんなんだろうこれ...0じゃないといけないらしい。
        }



        [DllImport("winmm.dll")]
        extern static uint midiInGetNumDevs();                                                      //有効なMIDIデバイスの数を返す。
        [DllImport("winmm.dll")]
        extern static uint midiInGetDevCaps(uint uDevID, out midi_Info pmic, int cbmic);            //有効なMIDIデバイスの情報を返す(？)
        [DllImport("winmm.dll")]
        private static extern int midiInOpen(out IntPtr lphMidiIn, int uDeviceID, MidiInProc dwCallback, IntPtr dwInstance, int dwFlags);    //デバイスに対して入力の受付準備を行う。
        [DllImport("winmm.dll")]
        private static extern int midiInStart(IntPtr hMidiIn);                                      //デバイスからの入力を受け付ける。
        [DllImport("winmm.dll")]
        private static extern int midiInStop(IntPtr hMidiIn);                                       //デバイスからの入力受付を停止する。
        [DllImport("winmm.dll")]
        private static extern int midiInClose(IntPtr hMidiIn);                                      //デバイスを閉じる(プログラムから切り離す。)
        [DllImport("winmm.dll")]
        private static extern int midiInMessage(IntPtr hMidiIn, int uMsg, IntPtr dw1, IntPtr dw2);  //MIDIメッセージを受信してくれる。
        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern int midiInGetErrorTextA(int errCode, StringBuilder errMsg, int size);         //エラーの内容を返してくれる。デバッグ用


        public delegate void MidiInProc(IntPtr hMidiIn, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);    //MIDI入力のコールバック関数(種類定義)






        //MIDI入力のコールバック関数
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
            else if (uMsg == MIDI_DATA)                             //MIDI入力が受け付けられているときにMIDI入力を受けたとき。
            {
                int status = dwParam1.ToInt32() & 0xFF;
                byte noteNum = (byte)((dwParam1.ToInt32() >> 8) & 0xFF);
                //int data2 = (dwParam1.ToInt32() >> 16) & 0xFF;    //今はまだ使ってないデータ。

                OnPressKey?.Invoke(noteNum);                        //イベントを発火させる。
            }
        }



        //接続済みのMIDIデバイス一覧を取得する。
        public static uint getDevices()
        {
            uint dev_amount = midiInGetNumDevs();
            midi_Info info = new midi_Info();
            for (int i = 0; i < dev_amount; i++)
            {
                midiInGetDevCaps((uint)i, out info, Marshal.SizeOf(typeof(midi_Info)));         //infoにデバイスの情報を格納する。
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



        //ゲーム終了時にも呼ばれる。
        public static void disconnectDevice()
        {
            int result = midiInClose(midi_DevPtr);
            outputError(result);
        }



        //エラーコードをエラーの内容に変換する関数。
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