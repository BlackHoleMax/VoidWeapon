using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidWeapon.Projectiles
{
    public class AISwordPlusQi : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // 更长的拖尾效果
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            // 弹幕基础属性
            Projectile.width = 65;
            Projectile.height = 64;
            Projectile.aiStyle = 0; // 自定义AI
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3; // 可穿透3个敌怪
            Projectile.timeLeft = 80; // 4秒 (60帧 = 1秒)
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; // 可穿墙
            Projectile.extraUpdates = 1;
            Projectile.light = 2.0f; // 强发光效果
            Projectile.scale = 1.0f;
        }

        public override void AI()
        {
            // 无重力效果
            Projectile.velocity.Y *= 0.99f; // 几乎无重力

            // 连续旋转效果，与AISwordQi一致
            Projectile.rotation += 0.3f; // 只保留正常旋转效果

            // 自动追踪逻辑
            float homingRange = 600f; // 追踪范围
            float homingSpeed = 0.2f; // 追踪速度

            // 寻找最近的敌人
            NPC targetNPC = null;
            float minDistance = homingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetNPC = npc;
                    }
                }
            }

            // 追踪目标
            if (targetNPC != null)
            {
                Vector2 direction = targetNPC.Center - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * Projectile.velocity.Length(), homingSpeed);

                // 追踪时的特效
                if (Main.rand.NextBool(2))
                {
                    Color rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.03f) % 1f, 1f, 0.8f);
                    Dust trackDust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.RainbowMk2,
                        0f, 0f, 100, rainbowColor, 1.5f);
                    trackDust.velocity = direction * 3f;
                    trackDust.noGravity = true;
                }
            }

            // 彩虹色弹道效果
            for (int i = 0; i < 4; i++)
            {
                Color rainbowColor = Main.hslToRgb(((Main.GameUpdateCount + i * 15) * 0.02f) % 1f, 1f, 0.7f);
                Vector2 dustPos = Projectile.position + Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f);
                Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 1.8f);
                dust.velocity = Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(3f, 3f);
                dust.noGravity = true;
                dust.fadeIn = 1.3f;
            }

            // 虚空效果
            if (Main.rand.NextBool(3))
            {
                Dust voidDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Shadowflame, 0f, 0f, 100, Color.DarkViolet, 1.2f);
                voidDust.velocity *= 0.5f;
                voidDust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 击中时生成六个方向的子弹幕
            Vector2 targetCenter = target.Center;

            // 六个方向：正上、正下、左上、左下、右上、右下
            Vector2[] directions = new Vector2[]
            {
                new Vector2(0, -1),      // 正上
                new Vector2(0, 1),       // 正下
                new Vector2(-0.7f, -0.7f), // 左上
                new Vector2(-0.7f, 0.7f),  // 左下
                new Vector2(0.7f, -0.7f),  // 右上
                new Vector2(0.7f, 0.7f)    // 右下
            };

            // 在每个方向生成子弹幕
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 spawnPos = targetCenter + directions[i] * 80f; // 距离敌怪80像素
                Vector2 velocity = directions[i] * 12f; // 朝敌怪方向射击

                // 创建子弹幕，使用新的弹幕类型
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, velocity,
                    ModContent.ProjectileType<AISwordPlusQi1>(), (int)(damageDone * 0.8f),
                    hit.Knockback * 0.6f, Projectile.owner);
            }

            // 击中特效
            for (int i = 0; i < 25; i++)
            {
                Color rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.02f + i * 0.1f) % 1f, 1f, 0.8f);
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 2.5f);
                dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
                dust.noGravity = true;
                dust.fadeIn = 2.0f;
            }

            // 虚空爆炸效果
            for (int i = 0; i < 15; i++)
            {
                Dust voidDust = Dust.NewDustDirect(target.Center, 0, 0,
                    DustID.Shadowflame, 0f, 0f, 100, Color.DarkViolet, 2.0f);
                voidDust.velocity = Main.rand.NextVector2Circular(8f, 8f);
                voidDust.noGravity = true;
            }

            // 减少穿透次数
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕消失时的彩虹爆炸效果
            for (int i = 0; i < 30; i++)
            {
                Color rainbowColor = Main.hslToRgb((i * 0.1f) % 1f, 1f, 0.8f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 3.0f);
                dust.velocity = Main.rand.NextVector2Circular(12f, 12f);
                dust.noGravity = true;
                dust.fadeIn = 2.5f;
            }

            // 虚空散射效果
            for (int i = 0; i < 20; i++)
            {
                Dust voidDust = Dust.NewDustDirect(Projectile.Center, 0, 0,
                    DustID.Shadowflame, 0f, 0f, 100, Color.DarkViolet, 2.5f);
                voidDust.velocity = Main.rand.NextVector2Circular(15f, 15f);
                voidDust.noGravity = true;
            }
        }
    }
}