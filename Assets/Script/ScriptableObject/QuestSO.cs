using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Quest", menuName = "ScriptableObject/Quest", order = 0)]
public class QuestSO : ScriptableObject
{
    public int id;
    public string title;
    public string description;
    public string[] sentences, inProgressSentence, completeSentence, afterQuestSentence;
    public string objectToFind;
    public int actualAmount, amountToFind;
    public int goldToGive;

    [System.Serializable]
    public enum Statut // Enum c'est un menu déroulant et on pourra accepter ce qu'il y a dedans
    {
        none,
        accepter,
        complete
    }

    public Statut statut;
}
