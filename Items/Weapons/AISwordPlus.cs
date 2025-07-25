using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VoidWeapon.Projectiles;

namespace VoidWeapon.Items.Weapons;

public class AISwordPlus : ModItem
{
    private const int CooldownDuration = 180;

    // 冷却计时器
    private int cooldownTimer;

    // 强化攻击剩余次数
    private int empoweredAttacksLeft;

    // 强化状态标记
    private bool isEmpowered;

    public override void SetDefaults()
    {
        // 基础武器属性 - 四柱后毕业武器
        Item.damage = 150; // 四柱后期高伤害
        Item.DamageType = DamageClass.Melee;
        Item.width = 61;
        Item.height = 64;
        Item.useTime = 15; // 更快的攻击速度，与AISword一致
        Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 8;
        Item.value = Item.buyPrice(gold: 20); // 高价值
        Item.rare = ItemRarityID.Red; // 四柱后稀有度
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.noMelee = false;

        // 弹幕设置
        Item.shoot = ModContent.ProjectileType<AISwordPlusQi>();
        Item.shootSpeed = 20f; // 极快速度
    }

    public override void SetStaticDefaults()
    {
        // 彩虹色名称效果 - 使用本地化
    }

    public override void UpdateInventory(Player player)
    {
        // 更新冷却计时器
        if (cooldownTimer > 0) cooldownTimer--;
    }

    public override void HoldItem(Player player)
    {
        var dashPlayer = player.GetModPlayer<DashPlayer>();

        // 右键蓄力处理
        if (player.controlUseTile && cooldownTimer <= 0) // 右键按住且不在冷却中
        {
            // 开始或继续蓄力
            if (!dashPlayer.isRightClickCharging) dashPlayer.StartRightClickCharging();
            dashPlayer.UpdateRightClickCharging();

            // 右键蓄力特效
            var chargeProgress = dashPlayer.GetRightClickChargeProgress();
            if (chargeProgress > 0)
            {
                // 基础蓄力特效
                if (Main.rand.NextBool(3))
                {
                    var playerCenter = player.Center;
                    var color = Color.Lerp(Color.Purple, Color.Red, chargeProgress);
                    var dust = Dust.NewDustDirect(playerCenter, 0, 0, DustID.Shadowflame,
                        0f, 0f, 100, color, 1.0f + chargeProgress);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    dust.fadeIn = 1.2f;
                }

                // 彩虹特效
                if (Main.rand.NextBool(2))
                {
                    var playerCenter = player.Center;
                    var rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.05f + chargeProgress) % 1f, 1f, 0.8f);
                    var dust = Dust.NewDustDirect(playerCenter, 0, 0, DustID.RainbowMk2,
                        0f, 0f, 100, rainbowColor, 1.5f + chargeProgress);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Circular(3f * chargeProgress, 3f * chargeProgress);
                    dust.fadeIn = 1.5f;
                }

                // 满蓄力特效
                if (dashPlayer.IsRightClickFullyCharged())
                {
                    var playerCenter = player.Center;
                    var rainbowColor = Main.hslToRgb(Main.GameUpdateCount * 0.1f % 1f, 1f, 1f);
                    var dust = Dust.NewDustDirect(playerCenter, 0, 0, DustID.RainbowMk2,
                        0f, 0f, 100, rainbowColor, 3f);
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
                    dust.fadeIn = 2f;
                }
            }
        }
        else
        {
            // 右键释放时检查是否满蓄力
            if (dashPlayer.isRightClickCharging && dashPlayer.IsRightClickFullyCharged())
                // 自动激活强化状态
                ActivateEmpowerment(player);

            // 停止蓄力
            if (dashPlayer.isRightClickCharging) dashPlayer.StopRightClickCharging();
        }

        // 持握物品时的强化效果
        if (isEmpowered)
            // 添加强化时的视觉效果
            if (Main.rand.NextBool(5))
            {
                var dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, 0f, 0f, 100, Color.Purple, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
    }

    public override bool AltFunctionUse(Player player)
    {
        var dashPlayer = player.GetModPlayer<DashPlayer>();

        // 右键功能 - 开始蓄力
        if (!dashPlayer.isRightClickCharging) dashPlayer.StartRightClickCharging();

        return true;
    }

    public override bool CanUseItem(Player player)
    {
        // 右键用于蓄力，不直接使用物品
        if (player.altFunctionUse == 2) return false;

        return true;
    }

    public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
    {
        // 获取蓄力进度
        var dashPlayer = player.GetModPlayer<DashPlayer>();
        var chargeProgress = dashPlayer.GetLeftClickChargeProgress();

        // 根据蓄力程度增加攻击范围
        if (chargeProgress > 0.5f)
        {
            var expand = (int)(10 * chargeProgress);
            hitbox.X -= expand;
            hitbox.Y -= expand;
            hitbox.Width += expand * 2;
            hitbox.Height += expand * 2;
        }
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        // 获取蓄力进度
        var dashPlayer = player.GetModPlayer<DashPlayer>();
        var chargeProgress = dashPlayer.GetLeftClickChargeProgress();

        // 根据蓄力程度增加伤害
        if (chargeProgress > 0) damage *= 1f + chargeProgress * 1.5f; // 最多增加150%伤害
    }

    public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        // 获取蓄力进度
        var dashPlayer = player.GetModPlayer<DashPlayer>();
        var chargeProgress = dashPlayer.GetLeftClickChargeProgress();

        // 根据蓄力程度增加击退
        if (chargeProgress > 0) modifiers.Knockback *= 1f + chargeProgress * 1f; // 最多增加100%击退
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        // 如果在强化状态，执行特殊效果
        if (isEmpowered && empoweredAttacksLeft > 0)
        {
            // 向四面八方发射弹幕
            ExecuteDashAttack(player, player.GetSource_OnHit(target), target.Center, hit.Damage, hit.Knockback);

            // 减少强化攻击次数
            empoweredAttacksLeft--;

            // 如果强化攻击次数用完，退出强化状态
            if (empoweredAttacksLeft <= 0) isEmpowered = false;
        }
    }

    /// <summary>
    ///     激活强化状态的辅助方法
    /// </summary>
    private void ActivateEmpowerment(Player player)
    {
        // 激活强化状态，固定3次
        isEmpowered = true;
        empoweredAttacksLeft = 3;

        // 回复200生命值
        player.statLife += 200;
        if (player.statLife > player.statLifeMax2)
            player.statLife = player.statLifeMax2;
        player.HealEffect(200);

        // 添加激活强化的特效
        for (var i = 0; i < 50; i++)
        {
            var dust = Dust.NewDustDirect(player.position, player.width, player.height,
                DustID.Shadowflame, 0f, 0f, 100, Color.Purple, 2.0f);
            dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }

        // 设置冷却时间
        cooldownTimer = CooldownDuration;
    }

    /// <summary>
    ///     当武器蓄力攻击击中敌人时调用此方法
    /// </summary>
    public void OnDashAttack(NPC target, NPC.HitInfo hit, int damageDone, Vector2 playerCenter)
    {
        // 向四面八方发射弹幕
        ExecuteDashAttack(Main.LocalPlayer, Main.LocalPlayer.GetSource_OnHit(target), target.Center, hit.Damage,
            hit.Knockback);
    }

    private void ExecuteDashAttack(Player player, IEntitySource source, Vector2 position, int damage, float knockback)
    {
        // 向四面八方发射弹幕，总共16个
        var projectileCount = 16;
        var speed = 8f; // 弹幕速度

        for (var i = 0; i < projectileCount; i++)
        {
            // 计算角度
            var angle = MathHelper.TwoPi / projectileCount * i;
            var velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            // 在击中位置生成弹幕
            var spawnPosition = position;

            // 创建弹幕
            Projectile.NewProjectile(source, spawnPosition, velocity,
                ModContent.ProjectileType<AISwordPlusQi>(),
                (int)(damage * 1.3f), knockback * 1.8f, player.whoAmI);
        }

        // 添加视觉效果
        for (var i = 0; i < 50; i++)
        {
            var rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.02f + i * 0.1f) % 1f, 1f, 0.8f);
            var dust = Dust.NewDustDirect(position, 0, 0,
                DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 2.0f);
            dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        // 如果在强化状态，执行强化攻击
        if (isEmpowered && empoweredAttacksLeft > 0)
        {
            // 执行强化攻击 - 向四面八方发射弹幕
            ExecuteEmpoweredAttack(player, source, position, damage, knockback);

            // 减少强化攻击次数
            empoweredAttacksLeft--;

            // 如果强化攻击次数用完，退出强化状态
            if (empoweredAttacksLeft <= 0) isEmpowered = false;

            return false;
        }

        // 正常攻击
        var shootPosition = player.RotatedRelativePoint(player.MountedCenter, true);
        var shootVelocity = velocity.SafeNormalize(Vector2.UnitX * player.direction) * Item.shootSpeed;

        // 发射AISwordPlusQi弹幕
        Projectile.NewProjectile(source, shootPosition, shootVelocity, type, damage, knockback, player.whoAmI);

        return false;
    }

    private void ExecuteEmpoweredAttack(Player player, IEntitySource source, Vector2 position, int damage,
        float knockback)
    {
        // 向四面八方发射弹幕，总共16个
        var projectileCount = 16;
        var speed = 8f; // 弹幕速度

        for (var i = 0; i < projectileCount; i++)
        {
            // 计算角度
            var angle = MathHelper.TwoPi / projectileCount * i;
            var velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            // 在玩家位置生成弹幕
            var spawnPosition = player.Center;

            // 创建弹幕
            Projectile.NewProjectile(source, spawnPosition, velocity,
                ModContent.ProjectileType<AISwordPlusQi>(),
                (int)(damage * 1.3f), knockback * 1.8f, player.whoAmI);
        }

        // 添加视觉效果
        for (var i = 0; i < 50; i++)
        {
            var rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.02f + i * 0.1f) % 1f, 1f, 0.8f);
            var dust = Dust.NewDustDirect(player.position, player.width, player.height,
                DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 2.0f);
            dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        // 移除了左键蓄力旋转效果，使用默认挥动方式
    }

    public override void AddRecipes()
    {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<AISword>()); // 需要虚空追踪者
        recipe.AddIngredient(ItemID.LunarBar, 15); // 夜明锭
        recipe.AddIngredient(ItemID.FragmentSolar, 8); // 日耀碎片
        recipe.AddIngredient(ItemID.FragmentNebula, 8); // 星云碎片
        recipe.AddIngredient(ItemID.FragmentVortex, 8); // 漩涡碎片
        recipe.AddIngredient(ItemID.FragmentStardust, 8); // 星尘碎片
        recipe.AddTile(TileID.LunarCraftingStation); // 远古操作台
        recipe.Register();
    }

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        // 彩虹色挥舞特效
        if (Main.rand.NextBool(2))
        {
            // 彩虹色粒子效果
            var rainbowColor = Main.hslToRgb(Main.GameUpdateCount * 0.02f % 1f, 1f, 0.7f);
            var dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height,
                DustID.RainbowMk2, player.velocity.X * 0.2f + player.direction * 3,
                player.velocity.Y * 0.2f, 100, rainbowColor, 2.0f);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }

        // 虚空效果
        if (Main.rand.NextBool(3))
        {
            var voidDust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height,
                DustID.Shadowflame, 0f, 0f, 120, Color.DarkViolet, 1.8f);
            voidDust.noGravity = true;
        }

        // 如果在强化状态，添加额外特效
        if (isEmpowered)
        {
            var empoweredDust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height,
                DustID.PurpleTorch, player.velocity.X * 0.2f + player.direction * 3,
                player.velocity.Y * 0.2f, 100, Color.Purple, 2.2f);
            empoweredDust.noGravity = true;
            empoweredDust.fadeIn = 1.8f;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        // 让武器名称发出彩虹色光芒
        foreach (var line in tooltips)
            if (line.Name == "ItemName")
                line.OverrideColor = Main.hslToRgb(Main.GameUpdateCount * 0.02f % 1f, 1f, 0.8f);

        // 添加特殊功能说明
        var specialAbilityLine = new TooltipLine(Mod, "SpecialAbility", "右键蓄力3秒后自动激活强化：回复200生命值，接下来三次攻击向四面八方发射弹幕");
        specialAbilityLine.OverrideColor = Color.HotPink;
        tooltips.Add(specialAbilityLine);

        // 添加冷却时间说明
        var cooldownLine = new TooltipLine(Mod, "Cooldown", "强化冷却时间: 3秒");
        cooldownLine.OverrideColor = Color.Cyan;
        tooltips.Add(cooldownLine);
    }
}