using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;

    public Sprite[] portraits;
    public bool[] isPlayerSpeaking;

    public DialogueChoice[] choices; // CÁC LỰA CHỌN khi kết thúc hội thoại
}

