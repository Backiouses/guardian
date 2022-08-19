using System.Collections.Generic;

public class HeroStat
{
	private static bool Init;

	public static Dictionary<string, HeroStat> StatCache;

	public string Name;

	public int Speed;

	public int Gas;

	public int Blade;

	public int Accel;

	public string SkillId = "petra";

	public static HeroStat GetInfo(string name)
	{
		InitData();
		return StatCache[name];
	}

	private static void InitData()
	{
		if (!Init)
		{
			Init = true;
			HeroStat heroStat = new HeroStat();
			heroStat.Name = "MIKASA";
			heroStat.SkillId = "mikasa";
			heroStat.Speed = 125;
			heroStat.Gas = 75;
			heroStat.Blade = 75;
			heroStat.Accel = 135;
			HeroStat heroStat2 = new HeroStat();
			heroStat2.Name = "LEVI";
			heroStat2.SkillId = "levi";
			heroStat2.Speed = 95;
			heroStat2.Gas = 100;
			heroStat2.Blade = 100;
			heroStat2.Accel = 150;
			HeroStat heroStat3 = new HeroStat();
			heroStat3.Name = "ARMIN";
			heroStat3.SkillId = "armin";
			heroStat3.Speed = 75;
			heroStat3.Gas = 150;
			heroStat3.Blade = 125;
			heroStat3.Accel = 85;
			HeroStat heroStat4 = new HeroStat();
			heroStat4.Name = "MARCO";
			heroStat4.SkillId = "marco";
			heroStat4.Speed = 110;
			heroStat4.Gas = 100;
			heroStat4.Blade = 115;
			heroStat4.Accel = 95;
			HeroStat heroStat5 = new HeroStat();
			heroStat5.Name = "JEAN";
			heroStat5.SkillId = "jean";
			heroStat5.Speed = 100;
			heroStat5.Gas = 150;
			heroStat5.Blade = 80;
			heroStat5.Accel = 100;
			HeroStat heroStat6 = new HeroStat();
			heroStat6.Name = "EREN";
			heroStat6.SkillId = "eren";
			heroStat6.Speed = 100;
			heroStat6.Gas = 90;
			heroStat6.Blade = 90;
			heroStat6.Accel = 100;
			HeroStat heroStat7 = new HeroStat();
			heroStat7.Name = "PETRA";
			heroStat7.SkillId = "petra";
			heroStat7.Speed = 80;
			heroStat7.Gas = 110;
			heroStat7.Blade = 100;
			heroStat7.Accel = 140;
			HeroStat heroStat8 = new HeroStat();
			heroStat8.Name = "SASHA";
			heroStat8.SkillId = "sasha";
			heroStat8.Speed = 140;
			heroStat8.Gas = 100;
			heroStat8.Blade = 100;
			heroStat8.Accel = 115;
			HeroStat heroStat9 = new HeroStat();
			heroStat9.SkillId = "jean";
			heroStat9.Speed = 100;
			heroStat9.Gas = 100;
			heroStat9.Blade = 100;
			heroStat9.Accel = 100;
			HeroStat heroStat10 = new HeroStat();
			heroStat10.Name = "AHSS";
			heroStat10.SkillId = "jean";
			heroStat10.Speed = 100;
			heroStat10.Gas = 100;
			heroStat10.Blade = 100;
			heroStat10.Accel = 100;
			StatCache = new Dictionary<string, HeroStat>();
			StatCache.Add("MIKASA", heroStat);
			StatCache.Add("LEVI", heroStat2);
			StatCache.Add("ARMIN", heroStat3);
			StatCache.Add("MARCO", heroStat4);
			StatCache.Add("JEAN", heroStat5);
			StatCache.Add("EREN", heroStat6);
			StatCache.Add("PETRA", heroStat7);
			StatCache.Add("SASHA", heroStat8);
			StatCache.Add("CUSTOM_DEFAULT", heroStat9);
			StatCache.Add("AHSS", heroStat10);
		}
	}
}
