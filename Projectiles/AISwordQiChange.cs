using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidWeapon.Projectiles;

public class AISwordQiChange : ModProjectile
{
    public override void SetStaticDefaults()
    {
        // 设置弹幕发光效果
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 拖尾效果长度
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 拖尾模式
    }

    public override void SetDefaults()
    {
        // 弹幕基础属性
        Projectile.width = 65;
        Projectile.height = 24; // 贴图大小65*24
        Projectile.aiStyle = 0; // 自定义AI
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = 5; // 增加穿透能力到5个敌怪
        Projectile.timeLeft = 60;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false; // 可穿墙
        Projectile.extraUpdates = 1; // 增加更新频率使速度更快
        Projectile.scale = 2.0f; // 设置初始弹幕大小为3倍
        Projectile.light = 3.0f; // 增强发光效果
        Projectile.usesLocalNPCImmunity = true; // 使用局部无敌帧
        Projectile.localNPCHitCooldown = 10; // 局部无敌帧时长
    }

    public override void AI()
    {
        // 延迟发射效果
        if (Projectile.ai[1] > 0)
        {
            Projectile.ai[1]--;
            Projectile.velocity = Vector2.Zero; // 延迟期间静止

            // 蓄力准备特效
            var prepareDust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.PurpleTorch, 0f, 0f, 100, Color.Purple,
                0.8f);
            prepareDust.noGravity = true;
            return;
        }

        // 弹幕大小随时间逐渐变小
        var timeProgress = (180 - Projectile.timeLeft) / 180f; // 0到1的进度
        Projectile.scale = 3.0f - timeProgress * 1.2f; // 从3.0缩放到1.8
        if (Projectile.scale < 0.5f) Projectile.scale = 0.5f; // 最小不低于0.5

        // 旋转效果
        Projectile.rotation += 0.7f; // 旋转速度更快（原0.4f）

        // 追踪敌怪逻辑
        var homingRange = 500f; // 追踪范围
        var homingSpeed = 0.15f; // 追踪速度调整因子

        // 寻找最近的敌人
        NPC targetNPC = null;
        var minDistance = homingRange;

        // 搜索范围内的敌人
        for (var i = 0; i < Main.maxNPCs; i++)
        {
            var npc = Main.npc[i];
            if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
            {
                var distance = Vector2.Distance(npc.Center, Projectile.Center);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetNPC = npc;
                }
            }
        }

        // 如果找到目标，调整弹幕方向
        if (targetNPC != null)
        {
            // 计算到目标的方向
            var direction = targetNPC.Center - Projectile.Center;
            direction.Normalize();

            // 逐渐调整弹幕速度方向
            Projectile.velocity =
                Vector2.Lerp(Projectile.velocity, direction * Projectile.velocity.Length(), homingSpeed);

            // 追踪时增加特殊追踪特效
            if (Main.rand.NextBool(2))
            {
                var targetDust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Electric, 0f, 0f, 100, Color.Violet,
                    1.2f * Projectile.scale);
                targetDust.velocity = direction * 2f;
                targetDust.noGravity = true;
                targetDust.fadeIn = 1.0f;
            }
        }

        // 紫色绚丽弹道效果
        for (var i = 0; i < 3; i++)
        {
            var dustPos = Projectile.position +
                          Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f);
            var dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.PurpleTorch, 0f, 0f, 100, Color.Purple,
                1.5f * Projectile.scale);
            dust.velocity = Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(2f, 2f);
            dust.noGravity = true;
            dust.fadeIn = 1.2f;
        }

        // 电光效果
        if (Main.rand.NextBool(3))
        {
            var electricDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                DustID.Electric, 0f, 0f, 100, Color.Violet, 1.0f * Projectile.scale);
            electricDust.velocity *= 0.5f;
            electricDust.noGravity = true;
        }

        // 紫色光芒拖尾
        if (Main.rand.NextBool(2))
        {
            var trailPos = Projectile.Center - Projectile.velocity * 0.5f;
            var trailDust = Dust.NewDustDirect(trailPos, 4, 4, DustID.Shadowflame, 0f, 0f, 150, Color.MediumPurple,
                0.8f * Projectile.scale);
            trailDust.velocity = Vector2.Zero;
            trailDust.noGravity = true;
            trailDust.fadeIn = 0.8f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        // 击中敌怪时的紫色爆炸特效
        for (var i = 0; i < 20; i++)
        {
            var dust = Dust.NewDustDirect(target.position, target.width, target.height,
                DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 2.0f * Projectile.scale);
            dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
            dust.noGravity = true;
            dust.fadeIn = 1.5f;
        }

        // 电光爆炸效果
        for (var i = 0; i < 10; i++)
        {
            var electricDust = Dust.NewDustDirect(target.Center, 0, 0,
                DustID.Electric, 0f, 0f, 100, Color.Violet, 1.8f * Projectile.scale);
            electricDust.velocity = Main.rand.NextVector2Circular(6f, 6f);
            electricDust.noGravity = true;
        }

        // 减少穿透次数
        Projectile.penetrate--;
        if (Projectile.penetrate <= 0) Projectile.Kill();
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        // 由于tileCollide = false，这个方法不会被调用
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        // 弹幕消失时的紫色绚丽爆炸效果
        for (var i = 0; i < 25; i++)
        {
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 2.5f * Projectile.scale);
            dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
            dust.noGravity = true;
            dust.fadeIn = 2.0f;
        }

        // 电光散射效果
        for (var i = 0; i < 15; i++)
        {
            var electricDust = Dust.NewDustDirect(Projectile.Center, 0, 0,
                DustID.Electric, 0f, 0f, 100, Color.Violet, 2.0f * Projectile.scale);
            electricDust.velocity = Main.rand.NextVector2Circular(12f, 12f);
            electricDust.noGravity = true;
        }

        // 暗影火焰环绕效果
        for (var i = 0; i < 8; i++)
        {
            var angle = i * MathHelper.TwoPi / 8f;
            var velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f;
            var shadowDust = Dust.NewDustDirect(Projectile.Center, 0, 0,
                DustID.Shadowflame, velocity.X, velocity.Y, 150, Color.MediumPurple, 1.5f * Projectile.scale);
            shadowDust.noGravity = true;
        }
    }
}