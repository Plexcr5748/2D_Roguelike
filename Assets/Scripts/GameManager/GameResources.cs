using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("������ RoomNodeTypeListSO�� ä��ϴ�.")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER SELECTION
    [Space(10)]
    [Header("PLAYER SELECTION")]
    #endregion
    #region Tooltip
    [Tooltip("�÷��̾� ���� ������")]
    #endregion
    public GameObject playerSelectionPrefab;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    #region Tooltip
    [Tooltip("�÷��̾� ���� ���� ����Ʈ - �÷��̾� ���� ������ ��ũ���ͺ� ��ü�� ä��ϴ�.")]
    #endregion
    public List<PlayerDetailsSO> playerDetailsList;
    #region Tooltip
    [Tooltip("���� �÷��̾� ��ũ���ͺ� ��ü - �� ���� ���� �÷��̾ �����ϴ� �� ���˴ϴ�.")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    #region Tooltip
    [Tooltip("���� ������ �ͼ� �׷����� ä��ϴ�.")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    #region Tooltip
    [Tooltip("���� �޴� ���� ��ũ���ͺ� ��ü")]
    #endregion
    public MusicTrackSO mainMenuMusic;
    #region Tooltip
    [Tooltip("���� on full ������")]
    #endregion
    public AudioMixerSnapshot musicOnFullSnapshot;
    #region Tooltip
    [Tooltip("���� low ������")]
    #endregion
    public AudioMixerSnapshot musicLowSnapshot;
    #region Tooltip
    [Tooltip("���� off ������")]
    #endregion
    public AudioMixerSnapshot musicOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    #region Tooltip
    [Tooltip("���� ������ �ͼ� �׷����� ä��ϴ�.")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    #region Tooltip
    [Tooltip("�� ����/�ݱ� ȿ����")]
    #endregion
    public SoundEffectSO doorOpenCloseSoundEffect;
    #region Tooltip
    [Tooltip("���̺� ������ ȿ����")]
    #endregion
    public SoundEffectSO tableFlip;
    #region Tooltip
    [Tooltip("���� ���� ȿ����")]
    #endregion
    public SoundEffectSO chestOpen;
    #region Tooltip
    [Tooltip("ü�� ȸ�� ������ ȿ����")]
    #endregion
    public SoundEffectSO healthPickup;
    #region Tooltip
    [Tooltip("���� ȹ�� ������ ȿ����")]
    #endregion
    public SoundEffectSO weaponPickup;
    #region Tooltip
    [Tooltip("ź�� ȹ�� ������ ȿ����")]
    #endregion
    public SoundEffectSO ammoPickup;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("dimmedMaterial")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default ���")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Variable Lit Shader�� ä��ϴ�.")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Materialize Shader�� ä��ϴ�.")]
    #endregion
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    #region Tooltip
    [Tooltip("���� �ɾ�ٴ� �� ���� �浹 Ÿ��")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("���� �̵��ϱ� ���ϴ� ��� Ÿ��")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("��Ʈ �̹��� ���������� ä��ϴ�.")]
    #endregion
    public GameObject heartPrefab;
    #region Tooltip
    [Tooltip("ź�� ������ ���������� ä��ϴ�.")]
    #endregion
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("���ھ� ������")]
    #endregion
    public GameObject scorePrefab;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("���� ������ ������")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("��Ʈ ������ ��������Ʈ�� ä��ϴ�.")]
    #endregion
    public Sprite heartIcon;
    #region Tooltip
    [Tooltip("�Ѿ� ������ ��������Ʈ�� ä��ϴ�.")]
    #endregion
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion
    #region Tooltip
    [Tooltip("�̴ϸ� �ذ� ������")]
    #endregion
    public GameObject minimapSkullPrefab;


    #region Validation
#if UNITY_EDITOR
    // ��ũ���ͺ� ��ü �Է��� ��ȿ�� �˻��մϴ�.
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlip), tableFlip);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickup), healthPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickup), ammoPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickup), weaponPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLowSnapshot), musicLowSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
    }

#endif
    #endregion
}