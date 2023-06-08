using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerBuilder : MonoBehaviour
{
    [SerializeField]
    Camera CAM;
    [SerializeField]
    GameObject RAIL;
    [SerializeField]
    GameObject NOTE;
    [SerializeField]
    GameObject KEY_WHITE;
    [SerializeField]
    GameObject KEY_BLACK;

    //基本的な画像素材の大きさ。
    //1pixel = 100ppu = 1unit
    const int SIZE_TEX = 2;
    //keyの数。
    const int NUM_KEY = 88; 
    //各素材のZ座標(奥行き)
    int Z_RAIL = 0;
    int Z_KEY = -1;
    int Z_CATHCER = -2;
    int Z_NOTE = -3;


    string[] doremi;
    //白鍵1つに使えるX座標(幅)
    float keySize;





    void Start()
    {
        doremi = splitDoremiAsync();
        buildRails();
    }



    void Update()
    {
        
    }


    private async Task<string[]> splitDoremiAsync() {
        const string filePath = "doremi";

        AsyncOperationHandle<TextAsset> loadHandle = Addressables.LoadAssetAsync<TextAsset>(filePath);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset textAsset = loadHandle.Result;
            string textContent = textAsset.text;

            // テキストファイルの内容を使用する処理をここに記述

            Addressables.Release(loadHandle); // ロードハンドルの解放
            return 
        }
        else
        {
            return "Failed to load text file: ";
        }
    }

    //画面にレールを並べる関数
    private void buildRails() {
        //各端の座標を取得して、ゲーム内に適応する画像のサイズを計算する。
        Vector2 startVec = CAM.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector2 endVec = CAM.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float leftX = startVec.x;
        float bottomY = startVec.y;
        float rightX = endVec.x;
        float upperY = endVec.y;


        //Key一つに使うX座標(幅)を計算する
        keySize = (rightX - leftX) / NUM_KEY;

        //各素材に乗ずる、幅の係数。
        float scaleMultiplier = keySize / SIZE_TEX;

        //NUM_KEYの数だけ実際にレールとキーを並べて、同時に大きさも調整する。
        GameObject temp_Obj;
        Vector3 temp_Scale;
        float railYScaleMultiplier = (upperY - bottomY) / SIZE_TEX; //レールを縦方向に画面いっぱいに伸ばす時に必要な係数の大きさ

        for (int i = 0; i < NUM_KEY; i++) {
            //レールを並べて大きさを調整する
            temp_Obj = Instantiate(RAIL, new Vector3(leftX + (keySize * i), bottomY, Z_RAIL), Quaternion.identity);
            temp_Scale = temp_Obj.transform.localScale;
            temp_Obj.transform.localScale = new Vector3(temp_Scale.x * scaleMultiplier, temp_Scale.y * railYScaleMultiplier, 1);
        
            //鍵盤を並べる
            Instantiate(KEY_WHITE, new Vector3(leftX + (keySize * i), bottomY, Z_KEY), Quaternion.identity);
        }
    }
}
