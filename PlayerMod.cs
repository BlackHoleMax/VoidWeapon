using Terraria.ModLoader;

namespace VoidWeapon;

public class DashPlayer : ModPlayer
{
    public const int MaxRightClickChargeTime = 180; // 3秒蓄力时间 (60 FPS * 3)
    public const int MaxLeftClickChargeTime = 180; // 3秒蓄力时间
    public bool isLeftClickCharging;
    public bool isRightClickCharging;

    // 左键蓄力系统
    public int leftClickChargeTime;

    // 右键蓄力系统
    public int rightClickChargeTime;

    public override void ResetEffects()
    {
        // 每帧重置效果
    }

    // 右键蓄力相关方法
    public void StartRightClickCharging()
    {
        isRightClickCharging = true;
        rightClickChargeTime = 0;
    }

    public void UpdateRightClickCharging()
    {
        if (isRightClickCharging)
        {
            rightClickChargeTime++;
            if (rightClickChargeTime > MaxRightClickChargeTime)
                rightClickChargeTime = MaxRightClickChargeTime;
        }
    }

    public void StopRightClickCharging()
    {
        isRightClickCharging = false;
    }

    public float GetRightClickChargeProgress()
    {
        return (float)rightClickChargeTime / MaxRightClickChargeTime;
    }

    public bool IsRightClickFullyCharged()
    {
        return rightClickChargeTime >= MaxRightClickChargeTime;
    }

    // 左键蓄力相关方法
    public void StartLeftClickCharging()
    {
        isLeftClickCharging = true;
        leftClickChargeTime = 0;
    }

    public void UpdateLeftClickCharging()
    {
        if (isLeftClickCharging)
        {
            leftClickChargeTime++;
            if (leftClickChargeTime > MaxLeftClickChargeTime)
                leftClickChargeTime = MaxLeftClickChargeTime;
        }
    }

    public void StopLeftClickCharging()
    {
        isLeftClickCharging = false;
    }

    public float GetLeftClickChargeProgress()
    {
        return (float)leftClickChargeTime / MaxLeftClickChargeTime;
    }

    public bool IsLeftClickFullyCharged()
    {
        return leftClickChargeTime >= MaxLeftClickChargeTime;
    }

    // 冲刺相关方法（在AISwordPlus中引用但未实现）
    public void StartChargedDash()
    {
        // 冲刺逻辑将在武器类中实现
    }
}