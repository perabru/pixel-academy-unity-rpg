using System;
using Aurora;
class Program
{
 static int count;
 static void Check(bool condition,string name){if(!condition)throw new Exception(name);count++;}
 static void Main()
 {
  var s=new GameState();Check(!s.UsePotion(),"full health");Check(s.potions==2,"no wasted potion");
  Check(s.Damage(4)&&s.hp==2,"damage");Check(s.UsePotion()&&s.hp==5&&s.potions==1,"healing");
  Check(s.UsePotion()&&s.hp==6&&s.potions==0,"healing clamp");Check(!s.UsePotion(),"empty inventory");
  Check(!s.ClaimKey(),"locked quest");s.questAccepted=true;
  Check(s.CollectCrystal("a"),"first crystal");Check(!s.CollectCrystal("a")&&s.crystals==1,"idempotent collection");
  Check(s.CollectCrystal("b"),"second crystal");Check(!s.ClaimKey(),"missing third crystal");
  Check(s.CollectCrystal("c"),"third crystal");Check(!s.CollectCrystal("d"),"crystal maximum");
  Check(s.ClaimKey()&&s.templeKey&&s.coins==10,"key reward");Check(!s.ClaimKey()&&s.coins==10,"reward once");
  Check(s.BuyPotion()&&s.coins==5&&s.potions==1,"purchase");Check(s.BuyPotion()&&s.coins==0,"second purchase");Check(!s.BuyPotion()&&s.coins==0,"no overdraft");
  s.Damage(999);Check(s.hp==0&&!s.UsePotion(),"death");Check(!s.Damage(1),"no damage after death");
  s=new GameState{hp=100,coins=-5,sceneIndex=99,positionX=float.NaN,positionY=float.PositiveInfinity};s.Validate();
  Check(s.hp==6&&s.coins==0&&s.sceneIndex==2,"sanitize ranges");Check(s.positionX==5&&s.positionY==-8,"sanitize coordinates");
  s.collectedIds.Add("a");s.collectedIds.Add("a");s.collectedIds.Add(null);s.Validate();Check(s.collectedIds.Count==1,"dedupe save");
  s=new GameState{bossDefeated=true};s.Validate();Check(s.gateOpen&&s.templeKey&&s.questAccepted&&s.crystals==3,"victory invariants");
  Check(s.Objective().Contains("Íris"),"victory objective");
  bool rejected=false;try{new GameState{version=2}.Validate();}catch(InvalidOperationException){rejected=true;}Check(rejected,"future save version");
  s=new GameState();Check(!s.Damage(-1)&&s.hp==6,"negative damage rejected");
  s=new GameState{coins=100,potions=99};Check(!s.BuyPotion()&&s.coins==100,"inventory capacity");
  Console.WriteLine(count+" domain checks passed. Unity runtime and player build are separate checks.");
 }
}
