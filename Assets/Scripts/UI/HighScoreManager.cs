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

    /// ��ũ���� ���� �ε�
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

    /// ��� ���� ����
    private void ClearScoreList()
    {
        highScores.scoreList.Clear();
    }

    /// ������ ��� ���� ����Ʈ�� �߰�
    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        // ������ �ִ� ���� �� ����
        if (highScores.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            highScores.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    /// ������ ��ũ�� ����
    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

        bf.Serialize(file, highScores);

        file.Close();
    }

    /// ��� ���� ��������
    public HighScores GetHighScores()
    {
        return highScores;
    }

    /// �÷��̾� ������ �ٸ� ��� ������ ���Ͽ� ������ ��ȯ (���� ������ ��� ���� ����Ʈ�� ������ 0�� ��ȯ)
    public int GetRank(long playerScore)
    {
        // ���� ����Ʈ�� ������ ������ �� ������ 1��
        if (highScores.scoreList.Count == 0) return 1;

        int index = 0;

        // �� ������ ������ ã�� ���� ����Ʈ�� ��ȸ
        for (int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }
        }

        // ��� ���� ����Ʈ�� �ִ� ���� ������ ���� ��
        if (highScores.scoreList.Count < Settings.numberOfHighScoresToSave)
            return (index + 1);

        return 0;
    }
}