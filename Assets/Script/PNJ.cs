using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PNJ : MonoBehaviour
{
    [SerializeField]
    string[] sentences; // Dialogue de base du pnj
    [SerializeField]
    string characterName;
    int index;
    bool isOndial, canDial;

    HUDManager manager => HUDManager.instance;

    public QuestSO quest; // On pourra ajouter une quete sur un pnj

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canDial)
        {
            if(quest != null && quest.statut == QuestSO.Statut.none) // Si nous n'avons pas accepter de quest
            {
                StartDialogue(quest.sentences); // On affiche le dialogue de la quete demandée par le pnj
            }
            else if(quest != null && quest.statut == QuestSO.Statut.accepter && quest.actualAmount < quest.amountToFind)
            {
                StartDialogue(quest.inProgressSentence);
            }
            else if(quest != null && quest.statut == QuestSO.Statut.accepter && quest.actualAmount == quest.amountToFind)
            {
                StartDialogue(quest.completeSentence);
                quest.statut = QuestSO.Statut.complete;
            }
            else if(quest != null && quest.statut == QuestSO.Statut.complete)
            {
                StartDialogue(quest.afterQuestSentence);
            }
            else if(quest == null)
            {
                StartDialogue(sentences);
            }
        }
    }

    public void StartDialogue(string[] sentence)
    {
        manager.dialogueHolder.SetActive(true);
        PlayerController.instance.canMove = false;
        PlayerController.instance.canAttack = false;
        isOndial = true;
        TypingText(sentence);
        manager.continueButton.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove listeners permet de faire un reset de tous les potentiels script du bouton quand on appuie dessus
        manager.continueButton.GetComponent<Button>().onClick.AddListener(delegate { NextLine(sentence); });
    }

    void TypingText(string[] sentence)
    {
        manager.nameDisplay.text = ""; // A chaque fois qu'on appellera cette méthodes nous reinitialiserons l'intérieur de nos variables, de nos text.
        manager.textDisplay.text = "";

        manager.nameDisplay.text = characterName;
        manager.textDisplay.text = sentence[index];

        if (manager.textDisplay.text == sentence[index])
        {
            manager.continueButton.SetActive(true);
        }
    }

    public void NextLine(string[] sentence)
    {
        manager.continueButton.SetActive(false);

        if(isOndial && index < sentence.Length - 1) // Si nous sommes en dialogue et que notre sentence est inférieure à length -1
        {
            index++;
            manager.textDisplay.text = "";
            TypingText(sentence);
        }
        else if(isOndial && index == sentence.Length - 1)
        {
            isOndial = false;
            index = 0;
            manager.textDisplay.text = "";
            manager.nameDisplay.text = "";
            manager.dialogueHolder.SetActive(false);

            PlayerController.instance.canMove = true;
            PlayerController.instance.canAttack = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) // Quand le joueur entrera dans la zone de collission le dialogue s'affichera
    {
        if(collision.tag == "Player")
        {
            canDial = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canDial = false;
        }
    }
}
