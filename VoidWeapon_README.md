# 虚空武器模组 (VoidWeapon Mod)

## 模组概述

- **模组名称**: VoidWeapon
- **命名空间**: VoidWeapon
- **版本**: 0.5
- **作者**: blackholemax

## 武器列表

### 1. 虚空追踪者 (AISword)

- 血肉墙后期武器
- 发射穿透弹幕
- 每第4次攻击发射追踪弹幕

### 2. 虚空逐梦者 (AISwordPlus)

- **类型**: 战士月后毕业武器
- **解锁条件**: 击败四柱后可用
- **特殊效果**: 武器名称发出彩虹色光芒
- **伤害**: 150 (四柱后期高伤害)
- **攻击速度**: 15帧 (较快)
- **击退**: 8
- **稀有度**: 红色 (四柱后稀有度)
- **贴图尺寸**: 61×64

## 弹幕系统

### 主弹幕 (AISwordPlusQi)

- **贴图尺寸**: 65×64
- **持续时间**: 4秒 (240帧)
- **特性**:
    - 无重力
    - 可穿墙
    - 可穿透3个敌怪
    - 自动追踪敌怪
    - 彩虹色弹道效果

### 子弹幕 (AISwordPlusQi1)

- **贴图尺寸**: 62×64
- **持续时间**: 1秒 (60帧)
- **特性**:
    - 无重力
    - 可穿墙
    - 可穿透1个敌怪
    - 朝敌怪方向射击

## 攻击机制

1. 挥舞武器时发射主弹幕 AISwordPlusQi
2. 主弹幕自动追踪敌怪
3. 当主弹幕击中敌怪时，在敌怪周围六个位置生成子弹幕：
    - 正上方
    - 正下方
    - 左上方
    - 左下方
    - 右上方
    - 右下方
4. 子弹幕朝敌怪方向射击

## 合成配方

- 虚空追踪者 × 1
- 夜明锭 × 15
- 日耀碎片 × 8
- 星云碎片 × 8
- 漩涡碎片 × 8
- 星尘碎片 × 8
- 合成站: 远古操作台

## 视觉效果

- 武器名称彩虹色光芒
- 挥舞时彩虹色和虚空特效
- 弹幕彩虹色拖尾效果
- 击中敌怪时彩虹爆炸效果
- 虚空暗影火焰效果

## 文件结构

```
VoidWeapon/
├── Items/Weapons/
│   ├── AISword.cs (虚空追踪者)
│   ├── AISword.png
│   ├── AISwordPlus.cs (虚空逐梦者)
│   └── AISwordPlus.png
├── Projectiles/
│   ├── AISwordQi.cs (基础弹幕)
│   ├── AISwordQi.png
│   ├── AISwordQiChange.cs (追踪弹幕)
│   ├── AISwordQiChange.png
│   ├── AISwordPlusQi.cs (主弹幕)
│   ├── AISwordPlusQi.png
│   ├── AISwordPlusQi1.cs (子弹幕)
│   └── AISwordPlusQi1.png
├── Localization/
│   └── en-US_Mods.VoidWeapon.hjson
├── VoidWeapon.cs (主模组文件)
├── VoidWeapon.csproj (项目文件)
├── build.txt
└── Voidseeker.sln
```

## 命名空间更改记录

- 所有文件的命名空间已从 `AIMOD` 更改为 `VoidWeapon`
- 主模组类名从 `AIMOD` 更改为 `VoidWeapon`
- 项目文件从 `AIMOD.csproj` 重命名为 `VoidWeapon.csproj`
- 本地化文件从 `en-US_Mods.AIMOD.hjson` 重命名为 `en-US_Mods.VoidWeapon.hjson`

## 注意事项

- 贴图文件需要按照指定尺寸制作
- 武器平衡性可根据测试结果调整
- 特效颜色和强度可以进一步优化
- 编译前请确保所有贴图文件存在