using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RiceBall.Mains
{
    public class Play_MIDIReader : MonoBehaviour
    {
        [SerializeField]
        DefaultAsset TESTMIDI;
        // Start is called before the first frame update
        void Start()
        {
            File.ReadAllBytes(TESTMIDI);
        }



        // Update is called once per frame
        void Update()
        {

        }
    }
}