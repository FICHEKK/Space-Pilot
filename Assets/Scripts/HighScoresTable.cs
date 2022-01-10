using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoresTable : MonoBehaviour
{
    [SerializeField] private GameObject rowsContainer;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Color evenRowColor;
    [SerializeField] private Color oddRowColor;

    public void SetHighScores(List<int> highScores)
    {
        foreach (Transform childTransform in rowsContainer.transform)
        {
            Destroy(childTransform.gameObject);
        }

        for (var i = 0; i < highScores.Count; i++)
        {
            var row = Instantiate(rowPrefab, rowsContainer.transform);
            row.GetComponent<Image>().color = i % 2 == 0 ? evenRowColor : oddRowColor;
            row.GetComponentInChildren<TMP_Text>().text = highScores[i] + "m";
        }
    }
}
