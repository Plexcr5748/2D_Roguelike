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
    [Tooltip("던전의 RoomNodeTypeListSO로 채웁니다.")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER SELECTION
    [Space(10)]
    [Header("PLAYER SELECTION")]
    #endregion
    #region Tooltip
    [Tooltip("플레이어 선택 프리팹")]
    #endregion
    public GameObject playerSelectionPrefab;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    #region Tooltip
    [Tooltip("플레이어 세부 정보 리스트 - 플레이어 세부 정보를 스크립터블 객체로 채웁니다.")]
    #endregion
    public List<PlayerDetailsSO> playerDetailsList;
    #region Tooltip
    [Tooltip("현재 플레이어 스크립터블 객체 - 씬 간에 현재 플레이어를 참조하는 데 사용됩니다.")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    #region Tooltip
    [Tooltip("음악 마스터 믹서 그룹으로 채웁니다.")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    #region Tooltip
    [Tooltip("메인 메뉴 음악 스크립터블 객체")]
    #endregion
    public MusicTrackSO mainMenuMusic;
    #region Tooltip
    [Tooltip("음악 on full 스냅샷")]
    #endregion
    public AudioMixerSnapshot musicOnFullSnapshot;
    #region Tooltip
    [Tooltip("음악 low 스냅샷")]
    #endregion
    public AudioMixerSnapshot musicLowSnapshot;
    #region Tooltip
    [Tooltip("음악 off 스냅샷")]
    #endregion
    public AudioMixerSnapshot musicOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    #region Tooltip
    [Tooltip("사운드 마스터 믹서 그룹으로 채웁니다.")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    #region Tooltip
    [Tooltip("문 열기/닫기 효과음")]
    #endregion
    public SoundEffectSO doorOpenCloseSoundEffect;
    #region Tooltip
    [Tooltip("테이블 뒤집기 효과음")]
    #endregion
    public SoundEffectSO tableFlip;
    #region Tooltip
    [Tooltip("상자 열기 효과음")]
    #endregion
    public SoundEffectSO chestOpen;
    #region Tooltip
    [Tooltip("체력 회복 아이템 효과음")]
    #endregion
    public SoundEffectSO healthPickup;
    #region Tooltip
    [Tooltip("무기 획득 아이템 효과음")]
    #endregion
    public SoundEffectSO weaponPickup;
    #region Tooltip
    [Tooltip("탄약 획득 아이템 효과음")]
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
    [Tooltip("Sprite-Lit-Default 재료")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Variable Lit Shader로 채웁니다.")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Materialize Shader로 채웁니다.")]
    #endregion
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    #region Tooltip
    [Tooltip("적이 걸어다닐 수 없는 충돌 타일")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("적이 이동하기 원하는 경로 타일")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("하트 이미지 프리팹으로 채웁니다.")]
    #endregion
    public GameObject heartPrefab;
    #region Tooltip
    [Tooltip("탄약 아이콘 프리팹으로 채웁니다.")]
    #endregion
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("스코어 프리팹")]
    #endregion
    public GameObject scorePrefab;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("상자 아이템 프리팹")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("하트 아이콘 스프라이트로 채웁니다.")]
    #endregion
    public Sprite heartIcon;
    #region Tooltip
    [Tooltip("총알 아이콘 스프라이트로 채웁니다.")]
    #endregion
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion
    #region Tooltip
    [Tooltip("미니맵 해골 프리팹")]
    #endregion
    public GameObject minimapSkullPrefab;


    #region Validation
#if UNITY_EDITOR
    // 스크립터블 객체 입력을 유효성 검사합니다.
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