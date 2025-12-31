using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string name;
    [TextArea(2,5)]
    public string line;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}
