using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VoidWeapon.Projectiles;

namespace VoidWeapon.Items.Weapons;

public class AISword : ModItem
{
    // 常量定义
    private const int BaseDamage = 85;
    private const int UseTimeValue = 20;
    private const int UseAnimationValue = 20;
    private const float ShootSpeed = 16f;
    private const int SpecialShotInterval = 4;

    private const int SoulOfNightRequired = 10;
    private const int SoulOfFrightRequired = 8;
    private const int TitaniumBarRequired = 15;
    private const int ObsidianRequired = 20;

    private const int DustChancePrimary = 2; // 1/2 的概率生成主粉尘
    private const int DustChanceSecondary = 4; // 1/4 的概率生成次级粉尘

    private int shotCount; // 发射计数器，用于控制特殊弹幕的发射频率

    /// <summary>
    ///     设置武器的基础属性
    /// </summary>
    public override void SetDefaults()
    {
        // 基础武器属性
        Item.damage = BaseDamage;
        Item.DamageType = DamageClass.Melee;
        Item.width = 60;
        Item.height = 64;
        Item.useTime = UseTimeValue;
        Item.useAnimation = UseAnimationValue;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.Pink;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;

        // 弹幕设置
        Item.shoot = ModContent.ProjectileType<AISwordQi>();
        Item.shootSpeed = ShootSpeed;
    }

    /// <summary>
    ///     处理武器发射弹幕的逻辑
    /// </summary>
    /// <param name="player">使用武器的玩家</param>
    /// <param name="source">弹幕发射源</param>
    /// <param name="position">发射位置</param>
    /// <param name="velocity">发射速度</param>
    /// <param name="type">弹幕类型</param>
    /// <param name="damage">弹幕伤害</param>
    /// <param name="knockback">击退力</param>
    /// <returns>是否成功发射弹幕</returns>
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        // 计算射击位置和速度
        var shootPosition = player.RotatedRelativePoint(player.MountedCenter, true);
        var shootVelocity = velocity.SafeNormalize(Vector2.UnitX * player.direction) * Item.shootSpeed;

        // 增加发射计数
        shotCount++;

        // 每第4次发射特殊弹幕
        if (shotCount % SpecialShotInterval == 0)
            // 发射AISwordQiChange特殊弹幕
            Projectile.NewProjectile(source, shootPosition, shootVelocity,
                ModContent.ProjectileType<AISwordQiChange>(),
                (int)(damage * 1.3f), knockback * 1.1f, player.whoAmI, 1f);
        else
            // 发射普通AISwordQi弹幕
            Projectile.NewProjectile(source, shootPosition, shootVelocity, type, damage, knockback, player.whoAmI);

        return false;
    }

    /// <summary>
    ///     注册武器的合成配方
    /// </summary>
    public override void AddRecipes()
    {
        var recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SoulofNight, SoulOfNightRequired);
        recipe.AddIngredient(ItemID.SoulofFright, SoulOfFrightRequired);
        recipe.AddIngredient(ItemID.TitaniumBar, TitaniumBarRequired);
        recipe.AddIngredient(ItemID.Obsidian, ObsidianRequired);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }

    /// <summary>
    ///     武器挥舞时的视觉效果
    /// </summary>
    /// <param name="player">挥舞武器的玩家</param>
    /// <param name="hitbox">攻击范围的矩形区域</param>
    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        // 主要紫色视觉效果（50%概率）
        if (Main.rand.NextBool(DustChancePrimary))
        {
            var dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height,
                DustID.PurpleTorch, player.velocity.X * 0.2f + player.direction * 2,
                player.velocity.Y * 0.2f, 100, Color.MediumPurple, 1.5f);
            dust.noGravity = true;
            dust.fadeIn = 1.2f;
        }

        // 次级阴影火焰效果（25%概率）
        if (Main.rand.NextBool(DustChanceSecondary))
        {
            var dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height,
                DustID.Shadowflame, 0f, 0f, 120, Color.Purple, 1.2f);
            dust.noGravity = true;
        }
    }
}