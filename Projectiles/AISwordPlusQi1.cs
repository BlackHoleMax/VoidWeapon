using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidWeapon.Projectiles
{
    // 子弹幕 - 六方向爆裂弹幕
    public class AISwordPlusQi1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        
        public override void SetDefaults()
        {
            // 弹幕基础属性
            Projectile.width = 62;
            Projectile.height = 64;
            Projectile.aiStyle = 0; // 自定义AI
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1; // 可穿透1个敌怪
            Projectile.timeLeft = 60; // 1秒
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; // 可穿墙
            Projectile.extraUpdates = 1;
            Projectile.light = 0.8f;
            Projectile.scale = 0.4f; // 稍小一些
        }

        public override void AI()
        {
            // 设置初始速度大小（用于后续追踪时保持速度）
            float initialSpeed = 12f; // 与在AISwordPlusQi.cs中设置的速度一致
            
            // 在第一帧时保存初始速度大小
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = initialSpeed;
            }
            
            // 前20帧直线飞行，之后开始追踪
            if (Projectile.timeLeft <= 40)
            {
                // 自动追踪逻辑
                float homingRange = 500f; // 追踪范围
                float homingSpeed = 0.15f; // 追踪速度调整因子
                
                // 寻找最近的敌人
                NPC targetNPC = null;
                float minDistance = homingRange;
                
                // 搜索范围内的敌人
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
                
                // 如果找到目标，调整弹幕方向
                if (targetNPC != null)
                {
                    // 计算到目标的方向
                    Vector2 direction = targetNPC.Center - Projectile.Center;
                    direction.Normalize();
                    
                    // 逐渐调整弹幕速度方向
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * Projectile.ai[0], homingSpeed);
                }
            }
            
            // 根据速度方向设置旋转，让剑尖指向运动方向
            // 由于图片是45度倾斜的，需要减去45度(π/4弧度)来校正
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            
            // 彩虹色弹道效果（较少的粒子）
            if (Main.rand.NextBool(2))
            {
                Color rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.03f) % 1f, 1f, 0.6f);
                Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(20f, 20f);
                Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 1.2f);
                dust.velocity = Projectile.velocity * 0.2f;
                dust.noGravity = true;
                dust.fadeIn = 1.0f;
            }
            
            // 虚空效果
            if (Main.rand.NextBool(4))
            {
                Dust voidDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Shadowflame, 0f, 0f, 100, Color.Purple, 1.0f);
                voidDust.velocity *= 0.3f;
                voidDust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 击中特效（较小规模）
            for (int i = 0; i < 15; i++)
            {
                Color rainbowColor = Main.hslToRgb((Main.GameUpdateCount * 0.02f + i * 0.15f) % 1f, 1f, 0.7f);
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, 
                    DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 1.8f);
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
            
            // 虚空爆炸效果
            for (int i = 0; i < 8; i++)
            {
                Dust voidDust = Dust.NewDustDirect(target.Center, 0, 0, 
                    DustID.Shadowflame, 0f, 0f, 100, Color.Purple, 1.5f);
                voidDust.velocity = Main.rand.NextVector2Circular(5f, 5f);
                voidDust.noGravity = true;
            }
            
            // 子弹幕只能穿透1个敌怪
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕消失时的小规模彩虹爆炸效果
            for (int i = 0; i < 12; i++)
            {
                Color rainbowColor = Main.hslToRgb((i * 0.2f) % 1f, 1f, 0.7f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.RainbowMk2, 0f, 0f, 100, rainbowColor, 2.0f);
                dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
                dust.noGravity = true;
                dust.fadeIn = 1.8f;
            }
            
            // 虚空散射效果
            for (int i = 0; i < 6; i++)
            {
                Dust voidDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 
                    DustID.Shadowflame, 0f, 0f, 100, Color.Purple, 1.8f);
                voidDust.velocity = Main.rand.NextVector2Circular(10f, 10f);
                voidDust.noGravity = true;
            }
        }
    }
}