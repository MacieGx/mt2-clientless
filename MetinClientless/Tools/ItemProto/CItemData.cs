using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

public class CItemData
{
    #region Constructors

    public CItemData()
    {
        Clear();
    }

    #endregion

    public void Clear()
    {
        m_strModelFileName = string.Empty;
        m_strSubModelFileName = string.Empty;
        m_strDropModelFileName = string.Empty;
        m_strIconFileName = string.Empty;
        m_strDescription = string.Empty;
        m_strSummary = string.Empty;
        m_strLODModelFileNameVector = new List<string>();

        m_dwVnum = 0;
        m_ItemTable = new SItemTable();
        m_ScaleTable = new SScaleTable();
        m_AuraScaleTable = new SAuraScaleTable();
        m_dwAuraEffectID = 0;
    }

    public void SetSummary(string summary)
    {
        m_strSummary = summary;
    }

    public void SetDescription(string description)
    {
        m_strDescription = description;
    }

    public SItemTable GetTable()
    {
        return m_ItemTable;
    }

    public uint GetIndex()
    {
        return m_dwVnum;
    }

    public string GetName()
    {
        return Encoding.ASCII.GetString(m_ItemTable.szLocaleName).TrimEnd('\0');
    }

    public string GetDescription()
    {
        return m_strDescription;
    }

    public string GetSummary()
    {
        return m_strSummary;
    }

    public byte GetType()
    {
        return m_ItemTable.bType;
    }

    public byte GetSubType()
    {
        return m_ItemTable.bSubType;
    }

    public uint GetRefine()
    {
        return m_ItemTable.dwRefinedVnum;
    }

    public string GetUseTypeString()
    {
        if (GetType() != (byte)EItemType.ITEM_TYPE_USE)
            return string.Empty;

        switch ((EUseSubTypes)GetSubType())
        {
            case EUseSubTypes.USE_POTION:
            case EUseSubTypes.USE_POTION_NODELAY:
            case EUseSubTypes.USE_POTION_CONTINUE:
                return "POTION";
            case EUseSubTypes.USE_TALISMAN:
                return "TALISMAN";
            case EUseSubTypes.USE_TUNING:
                return "TUNING";
            case EUseSubTypes.USE_MOVE:
                return "MOVE";
            case EUseSubTypes.USE_TREASURE_BOX:
                return "TREASURE_BOX";
            case EUseSubTypes.USE_MONEYBAG:
                return "MONEYBAG";
            case EUseSubTypes.USE_BAIT:
                return "BAIT";
            case EUseSubTypes.USE_ABILITY_UP:
                return "ABILITY_UP";
            case EUseSubTypes.USE_AFFECT:
                return "AFFECT";
            case EUseSubTypes.USE_CREATE_STONE:
                return "CREATE_STONE";
            case EUseSubTypes.USE_SPECIAL:
                return "SPECIAL";
            case EUseSubTypes.USE_CLEAR:
                return "CLEAR";
            case EUseSubTypes.USE_INVISIBILITY:
                return "INVISIBILITY";
            case EUseSubTypes.USE_DETACHMENT:
                return "DETACHMENT";
            case EUseSubTypes.USE_BUCKET:
                return "BUCKET";
            case EUseSubTypes.USE_CLEAN_SOCKET:
                return "CLEAN_SOCKET";
            case EUseSubTypes.USE_CHANGE_ATTRIBUTE:
                return "CHANGE_ATTRIBUTE";
            case EUseSubTypes.USE_ADD_ATTRIBUTE:
                return "ADD_ATTRIBUTE";
            case EUseSubTypes.USE_ADD_ACCESSORY_SOCKET:
                return "ADD_ACCESSORY_SOCKET";
            case EUseSubTypes.USE_PUT_INTO_ACCESSORY_SOCKET:
                return "PUT_INTO_ACCESSORY_SOCKET";
            case EUseSubTypes.USE_ADD_ATTRIBUTE2:
                return "ADD_ATTRIBUTE2";
            case EUseSubTypes.USE_RECIPE:
                return "RECIPE";
            case EUseSubTypes.USE_CHANGE_ATTRIBUTE2:
                return "CHANGE_ATTRIBUTE2";
            case EUseSubTypes.USE_BIND:
                return "BIND";
            case EUseSubTypes.USE_UNBIND:
                return "UNBIND";
            case EUseSubTypes.USE_TIME_CHARGE_PER:
                return "TIME_CHARGE_PER";
            case EUseSubTypes.USE_TIME_CHARGE_FIX:
                return "TIME_CHARGE_FIX";
            case EUseSubTypes.USE_PUT_INTO_BELT_SOCKET:
                return "PUT_INTO_BELT_SOCKET";
            case EUseSubTypes.USE_PUT_INTO_RING_SOCKET:
                return "PUT_INTO_RING_SOCKET";
            case EUseSubTypes.USE_CHANGE_COSTUME_ATTR:
                return "CHANGE_COSTUME_ATTR";
            case EUseSubTypes.USE_RESET_COSTUME_ATTR:
                return "RESET_COSTUME_ATTR";
            case EUseSubTypes.USE_PUT_INTO_AURA_SOCKET:
                return "PUT_INTO_AURA_SOCKET";
            default:
                return "USE_UNKNOWN_TYPE";
        }
    }

    public uint GetWeaponType()
    {
        if (GetType() != (byte)EItemType.ITEM_TYPE_WEAPON)
            return (uint)EWeaponSubTypes.WEAPON_NONE;

        return GetSubType();
    }

    public byte GetSize()
    {
        return m_ItemTable.bSize;
    }

    public bool IsAntiFlag(uint dwFlag)
    {
        return (m_ItemTable.dwAntiFlags & dwFlag) != 0;
    }

    public bool IsFlag(uint dwFlag)
    {
        return (m_ItemTable.dwFlags & dwFlag) != 0;
    }

    public bool IsWearableFlag(uint dwFlag)
    {
        return (m_ItemTable.dwWearFlags & dwFlag) != 0;
    }

    public bool HasNextGrade()
    {
        return m_ItemTable.dwRefinedVnum != 0;
    }

    public uint GetWearFlags()
    {
        return m_ItemTable.dwWearFlags;
    }

    public uint GetIBuyItemPrice()
    {
        return m_ItemTable.dwIBuyItemPrice;
    }

    public uint GetISellItemPrice()
    {
        return m_ItemTable.dwISellItemPrice;
    }

    public bool GetLimit(byte byIndex, out SItemLimit pItemLimit)
    {
        pItemLimit = new SItemLimit();

        if (byIndex >= ITEM_LIMIT_MAX_NUM)
            return false;

        pItemLimit = m_ItemTable.aLimits[byIndex];
        return true;
    }

    public bool GetApply(byte byIndex, out SItemApply pItemApply)
    {
        pItemApply = new SItemApply();

        if (byIndex >= ITEM_APPLY_MAX_NUM)
            return false;

        pItemApply = m_ItemTable.aApplies[byIndex];
        return true;
    }

    public int GetValue(byte byIndex)
    {
        if (byIndex >= ITEM_VALUES_MAX_NUM)
            return 0;

        return m_ItemTable.alValues[byIndex];
    }

    public int GetSocket(byte byIndex)
    {
        if (byIndex >= ITEM_SOCKET_MAX_NUM)
            return 0;

        return m_ItemTable.alSockets[byIndex];
    }

    public int SetSocket(byte byIndex, int value)
    {
        if (byIndex >= ITEM_SOCKET_MAX_NUM)
            return 0;

        return m_ItemTable.alSockets[byIndex] = value;
    }

    public int GetSocketCount()
    {
        return ITEM_SOCKET_MAX_NUM;
    }

    public uint GetIconNumber()
    {
        return m_dwVnum;
    }

    public uint GetSpecularPoweru()
    {
        return m_ItemTable.bSpecular;
    }

    public float GetSpecularPowerf()
    {
        return m_ItemTable.bSpecular / 255.0f;
    }

    public bool IsEquipment()
    {
        switch ((EItemType)GetType())
        {
            case EItemType.ITEM_TYPE_WEAPON:
            case EItemType.ITEM_TYPE_ARMOR:
                return true;
        }

        return false;
    }

    public void SetDefaultItemData(string iconFileName, string modelFileName = null)
    {
        m_strIconFileName = iconFileName;

        if (modelFileName != null)
            m_strModelFileName = modelFileName;

        __LoadFiles();
    }

    public void SetItemTableData(SItemTable pItemTable)
    {
        m_ItemTable = pItemTable;
        m_dwVnum = pItemTable.dwVnum;
    }

    public void SetAuraScaleTableData(byte byJob, byte bySex, float fMeshScaleX, float fMeshScaleY, float fMeshScaleZ,
        float fParticleScale)
    {
        var index = bySex * 8 + byJob; // Assuming JOB_MAX_NUM = 8
        m_AuraScaleTable.v3MeshScale[index] = new Vector3(fMeshScaleX, fMeshScaleY, fMeshScaleZ);
        m_AuraScaleTable.fParticleScale[index] = fParticleScale;
    }

    public Vector3 GetAuraMeshScaleVector(byte byJob, byte bySex)
    {
        var index = bySex * 8 + byJob; // Assuming JOB_MAX_NUM = 8
        return m_AuraScaleTable.v3MeshScale[index];
    }

    public float GetAuraParticleScale(byte byJob, byte bySex)
    {
        var index = bySex * 8 + byJob; // Assuming JOB_MAX_NUM = 8
        return m_AuraScaleTable.fParticleScale[index];
    }

    public void SetAuraEffectID(string szAuraEffectPath)
    {
        // This would be implemented with the actual effect system
        m_dwAuraEffectID = (uint)szAuraEffectPath.GetHashCode(); // Placeholder implementation
    }

    public uint GetAuraEffectID()
    {
        return m_dwAuraEffectID;
    }

    public bool GetItemScale(uint dwPos, out float fScaleX, out float fScaleY, out float fScaleZ, out float fPositionX,
        out float fPositionY, out float fPositionZ)
    {
        fScaleX = fScaleY = fScaleZ = 1.0f;
        fPositionX = fPositionY = fPositionZ = 0.0f;

        if (dwPos >= 10)
            return false;

        fScaleX = m_ScaleTable.tInfo[dwPos].fScaleX;
        fScaleY = m_ScaleTable.tInfo[dwPos].fScaleY;
        fScaleZ = m_ScaleTable.tInfo[dwPos].fScaleZ;
        fPositionX = m_ScaleTable.tInfo[dwPos].fPositionX;
        fPositionY = m_ScaleTable.tInfo[dwPos].fPositionY;
        fPositionZ = m_ScaleTable.tInfo[dwPos].fPositionZ;

        return true;
    }

    public void SetItemScale(string strJob, string strSex, string strScaleX, string strScaleY, string strScaleZ,
        string strPositionX, string strPositionY, string strPositionZ)
    {
        uint dwPos = 0;

        if (strJob == "JOB_WARRIOR")
            dwPos = 0;
        else if (strJob == "JOB_ASSASSIN")
            dwPos = 1;
        else if (strJob == "JOB_SURA")
            dwPos = 2;
        else if (strJob == "JOB_SHAMAN")
            dwPos = 3;
        else if (strJob == "JOB_WOLFMAN")
            dwPos = 4;

        if (strSex == "SEX_MALE")
            dwPos += 0;
        else if (strSex == "SEX_FEMALE")
            dwPos += 5;

        if (dwPos >= 10)
            return;

        m_ScaleTable.tInfo[dwPos].fScaleX = float.Parse(strScaleX);
        m_ScaleTable.tInfo[dwPos].fScaleY = float.Parse(strScaleY);
        m_ScaleTable.tInfo[dwPos].fScaleZ = float.Parse(strScaleZ);
        m_ScaleTable.tInfo[dwPos].fPositionX = float.Parse(strPositionX);
        m_ScaleTable.tInfo[dwPos].fPositionY = float.Parse(strPositionY);
        m_ScaleTable.tInfo[dwPos].fPositionZ = float.Parse(strPositionZ);
    }

    private void __LoadFiles()
    {
        // Placeholder for loading files - would be implemented with actual file loading system
        // This method would load model, icon, etc. files in a real implementation
        __SetIconImage(m_strIconFileName);
    }

    private void __SetIconImage(string fileName)
    {
        // Placeholder for setting icon image - would be implemented with actual image loading system
        m_strIconFileName = fileName;
    }

    // Static methods for object pooling
    public static void DestroySystem()
    {
        s_pool.Clear();
    }

    public static CItemData New()
    {
        if (s_pool.Count > 0)
            return s_pool.Pop();

        return new CItemData();
    }

    public static void Delete(CItemData pkItemData)
    {
        if (pkItemData != null)
        {
            pkItemData.Clear();
            s_pool.Push(pkItemData);
        }
    }

    // Method to deserialize byte array into CItemData
    public static unsafe CItemData FromByteArray(byte[] data)
    {
        if (data == null || data.Length == 0)
            return null;

        var itemData = New();

        // Try to determine which version of the structure we're dealing with based on data length
        var structSize = 0;

        if (data.Length == Marshal.SizeOf(typeof(SItemTable_r152)))
        {
            structSize = Marshal.SizeOf(typeof(SItemTable_r152));
        }
        else if (data.Length == Marshal.SizeOf(typeof(SItemTable_r156)))
        {
            structSize = Marshal.SizeOf(typeof(SItemTable_r156));
        }
        else
        {
            // Console.WriteLine($"Invalid item proto size: {data.Length}. Falling back to r158. Some data may be missing.");
            structSize = Marshal.SizeOf(typeof(SItemTable_r158));
        }

        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            var itemTable = new SItemTable();

            switch (structSize)
            {
                case var _ when structSize == Marshal.SizeOf(typeof(SItemTable_r152)):
                    var table152 = ReadStructure<SItemTable_r152>(reader);
                    ConvertToItemTable(table152, ref itemTable);
                    break;

                case var _ when structSize == Marshal.SizeOf(typeof(SItemTable_r156)):
                    var table156 = ReadStructure<SItemTable_r156>(reader);
                    ConvertToItemTable(table156, ref itemTable);
                    break;

                case var _ when structSize == Marshal.SizeOf(typeof(SItemTable_r158)):
                    var table158 = ReadStructure<SItemTable_r158>(reader);
                    ConvertToItemTable(table158, ref itemTable);
                    break;
                
                default:
                    throw new Exception("Invalid item proto size");
            }

            itemData.SetItemTableData(itemTable);
        }

        return itemData;
    }

    private static T ReadStructure<T>(BinaryReader reader) where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        var buffer = reader.ReadBytes(size);

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();

        return result;
    }

    private static void ConvertToItemTable(SItemTable_r152 source, ref SItemTable target)
    {
        target.dwVnum = source.dwVnum;
        target.dwVnumRange = 0; // Not present in r152
        target.szName = source.szName;
        target.szLocaleName = source.szLocaleName;
        target.bType = source.bType;
        target.bSubType = source.bSubType;
        target.bWeight = source.bWeight;
        target.bSize = source.bSize;
        target.dwAntiFlags = source.dwAntiFlags;
        target.dwFlags = source.dwFlags;
        target.dwWearFlags = source.dwWearFlags;
        target.dwImmuneFlag = source.dwImmuneFlag;
        target.dwIBuyItemPrice = source.dwIBuyItemPrice;
        target.dwISellItemPrice = source.dwISellItemPrice;
        target.aLimits = source.aLimits;
        target.aApplies = source.aApplies;
        target.alValues = source.alValues;
        target.alSockets = source.alSockets;
        target.dwRefinedVnum = source.dwRefinedVnum;
        target.wRefineSet = source.wRefineSet;
        target.bAlterToMagicItemPct = source.bAlterToMagicItemPct;
        target.bSpecular = source.bSpecular;
        target.bGainSocketPct = source.bGainSocketPct;
    }

    private static void ConvertToItemTable(SItemTable_r156 source, ref SItemTable target)
    {
        target.dwVnum = source.dwVnum;
        target.dwVnumRange = source.dwVnumRange;
        target.szName = source.szName;
        target.szLocaleName = source.szLocaleName;
        target.bType = source.bType;
        target.bSubType = source.bSubType;
        target.bWeight = source.bWeight;
        target.bSize = source.bSize;
        target.dwAntiFlags = source.dwAntiFlags;
        target.dwFlags = source.dwFlags;
        target.dwWearFlags = source.dwWearFlags;
        target.dwImmuneFlag = source.dwImmuneFlag;
        target.dwIBuyItemPrice = source.dwIBuyItemPrice;
        target.dwISellItemPrice = source.dwISellItemPrice;
        target.aLimits = source.aLimits;
        target.aApplies = source.aApplies;
        target.alValues = source.alValues;
        target.alSockets = source.alSockets;
        target.dwRefinedVnum = source.dwRefinedVnum;
        target.wRefineSet = source.wRefineSet;
        target.bAlterToMagicItemPct = source.bAlterToMagicItemPct;
        target.bSpecular = source.bSpecular;
        target.bGainSocketPct = source.bGainSocketPct;
    }

    private static void ConvertToItemTable(SItemTable_r158 source, ref SItemTable target)
    {
        target.dwVnum = source.dwVnum;
        target.dwVnumRange = source.dwVnumRange;
        target.szName = source.szName;
        target.szLocaleName = source.szLocaleName;
        target.bType = source.bType;
        target.bSubType = source.bSubType;
        target.bWeight = source.bWeight;
        target.bSize = source.bSize;
        target.dwAntiFlags = source.dwAntiFlags;
        target.dwFlags = source.dwFlags;
        target.dwWearFlags = source.dwWearFlags;
        target.dwImmuneFlag = source.dwImmuneFlag;
        target.dwIBuyItemPrice = source.dwIBuyItemPrice;
        target.dwISellItemPrice = source.dwISellItemPrice;
        target.aLimits = source.aLimits;
        target.aApplies = source.aApplies;
        target.alValues = source.alValues;
        target.alSockets = source.alSockets;
        target.dwRefinedVnum = source.dwRefinedVnum;
        target.wRefineSet = source.wRefineSet;
        target.bAlterToMagicItemPct = source.bAlterToMagicItemPct;
        target.bSpecular = source.bSpecular;
        target.bGainSocketPct = source.bGainSocketPct;
        // Note: wWearableFlag from r158 is not stored in the base ItemTable
    }

    #region Constants

    public const int ITEM_NAME_MAX_LEN = 24;
    public const int ITEM_LIMIT_MAX_NUM = 2;
    public const int ITEM_VALUES_MAX_NUM = 6;
    public const int ITEM_SMALL_DESCR_MAX_LEN = 256;
    public const int ITEM_APPLY_MAX_NUM = 3;
    public const int ITEM_SOCKET_MAX_NUM = 3;

    #endregion

    #region Enums

    public enum EItemType
    {
        ITEM_TYPE_NONE,
        ITEM_TYPE_WEAPON,
        ITEM_TYPE_ARMOR,
        ITEM_TYPE_USE,
        ITEM_TYPE_AUTOUSE,
        ITEM_TYPE_MATERIAL,
        ITEM_TYPE_SPECIAL,
        ITEM_TYPE_TOOL,
        ITEM_TYPE_LOTTERY,
        ITEM_TYPE_ELK,
        ITEM_TYPE_METIN,
        ITEM_TYPE_CONTAINER,
        ITEM_TYPE_FISH,
        ITEM_TYPE_ROD,
        ITEM_TYPE_RESOURCE,
        ITEM_TYPE_CAMPFIRE,
        ITEM_TYPE_UNIQUE,
        ITEM_TYPE_SKILLBOOK,
        ITEM_TYPE_QUEST,
        ITEM_TYPE_POLYMORPH,
        ITEM_TYPE_TREASURE_BOX,
        ITEM_TYPE_TREASURE_KEY,
        ITEM_TYPE_SKILLFORGET,
        ITEM_TYPE_GIFTBOX,
        ITEM_TYPE_PICK,
        ITEM_TYPE_HAIR,
        ITEM_TYPE_TOTEM,
        ITEM_TYPE_BLEND,
        ITEM_TYPE_COSTUME,
        ITEM_TYPE_DS,
        ITEM_TYPE_SPECIAL_DS,
        ITEM_TYPE_EXTRACT,
        ITEM_TYPE_SECONDARY_COIN,
        ITEM_TYPE_RING,
        ITEM_TYPE_BELT,
        ITEM_TYPE_MAX_NUM
    }

    public enum EResourceSubTypes
    {
        RESOURCE_FISHBONE = 0,
        RESOURCE_WATERSTONEPIECE = 1,
        RESOURCE_WATERSTONE = 2,
        RESOURCE_BLOOD_PEARL = 3,
        RESOURCE_BLUE_PEARL = 4,
        RESOURCE_WHITE_PEARL = 5,
        RESOURCE_BUCKET = 6,
        RESOURCE_CRYSTAL = 7,
        RESOURCE_GEM = 8,
        RESOURCE_STONE = 9,
        RESOURCE_METIN = 10,
        RESOURCE_ORE = 11,
        RESOURCE_AURA
    }

    public enum EWeaponSubTypes
    {
        WEAPON_SWORD,
        WEAPON_DAGGER,
        WEAPON_BOW,
        WEAPON_TWO_HANDED,
        WEAPON_BELL,
        WEAPON_FAN,
        WEAPON_ARROW,
        WEAPON_MOUNT_SPEAR,
        WEAPON_CLAW,
        WEAPON_QUIVER,
        WEAPON_NUM_TYPES,
        WEAPON_NONE = WEAPON_NUM_TYPES + 1
    }

    public enum EMaterialSubTypes
    {
        MATERIAL_LEATHER,
        MATERIAL_BLOOD,
        MATERIAL_ROOT,
        MATERIAL_NEEDLE,
        MATERIAL_JEWEL,
        MATERIAL_DS_REFINE_NORMAL,
        MATERIAL_DS_REFINE_BLESSED,
        MATERIAL_DS_REFINE_HOLLY
    }

    public enum EArmorSubTypes
    {
        ARMOR_BODY,
        ARMOR_HEAD,
        ARMOR_SHIELD,
        ARMOR_WRIST,
        ARMOR_FOOTS,
        ARMOR_NECK,
        ARMOR_EAR,
        ARMOR_NUM_TYPES
    }

    public enum ECostumeSubTypes
    {
        COSTUME_BODY,
        COSTUME_HAIR,
        COSTUME_MOUNT = 2,
        COSTUME_ACCE = 3,
        COSTUME_WEAPON = 4,
        COSTUME_AURA = 5,
        COSTUME_NUM_TYPES
    }

    public enum EUseSubTypes
    {
        USE_POTION,
        USE_TALISMAN,
        USE_TUNING,
        USE_MOVE,
        USE_TREASURE_BOX,
        USE_MONEYBAG,
        USE_BAIT,
        USE_ABILITY_UP,
        USE_AFFECT,
        USE_CREATE_STONE,
        USE_SPECIAL,
        USE_POTION_NODELAY,
        USE_CLEAR,
        USE_INVISIBILITY,
        USE_DETACHMENT,
        USE_BUCKET,
        USE_POTION_CONTINUE,
        USE_CLEAN_SOCKET,
        USE_CHANGE_ATTRIBUTE,
        USE_ADD_ATTRIBUTE,
        USE_ADD_ACCESSORY_SOCKET,
        USE_PUT_INTO_ACCESSORY_SOCKET,
        USE_ADD_ATTRIBUTE2,
        USE_RECIPE,
        USE_CHANGE_ATTRIBUTE2,
        USE_BIND,
        USE_UNBIND,
        USE_TIME_CHARGE_PER,
        USE_TIME_CHARGE_FIX,
        USE_PUT_INTO_BELT_SOCKET,
        USE_PUT_INTO_RING_SOCKET,
        USE_CHANGE_COSTUME_ATTR,
        USE_RESET_COSTUME_ATTR,
        USE_PUT_INTO_AURA_SOCKET
    }

    public enum EDragonSoulSubType
    {
        DS_SLOT1,
        DS_SLOT2,
        DS_SLOT3,
        DS_SLOT4,
        DS_SLOT5,
        DS_SLOT6,
        DS_SLOT_NUM_TYPES = 6
    }

    public enum EMetinSubTypes
    {
        METIN_NORMAL,
        METIN_GOLD
    }

    public enum ELimitTypes
    {
        LIMIT_NONE,
        LIMIT_LEVEL,
        LIMIT_STR,
        LIMIT_DEX,
        LIMIT_INT,
        LIMIT_CON,
        LIMIT_PCBANG,
        LIMIT_REAL_TIME,
        LIMIT_REAL_TIME_START_FIRST_USE,
        LIMIT_TIMER_BASED_ON_WEAR,
        LIMIT_MAX_NUM
    }

    [Flags]
    public enum EItemAntiFlag : uint
    {
        ITEM_ANTIFLAG_FEMALE = 1 << 0,
        ITEM_ANTIFLAG_MALE = 1 << 1,
        ITEM_ANTIFLAG_WARRIOR = 1 << 2,
        ITEM_ANTIFLAG_ASSASSIN = 1 << 3,
        ITEM_ANTIFLAG_SURA = 1 << 4,
        ITEM_ANTIFLAG_SHAMAN = 1 << 5,
        ITEM_ANTIFLAG_GET = 1 << 6,
        ITEM_ANTIFLAG_DROP = 1 << 7,
        ITEM_ANTIFLAG_SELL = 1 << 8,
        ITEM_ANTIFLAG_EMPIRE_A = 1 << 9,
        ITEM_ANTIFLAG_EMPIRE_B = 1 << 10,
        ITEM_ANTIFLAG_EMPIRE_R = 1 << 11,
        ITEM_ANTIFLAG_SAVE = 1 << 12,
        ITEM_ANTIFLAG_GIVE = 1 << 13,
        ITEM_ANTIFLAG_PKDROP = 1 << 14,
        ITEM_ANTIFLAG_STACK = 1 << 15,
        ITEM_ANTIFLAG_MYSHOP = 1 << 16,
        ITEM_ANTIFLAG_SAFEBOX = 1 << 17,
        ITEM_ANTIFLAG_WOLFMAN = 1 << 18
    }

    [Flags]
    public enum EItemFlag : uint
    {
        ITEM_FLAG_REFINEABLE = 1 << 0,
        ITEM_FLAG_SAVE = 1 << 1,
        ITEM_FLAG_STACKABLE = 1 << 2,
        ITEM_FLAG_COUNT_PER_1GOLD = 1 << 3,
        ITEM_FLAG_SLOW_QUERY = 1 << 4,
        ITEM_FLAG_RARE = 1 << 5,
        ITEM_FLAG_UNIQUE = 1 << 6,
        ITEM_FLAG_MAKECOUNT = 1 << 7,
        ITEM_FLAG_IRREMOVABLE = 1 << 8,
        ITEM_FLAG_CONFIRM_WHEN_USE = 1 << 9,
        ITEM_FLAG_QUEST_USE = 1 << 10,
        ITEM_FLAG_QUEST_USE_MULTIPLE = 1 << 11,
        ITEM_FLAG_UNUSED03 = 1 << 12,
        ITEM_FLAG_LOG = 1 << 13,
        ITEM_FLAG_APPLICABLE = 1 << 14
    }

    public enum EWearPositions
    {
        WEAR_BODY,
        WEAR_HEAD,
        WEAR_FOOTS,
        WEAR_WRIST,
        WEAR_WEAPON,
        WEAR_NECK,
        WEAR_EAR,
        WEAR_UNIQUE1,
        WEAR_UNIQUE2,
        WEAR_ARROW,
        WEAR_SHIELD,
        WEAR_ABILITY1,
        WEAR_ABILITY2,
        WEAR_ABILITY3,
        WEAR_ABILITY4,
        WEAR_ABILITY5,
        WEAR_ABILITY6,
        WEAR_ABILITY7,
        WEAR_ABILITY8,
        WEAR_COSTUME_BODY,
        WEAR_COSTUME_HAIR,
        WEAR_COSTUME_MOUNT,
        WEAR_COSTUME_ACCE,
        WEAR_COSTUME_WEAPON,
        WEAR_COSTUME_AURA,
        WEAR_RING1,
        WEAR_RING2,
        WEAR_BELT,
        WEAR_MAX_NUM = 32
    }

    [Flags]
    public enum EItemWearableFlag : uint
    {
        WEARABLE_BODY = 1 << 0,
        WEARABLE_HEAD = 1 << 1,
        WEARABLE_FOOTS = 1 << 2,
        WEARABLE_WRIST = 1 << 3,
        WEARABLE_WEAPON = 1 << 4,
        WEARABLE_NECK = 1 << 5,
        WEARABLE_EAR = 1 << 6,
        WEARABLE_UNIQUE = 1 << 7,
        WEARABLE_SHIELD = 1 << 8,
        WEARABLE_ARROW = 1 << 9
    }

    public enum EApplyTypes
    {
        APPLY_NONE,
        APPLY_MAX_HP,
        APPLY_MAX_SP,
        APPLY_CON,
        APPLY_INT,
        APPLY_STR,
        APPLY_DEX,
        APPLY_ATT_SPEED,
        APPLY_MOV_SPEED,
        APPLY_CAST_SPEED,
        APPLY_HP_REGEN,
        APPLY_SP_REGEN,
        APPLY_POISON_PCT,
        APPLY_STUN_PCT,
        APPLY_SLOW_PCT,
        APPLY_CRITICAL_PCT,
        APPLY_PENETRATE_PCT,
        APPLY_ATTBONUS_HUMAN,
        APPLY_ATTBONUS_ANIMAL,
        APPLY_ATTBONUS_ORC,
        APPLY_ATTBONUS_MILGYO,
        APPLY_ATTBONUS_UNDEAD,
        APPLY_ATTBONUS_DEVIL,
        APPLY_STEAL_HP,
        APPLY_STEAL_SP,
        APPLY_MANA_BURN_PCT,
        APPLY_DAMAGE_SP_RECOVER,
        APPLY_BLOCK,
        APPLY_DODGE,
        APPLY_RESIST_SWORD,
        APPLY_RESIST_TWOHAND,
        APPLY_RESIST_DAGGER,
        APPLY_RESIST_BELL,
        APPLY_RESIST_FAN,
        APPLY_RESIST_BOW,
        APPLY_RESIST_FIRE,
        APPLY_RESIST_ELEC,
        APPLY_RESIST_MAGIC,
        APPLY_RESIST_WIND,
        APPLY_REFLECT_MELEE,
        APPLY_REFLECT_CURSE,
        APPLY_POISON_REDUCE,
        APPLY_KILL_SP_RECOVER,
        APPLY_EXP_DOUBLE_BONUS,
        APPLY_GOLD_DOUBLE_BONUS,
        APPLY_ITEM_DROP_BONUS,
        APPLY_POTION_BONUS,
        APPLY_KILL_HP_RECOVER,
        APPLY_IMMUNE_STUN,
        APPLY_IMMUNE_SLOW,
        APPLY_IMMUNE_FALL,
        APPLY_SKILL,
        APPLY_BOW_DISTANCE,
        APPLY_ATT_GRADE_BONUS,
        APPLY_DEF_GRADE_BONUS,
        APPLY_MAGIC_ATT_GRADE,
        APPLY_MAGIC_DEF_GRADE,
        APPLY_CURSE_PCT,
        APPLY_MAX_STAMINA,
        APPLY_ATT_BONUS_TO_WARRIOR,
        APPLY_ATT_BONUS_TO_ASSASSIN,
        APPLY_ATT_BONUS_TO_SURA,
        APPLY_ATT_BONUS_TO_SHAMAN,
        APPLY_ATT_BONUS_TO_MONSTER,
        APPLY_MALL_ATTBONUS,
        APPLY_MALL_DEFBONUS,
        APPLY_MALL_EXPBONUS,
        APPLY_MALL_ITEMBONUS,
        APPLY_MALL_GOLDBONUS,
        APPLY_MAX_HP_PCT,
        APPLY_MAX_SP_PCT,
        APPLY_SKILL_DAMAGE_BONUS,
        APPLY_NORMAL_HIT_DAMAGE_BONUS,
        APPLY_SKILL_DEFEND_BONUS,
        APPLY_NORMAL_HIT_DEFEND_BONUS,
        APPLY_EXTRACT_HP_PCT,
        APPLY_PC_BANG_EXP_BONUS,
        APPLY_PC_BANG_DROP_BONUS,
        APPLY_RESIST_WARRIOR,
        APPLY_RESIST_ASSASSIN,
        APPLY_RESIST_SURA,
        APPLY_RESIST_SHAMAN,
        APPLY_ENERGY,
        APPLY_DEF_GRADE,
        APPLY_COSTUME_ATTR_BONUS,
        APPLY_MAGIC_ATTBONUS_PER,
        APPLY_MELEE_MAGIC_ATTBONUS_PER,
        APPLY_RESIST_ICE,
        APPLY_RESIST_EARTH,
        APPLY_RESIST_DARK,
        APPLY_ANTI_CRITICAL_PCT,
        APPLY_ANTI_PENETRATE_PCT,
        APPLY_BLEEDING_REDUCE,
        APPLY_BLEEDING_PCT,
        APPLY_ATT_BONUS_TO_WOLFMAN,
        APPLY_RESIST_WOLFMAN,
        APPLY_RESIST_CLAW,
        APPLY_ACCEDRAIN_RATE,
        APPLY_RESIST_MAGIC_REDUCTION,
        MAX_APPLY_NUM = 130
    }

    [Flags]
    public enum EImmuneFlags : uint
    {
        IMMUNE_PARA = 1 << 0,
        IMMUNE_CURSE = 1 << 1,
        IMMUNE_STUN = 1 << 2,
        IMMUNE_SLEEP = 1 << 3,
        IMMUNE_SLOW = 1 << 4,
        IMMUNE_POISON = 1 << 5,
        IMMUNE_TERROR = 1 << 6
    }

    public enum EAuraGradeType
    {
        AURA_GRADE_NONE,
        AURA_GRADE_ORDINARY,
        AURA_GRADE_SIMPLE,
        AURA_GRADE_NOBLE,
        AURA_GRADE_SPARKLING,
        AURA_GRADE_MAGNIFICENT,
        AURA_GRADE_RADIANT,
        AURA_GRADE_MAX_NUM
    }

    public enum EAuraItem
    {
        AURA_BOOST_ITEM_VNUM_BASE = 49980
    }

    public enum EAuraBoostIndex
    {
        ITEM_AURA_BOOST_ERASER,
        ITEM_AURA_BOOST_WEAK,
        ITEM_AURA_BOOST_NORMAL,
        ITEM_AURA_BOOST_STRONG,
        ITEM_AURA_BOOST_ULTIMATE,
        ITEM_AURA_BOOST_MAX
    }

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemLimit
    {
        public byte bType;
        public int lValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemApply
    {
        public byte bType;
        public int lValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemTable
    {
        public uint dwVnum;
        public uint dwVnumRange;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szLocaleName;

        public byte bType;
        public byte bSubType;
        public byte bWeight;
        public byte bSize;
        public uint dwAntiFlags;
        public uint dwFlags;
        public uint dwWearFlags;
        public uint dwImmuneFlag;
        public uint dwIBuyItemPrice;
        public uint dwISellItemPrice;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_LIMIT_MAX_NUM)]
        public SItemLimit[] aLimits;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_APPLY_MAX_NUM)]
        public SItemApply[] aApplies;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_VALUES_MAX_NUM)]
        public int[] alValues;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_SOCKET_MAX_NUM)]
        public int[] alSockets;

        public uint dwRefinedVnum;
        public ushort wRefineSet;
        public byte bAlterToMagicItemPct;
        public byte bSpecular;
        public byte bGainSocketPct;
    }

    // Alternative structures for different versions
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemTable_r152
    {
        public uint dwVnum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szLocaleName;

        public byte bType;
        public byte bSubType;
        public byte bWeight;
        public byte bSize;
        public uint dwAntiFlags;
        public uint dwFlags;
        public uint dwWearFlags;
        public uint dwImmuneFlag;
        public uint dwIBuyItemPrice;
        public uint dwISellItemPrice;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_LIMIT_MAX_NUM)]
        public SItemLimit[] aLimits;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_APPLY_MAX_NUM)]
        public SItemApply[] aApplies;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_VALUES_MAX_NUM)]
        public int[] alValues;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_SOCKET_MAX_NUM)]
        public int[] alSockets;

        public uint dwRefinedVnum;
        public ushort wRefineSet;
        public byte bAlterToMagicItemPct;
        public byte bSpecular;
        public byte bGainSocketPct;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemTable_r156
    {
        public uint dwVnum;
        public uint dwVnumRange;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szLocaleName;

        public byte bType;
        public byte bSubType;
        public byte bWeight;
        public byte bSize;
        public uint dwAntiFlags;
        public uint dwFlags;
        public uint dwWearFlags;
        public uint dwImmuneFlag;
        public uint dwIBuyItemPrice;
        public uint dwISellItemPrice;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_LIMIT_MAX_NUM)]
        public SItemLimit[] aLimits;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_APPLY_MAX_NUM)]
        public SItemApply[] aApplies;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_VALUES_MAX_NUM)]
        public int[] alValues;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_SOCKET_MAX_NUM)]
        public int[] alSockets;

        public uint dwRefinedVnum;
        public ushort wRefineSet;
        public byte bAlterToMagicItemPct;
        public byte bSpecular;
        public byte bGainSocketPct;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SItemTable_r158
    {
        public uint dwVnum;
        public uint dwVnumRange;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_NAME_MAX_LEN + 1)]
        public byte[] szLocaleName;

        public byte bType;
        public byte bSubType;
        public byte bWeight;
        public byte bSize;
        public uint dwAntiFlags;
        public uint dwFlags;
        public uint dwWearFlags;
        public uint dwImmuneFlag;
        public uint dwIBuyItemPrice;
        public uint dwISellItemPrice;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_LIMIT_MAX_NUM)]
        public SItemLimit[] aLimits;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_APPLY_MAX_NUM)]
        public SItemApply[] aApplies;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_VALUES_MAX_NUM)]
        public int[] alValues;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_SOCKET_MAX_NUM)]
        public int[] alSockets;

        public uint dwRefinedVnum;
        public ushort wRefineSet;
        public byte bAlterToMagicItemPct;
        public byte bSpecular;
        public byte bGainSocketPct;
        public ushort wWearableFlag;
    }

    public struct SScaleInfo
    {
        public float fScaleX, fScaleY, fScaleZ;
        public float fPositionX, fPositionY, fPositionZ;
    }

    public struct SScaleTable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public SScaleInfo[] tInfo;
    }

    public struct SAuraScaleTable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 8)] // SEX_MAX_NUM * JOB_MAX_NUM
        public Vector3[] v3MeshScale;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 8)] // SEX_MAX_NUM * JOB_MAX_NUM
        public float[] fParticleScale;
    }

    #endregion

    #region Fields

    private string m_strModelFileName;
    private string m_strSubModelFileName;
    private string m_strDropModelFileName;
    private string m_strIconFileName;
    private string m_strDescription;
    private string m_strSummary;
    private List<string> m_strLODModelFileNameVector;

    private uint m_dwVnum;
    private SItemTable m_ItemTable;
    private SScaleTable m_ScaleTable;
    private SAuraScaleTable m_AuraScaleTable;
    private uint m_dwAuraEffectID;

    // Static pool for object pooling - equivalent to C++ ms_kPool
    private static readonly Stack<CItemData> s_pool = new();

    #endregion
}