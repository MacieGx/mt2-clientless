﻿namespace MetinClientless.Packets;

public enum EServerToClient
{
    UNKNOWN_0,
    HEADER_GC_CHARACTER_ADD,
    HEADER_GC_CHARACTER_DEL,
    HEADER_GC_CHARACTER_MOVE,
    HEADER_GC_CHAT,
    HEADER_GC_SYNC_POSITION,
    HEADER_GC_LOGIN_SUCCESS3,
    HEADER_GC_LOGIN_FAILURE,
    HEADER_GC_PLAYER_CREATE_SUCCESS,
    HEADER_GC_PLAYER_CREATE_FAILURE,
    HEADER_GC_PLAYER_DELETE_SUCCESS,
    HEADER_GC_PLAYER_DELETE_WRONG_SOCIAL_ID,
    UNKNOWN_12,
    HEADER_GC_STUN,
    HEADER_GC_DEAD,
    HEADER_GC_MAIN_CHARACTER,
    HEADER_GC_PLAYER_POINTS,
    HEADER_GC_PLAYER_POINT_CHANGE,
    HEADER_GC_CHANGE_SPEED,
    HEADER_GC_CHARACTER_UPDATE,
    HEADER_GC_ITEM_SET,
    HEADER_GC_ITEM_SET2,
    HEADER_GC_ITEM_USE,
    HEADER_GC_ITEM_DROP,
    UNKNOWN_24,
    HEADER_GC_ITEM_UPDATE,
    HEADER_GC_ITEM_GROUND_ADD,
    HEADER_GC_ITEM_GROUND_DEL,
    HEADER_GC_QUICKSLOT_ADD,
    HEADER_GC_QUICKSLOT_DEL,
    HEADER_GC_QUICKSLOT_SWAP,
    HEADER_GC_ITEM_OWNERSHIP,
    HEADER_GC_LOGIN_SUCCESS4,
    HEADER_GC_ITEM_UNBIND_TIME,
    HEADER_GC_WHISPER,
    HEADER_GC_ALERT,
    HEADER_GC_MOTION,
    UNKNOWN_37,
    HEADER_GC_SHOP,
    HEADER_GC_SHOP_SIGN,
    HEADER_GC_DUEL_START,
    HEADER_GC_PVP,
    HEADER_GC_EXCHANGE,
    HEADER_GC_CHARACTER_POSITION,
    HEADER_GC_PING,
    HEADER_GC_SCRIPT,
    HEADER_GC_QUEST_CONFIRM,
    UNKNOWN_47,
    UNKNOWN_48,
    UNKNOWN_49,
    UNKNOWN_50,
    UNKNOWN_51,
    UNKNOWN_52,
    UNKNOWN_53,
    UNKNOWN_54,
    UNKNOWN_55,
    UNKNOWN_56,
    UNKNOWN_57,
    UNKNOWN_58,
    UNKNOWN_59,
    UNKNOWN_60,
    HEADER_GC_MOUNT,
    HEADER_GC_OWNERSHIP,
    HEADER_GC_TARGET,
    UNKNOWN_64,
    HEADER_GC_WARP,
    UNKNOWN_66,
    UNKNOWN_67,
    UNKNOWN_68,
    HEADER_GC_ADD_FLY_TARGETING,
    HEADER_GC_CREATE_FLY,
    HEADER_GC_FLY_TARGETING,
    HEADER_GC_SKILL_LEVEL,
    HEADER_GC_SKILL_COOLTIME_END,
    HEADER_GC_MESSENGER,
    HEADER_GC_GUILD,
    HEADER_GC_SKILL_LEVEL_NEW,
    HEADER_GC_PARTY_INVITE,
    HEADER_GC_PARTY_ADD,
    HEADER_GC_PARTY_UPDATE,
    HEADER_GC_PARTY_REMOVE,
    HEADER_GC_QUEST_INFO,
    HEADER_GC_REQUEST_MAKE_GUILD,
    HEADER_GC_PARTY_PARAMETER,
    HEADER_GC_SAFEBOX_MONEY_CHANGE,
    HEADER_GC_SAFEBOX_SET,
    HEADER_GC_SAFEBOX_DEL,
    HEADER_GC_SAFEBOX_WRONG_PASSWORD,
    HEADER_GC_SAFEBOX_SIZE,
    HEADER_GC_FISHING,
    HEADER_GC_EMPIRE,
    HEADER_GC_PARTY_LINK,
    HEADER_GC_PARTY_UNLINK,
    UNKNOWN_93,
    UNKNOWN_94,
    HEADER_GC_REFINE_INFORMATION,
    HEADER_GC_OBSERVER_ADD,
    HEADER_GC_OBSERVER_REMOVE,
    HEADER_GC_OBSERVER_MOVE,
    HEADER_GC_VIEW_EQUIP,
    HEADER_GC_MARK_BLOCK,
    HEADER_GC_MARK_DIFF_DATA,
    HEADER_GC_MARK_IDXLIST,
    UNKNOWN_103,
    UNKNOWN_104,
    UNKNOWN_105,
    HEADER_GC_TIME,
    HEADER_GC_CHANGE_NAME,
    UNKNOWN_108,
    UNKNOWN_109,
    HEADER_GC_DUNGEON,
    HEADER_GC_WALK_MODE,
    HEADER_GC_CHANGE_SKILL_GROUP,
    HEADER_GC_MAIN_CHARACTER2_EMPIRE,
    HEADER_GC_SEPCIAL_EFFECT,
    HEADER_GC_NPC_POSITION,
    HEADER_GC_CHINA_MATRIX_CARD,
    HEADER_GC_CHARACTER_UPDATE2,
    HEADER_GC_LOGIN_KEY,
    HEADER_GC_REFINE_INFORMATION_NEW,
    HEADER_GC_CHARACTER_ADD2,
    HEADER_GC_CHANNEL,
    HEADER_GC_MALL_OPEN,
    HEADER_GC_TARGET_UPDATE,
    HEADER_GC_TARGET_DELETE,
    HEADER_GC_TARGET_CREATE_NEW,
    HEADER_GC_AFFECT_ADD,
    HEADER_GC_AFFECT_REMOVE,
    HEADER_GC_MALL_SET,
    HEADER_GC_MALL_DEL,
    HEADER_GC_LAND_LIST,
    HEADER_GC_LOVER_INFO,
    HEADER_GC_LOVE_POINT_UPDATE,
    HEADER_GC_GUILD_SYMBOL_DATA,
    HEADER_GC_DIG_MOTION,
    HEADER_GC_DAMAGE_INFO,
    HEADER_GC_CHAR_ADDITIONAL_INFO,
    HEADER_GC_MAIN_CHARACTER3_BGM,
    HEADER_GC_MAIN_CHARACTER4_BGM_VOL,
    UNKNOWN_139,
    UNKNOWN_140,
    UNKNOWN_141,
    UNKNOWN_142,
    UNKNOWN_143,
    UNKNOWN_144,
    UNKNOWN_145,
    UNKNOWN_146,
    UNKNOWN_147,
    UNKNOWN_148,
    UNKNOWN_149,
    HEADER_GC_AUTH_SUCCESS,
    HEADER_GC_PANAMA_PACK,
    HEADER_GC_HYBRIDCRYPT_KEYS,
    HEADER_GC_HYBRIDCRYPT_SDB,
    HEADER_GC_AUTH_SUCCESS_OPENID,
    UNKNOWN_155,
    UNKNOWN_156,
    UNKNOWN_157,
    UNKNOWN_158,
    UNKNOWN_159,
    UNKNOWN_160,
    UNKNOWN_161,
    UNKNOWN_162,
    UNKNOWN_163,
    UNKNOWN_164,
    UNKNOWN_165,
    UNKNOWN_166,
    UNKNOWN_167,
    UNKNOWN_168,
    UNKNOWN_169,
    UNKNOWN_170,
    UNKNOWN_171,
    UNKNOWN_172,
    UNKNOWN_173,
    UNKNOWN_174,
    UNKNOWN_175,
    UNKNOWN_176,
    UNKNOWN_177,
    UNKNOWN_178,
    UNKNOWN_179,
    UNKNOWN_180,
    UNKNOWN_181,
    UNKNOWN_182,
    UNKNOWN_183,
    UNKNOWN_184,
    UNKNOWN_185,
    UNKNOWN_186,
    UNKNOWN_187,
    UNKNOWN_188,
    UNKNOWN_189,
    UNKNOWN_190,
    UNKNOWN_191,
    UNKNOWN_192,
    UNKNOWN_193,
    UNKNOWN_194,
    UNKNOWN_195,
    UNKNOWN_196,
    UNKNOWN_197,
    UNKNOWN_198,
    UNKNOWN_199,
    UNKNOWN_200,
    HEADER_GC_RUNUP_MATRIX_QUIZ,
    HEADER_GC_NEWCIBN_PASSPOD_REQUEST,
    HEADER_GC_NEWCIBN_PASSPOD_FAILURE,
    HEADER_GC_HS_REQUEST,
    HEADER_GC_XTRAP_CS1_REQUEST,
    UNKNOWN_206,
    UNKNOWN_207,
    HEADER_GC_SPECIFIC_EFFECT,
    HEADER_GC_DRAGON_SOUL_REFINE,
    HEADER_GC_RESPOND_CHANNELSTATUS,
    UNKNOWN_211,
    UNKNOWN_212,
    UNKNOWN_213,
    HEADER_GC_MT2009_SHOP_DATA,
    UNKNOWN_215,
    UNKNOWN_216,
    UNKNOWN_217,
    HEADER_GC_POLANDMT2_SHOP_DATA,
    UNKNOWN_219,
    UNKNOWN_220,
    UNKNOWN_221,
    UNKNOWN_222,
    UNKNOWN_223,
    UNKNOWN_224,
    UNKNOWN_225,
    UNKNOWN_226,
    UNKNOWN_227,
    UNKNOWN_228,
    UNKNOWN_229,
    UNKNOWN_230,
    UNKNOWN_231,
    UNKNOWN_232,
    UNKNOWN_233,
    UNKNOWN_234,
    UNKNOWN_235,
    UNKNOWN_236,
    UNKNOWN_237,
    UNKNOWN_238,
    UNKNOWN_239,
    UNKNOWN_240,
    UNKNOWN_241,
    UNKNOWN_242,
    UNKNOWN_243,
    UNKNOWN_244,
    UNKNOWN_245,
    UNKNOWN_246,
    UNKNOWN_247,
    UNKNOWN_248,
    UNKNOWN_249,
    HEADER_GC_KEY_AGREEMENT_COMPLETED,
    HEADER_GC_KEY_AGREEMENT,
    HEADER_GC_HANDSHAKE_OK,
    HEADER_GC_PHASE,
    HEADER_GC_BINDUDP,
    HEADER_GC_HANDSHAKE 
}