using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores highScores = new HighScores();

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    /// 디스크에서 점수 로드
    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/DungeonGunnerHighScores.dat"))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

            highScores = (HighScores)bf.Deserialize(file);

            file.Close();
        }
    }

    /// 모든 점수 삭제
    private void ClearScoreList()
    {
        highScores.scoreList.Clear();
    }

    /// 점수를 고등 점수 리스트에 추가
    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        // 저장할 최대 점수 수 유지
        if (highScores.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            highScores.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    /// 점수를 디스크에 저장
    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

        bf.Serialize(file, highScores);

        file.Close();
    }

    /// 고등 점수 가져오기
    public HighScores GetHighScores()
    {
        return highScores;
    }

    /// 플레이어 점수와 다른 고등 점수와 비교하여 순위를 반환 (만일 점수가 고등 점수 리스트에 없으면 0을 반환)
    public int GetRank(long playerScore)
    {
        // 현재 리스트에 점수가 없으면 이 점수는 1등
        if (highScores.scoreList.Count == 0) return 1;

        int index = 0;

        // 이 점수의 순위를 찾기 위해 리스트를 순회
        for (int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }
        }

        // 고등 점수 리스트의 최대 저장 수보다 적을 때
        if (highScores.scoreList.Count < Settings.numberOfHighScoresToSave)
            return (index + 1);

        return 0;
    }
}