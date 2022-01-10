using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatisticsTracker : MonoBehaviour
{
    [SerializeField] private string statisticsFileName;
    [SerializeField] private Spaceship spaceship;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private HighScoresTable highScoresTable;
    private Statistics _statistics;

    public int HighScore => _statistics.highScore;
    public List<int> AllScores => _statistics.allScores;

    private void Awake()
    {
        LoadStatistics();

        spaceship.OnDeath += () =>
        {
            var score = (int) spaceship.transform.position.z;
            highScoreText.enabled = score > _statistics.highScore;

            _statistics.highScore = Mathf.Max(score, _statistics.highScore);
            _statistics.allScores.Add(score);

            highScoresTable.SetHighScores(_statistics.allScores.OrderByDescending(s => s).ToList());
        };
    }

    private void LoadStatistics()
    {
        var filePath = Path.Combine(Application.persistentDataPath, statisticsFileName);

        if (!File.Exists(filePath))
        {
            _statistics = new Statistics();
            return;
        }
        
        var statisticsJson = File.ReadAllText(filePath);
        _statistics = JsonUtility.FromJson<Statistics>(statisticsJson);
    }

    private void OnDestroy() => SaveStatistics();

    private void SaveStatistics()
    {
        var statisticsJson = JsonUtility.ToJson(_statistics);
        var filePath = Path.Combine(Application.persistentDataPath, statisticsFileName);
        File.WriteAllText(filePath, statisticsJson);
    }

    [Serializable]
    private class Statistics
    {
        public int highScore;
        public List<int> allScores = new List<int>();
    }
}
