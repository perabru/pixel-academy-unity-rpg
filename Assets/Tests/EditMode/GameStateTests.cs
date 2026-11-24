using NUnit.Framework;
using Aurora;
public class GameStateTests
{
    [Test] public void PotionDoesNotWasteAtFullHealth() { var s = new GameState(); Assert.IsFalse(s.UsePotion()); Assert.AreEqual(2,s.potions); }
    [Test] public void PotionHealsAndConsumesExactlyOnce() { var s = new GameState{hp=2}; Assert.IsTrue(s.UsePotion()); Assert.AreEqual(5,s.hp); Assert.AreEqual(1,s.potions); }
    [Test] public void DeadPlayerCannotUsePotion() { var s = new GameState{hp=0}; Assert.IsFalse(s.UsePotion()); }
    [Test] public void CrystalCannotBeCollectedTwice() { var s = new GameState(); Assert.IsTrue(s.CollectCrystal("a")); Assert.IsFalse(s.CollectCrystal("a")); Assert.AreEqual(1,s.crystals); }
    [Test] public void ThreeCrystalsAreRequiredForKey() { var s = new GameState{questAccepted=true}; s.CollectCrystal("a");s.CollectCrystal("b");Assert.IsFalse(s.ClaimKey());s.CollectCrystal("c");Assert.IsTrue(s.ClaimKey());Assert.IsFalse(s.ClaimKey());Assert.AreEqual(10,s.coins); }
    [Test] public void KeyRequiresQuestAcceptance() { var s = new GameState{crystals=3};Assert.IsFalse(s.ClaimKey()); }
    [Test] public void ShopCannotOverdrawCoins() { var s=new GameState{coins=4};Assert.IsFalse(s.BuyPotion());Assert.AreEqual(4,s.coins);s.coins=5;Assert.IsTrue(s.BuyPotion());Assert.AreEqual(0,s.coins); }
    [Test] public void DamageCannotMakeNegativeHealth() { var s = new GameState(); s.Damage(20); Assert.AreEqual(0,s.hp); Assert.IsFalse(s.Damage(1)); }
    [Test] public void CorruptCoordinatesAreSanitized() { var s=new GameState{positionX=float.NaN,positionY=float.PositiveInfinity};s.Validate();Assert.AreEqual(5,s.positionX);Assert.AreEqual(-8,s.positionY); }
    [Test] public void SaveVersionMustBeSupported() { var s=new GameState{version=99};Assert.Throws<System.InvalidOperationException>(()=>s.Validate()); }
    [Test] public void VictoryRestoresPrerequisiteFlags() { var s=new GameState{bossDefeated=true};s.Validate();Assert.IsTrue(s.templeKey);Assert.IsTrue(s.gateOpen);Assert.AreEqual(3,s.crystals); }
    [Test] public void DuplicateCollectedIdsAreRemoved() { var s=new GameState();s.collectedIds.Add("a");s.collectedIds.Add("a");s.Validate();Assert.AreEqual(1,s.collectedIds.Count); }
}
