using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum MapMode : ushort {
	MOVE, TIME
}

[Serializable]
public enum ElementType {
	NULL,       		// 格子不存在
	EMPTY,      		// 空
	RANDOM,     		// 随机, 用于编辑器
	FAKE,       		// 假元素,站位
	NORMAL_A,   		// 普通元素A
	NORMAL_B,   		// 普通元素B
	NORMAL_C,   		// 普通元素C
	NORMAL_D,   		// 普通元素D
	NORMAL_E,   		// 普通元素E
	NORMAL_F,   		// 普通元素F
	LINE_H,     		// 横消
	LINE_V,     		// 纵消
	BOMB,       		// 炸弹
	COLOR_ERASE,		// 同色消
	SAND_L1,    		// 沙地一层
	SAND_L2,    		// 沙地二层
	SAND_L3,    		// 沙地三层
	CIRRUS_L1,  		// 藤蔓一层
	CIRRUS_L2,  		// 藤蔓二层
	JELLY_L1,   		// 果冻一层
	JELLY_L2,   		// 果冻二层
	TIRE_L1,    		// 轮胎一层
	TIRE_L2,    		// 轮胎二层
	STONE_L1,   		// 石头一层
	STONE_L2,   		// 石头二层
	ICE_1,      		// 冰块1层
	ICE_2,      		// 冰块2层
	ICE_3,      		// 冰块3层
	WOOD_1,     		// 木板1层
	WOOD_2,     		// 木板2层
	WOOD_3,     		// 木板3层
	TREASURE_ICE_L1, 	// 钻石冰1层
	TREASURE_ICE_L2, 	// 钻石冰2层
	TREASURE_ICE_L3, 	// 钻石冰3层
	JAM,        		// 果酱
	COLLECTION1,		// 收集品1
	COLLECTION2,		// 收集品2
	COLLECTION3,		// 收集品3
}

[Serializable]
public enum ElementGroup {
    NORMAL,     // 普通元素
    LINE_H,     // 横消
    LINE_V,     // 纵消
    BOMB,       // 炸弹
    COLOR_ERASE,// 同色消
    SAND,       // 沙地
    CIRRUS,     // 藤蔓
    JELLY,      // 果冻
}

[Serializable]
public enum ElementLayer {
    BACKGROUND, // 背景层
    NULL,       // 无
    CIRRUS,     // 藤蔓
    JELLY,      // 果冻
    OBJECT,     // 普通元素
    SAND,       // 沙地层
    BAMBOO,     // 竹子
    OTHER,      // 其它
    COUNT       // 最大个数
}

[Serializable]
public enum PatternType
{
    FIVE_ONE_LINE,      // 5消
    FORE_ONE_LINE_H,    // 4消横向
    FORE_ONE_LINE_V,    // 4消纵向
    L_STYLE,            // L消
    T_STYLE,            // T消
    CROSS_STYLE,        // 十字消除
    THREE               // 3消
}

[Serializable]
public enum CellShape : uint
{
	CENTER, 
	WEST, 
	SOUTH, 
	COUNT
}

[Serializable]
[Flags]
public enum Direction : byte
{
    NONE = 0x00,
    LEFT = 0x01,
    RIGHT = 0x02,
    TOP = 0x10,
    BOTTOM = 0x20,
    LEFT_TOP = 0x11,
    RIGHT_TOP = 0x12,
    LEFT_BOTTOM = 0x21,
    RIGHT_BOTTOM = 0x22
}


public static class Config {
    public static readonly int CELL_SIZE = 128;
    public static readonly int DEFAULT_MAP_W = 11;
    public static readonly int DEFAULT_MAP_H = 9;
}
