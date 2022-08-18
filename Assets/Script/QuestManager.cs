using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{

    public List<QuestSO> allQuest;

    public GameObject panelQuest, descriptionPanel, parent, quest;

    public static QuestManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allQuest.Count; i++) // Au départ du jeu je recupere toutes les quetes et j'attribue un id i à chaque quête
        {
            allQuest[i].id = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            panelQuest.SetActive(true);

            if (parent.transform.childCount > 0)
            {
                foreach (Transform child in parent.transform) // Si j'ai des enfants, je vais les detruires un à un, empeche de recloner les quetes deja cloner
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < allQuest.Count; i++)
            {
                if(i <= allQuest.Count - 1)
                {
                    if(allQuest[i].statut == QuestSO.Statut.accepter)
                    {
                        GameObject slot = Instantiate(quest, parent.transform.position, transform.rotation);
                        slot.transform.SetParent(parent.transform);

                        TextMeshProUGUI title = slot.transform.Find("TitleQuest").GetComponent<TextMeshProUGUI>();
                        title.text = allQuest[i].title;

                        TextMeshProUGUI statut = slot.transform.Find("Statut").GetComponent<TextMeshProUGUI>();
                        statut.text = "" + allQuest[i].statut;
                    }
                }
            }
        }
    }
}
