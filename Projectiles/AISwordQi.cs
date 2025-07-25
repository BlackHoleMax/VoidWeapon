using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidWeapon.Projectiles
{
    public class AISwordQi : ModProjectile
    {
        public override void SetDefaults()
        {
            // 弹幕基础属性
            Projectile.width = 65;
            Projectile.height = 24; // 更新为实际贴图高度
            Projectile.aiStyle = 0; // 自定义AI
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3; // 可穿透3个敌怪
            Projectile.timeLeft = 40; // 持续3秒 (60帧 = 1秒)
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; // 可穿墙
            Projectile.extraUpdates = 1; // 增加更新频率使速度更快
            Projectile.light = 0.5f; // 发光效果
            Projectile.scale = 1.0f; // 默认缩放
        }

        public override void AI()
        {
            // 无重力下坠效果
            Projectile.velocity.Y *= 0.98f; // 轻微减速但不下坠
            
            // 紫色绚丽弹道效果
            for (int i = 0; i < 3; i++)
            {
                Vector2 dustPos = Projectile.position + Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f);
                Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 1.5f);
                dust.velocity = Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(2f, 2f);
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
            
            // 电光效果
            if (Main.rand.NextBool(3))
            {
                Dust electricDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Electric, 0f, 0f, 100, Color.Violet, 1.0f);
                electricDust.velocity *= 0.5f;
                electricDust.noGravity = true;
            }
            
            // 紫色光芒拖尾
            if (Main.rand.NextBool(2))
            {
                Vector2 trailPos = Projectile.Center - Projectile.velocity * 0.5f;
                Dust trailDust = Dust.NewDustDirect(trailPos, 4, 4, DustID.Shadowflame, 0f, 0f, 150, Color.MediumPurple, 0.8f);
                trailDust.velocity = Vector2.Zero;
                trailDust.noGravity = true;
                trailDust.fadeIn = 0.8f;
            }
            
            // 延迟发射效果（右键连续发射）
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                Projectile.velocity = Vector2.Zero; // 延迟期间静止
                
                // 蓄力准备特效
                Dust prepareDust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 0.8f);
                prepareDust.noGravity = true;
                return;
            }
            
            Projectile.rotation += 0.3f; // 只保留正常旋转效果
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 击中敌怪时的紫色爆炸特效
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, 
                    DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 2.0f);
                dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
            
            // 电光爆炸效果
            for (int i = 0; i < 10; i++)
            {
                Dust electricDust = Dust.NewDustDirect(target.Center, 0, 0, 
                    DustID.Electric, 0f, 0f, 100, Color.Violet, 1.8f);
                electricDust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                electricDust.noGravity = true;
            }
            
            // 减少穿透次数
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 由于tileCollide = false，这个方法不会被调用
            // 但保留以防将来需要修改
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕消失时的紫色绚丽爆炸效果
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.PurpleTorch, 0f, 0f, 100, Color.Purple, 2.5f);
                dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
                dust.noGravity = true;
                dust.fadeIn = 2.0f;
            }

            // 电光散射效果
            for (int i = 0; i < 15; i++)
            {
                Dust electricDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 
                    DustID.Electric, 0f, 0f, 100, Color.Violet, 2.0f);
                electricDust.velocity = Main.rand.NextVector2Circular(12f, 12f);
                electricDust.noGravity = true;
            }

            // 暗影火焰环绕效果
            for (int i = 0; i < 8; i++)
            {
                float angle = i * MathHelper.TwoPi / 8f;
                Vector2 velocity = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 5f;
                Dust shadowDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 
                    DustID.Shadowflame, velocity.X, velocity.Y, 150, Color.MediumPurple, 1.5f);
                shadowDust.noGravity = true;
            }
        }
    }
}