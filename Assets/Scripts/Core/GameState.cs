using System;
using System.Collections.Generic;

namespace Aurora
{
    // Estado serializável sem dependência do motor: pode ser testado fora do Unity.
    [Serializable]
    public sealed class GameState
    {
        public int version = 1;
        public int hp = 6, maxHp = 6, coins = 0, potions = 2, crystals = 0;
        public bool questAccepted, templeKey, gateOpen, bossDefeated;
        public int sceneIndex;
        public float positionX = 5, positionY = -8;
        public List<string> collectedIds = new List<string>();

        public void Validate()
        {
            if (version != 1) throw new InvalidOperationException("Versão de salvamento incompatível.");
            maxHp = 6; hp = Math.Max(0, Math.Min(maxHp, hp));
            coins = Math.Max(0, Math.Min(coins, 9999));
            potions = Math.Max(0, Math.Min(potions, 99));
            crystals = Math.Max(0, Math.Min(crystals, 3));
            sceneIndex = Math.Max(0, Math.Min(sceneIndex, 2));
            if (float.IsNaN(positionX) || float.IsInfinity(positionX) || positionX < 1 || positionX > 30) positionX = 5;
            if (float.IsNaN(positionY) || float.IsInfinity(positionY) || positionY < -20 || positionY > -1) positionY = -8;
            if (collectedIds == null) collectedIds = new List<string>();
            collectedIds = new List<string>(new HashSet<string>(collectedIds.FindAll(id => !string.IsNullOrEmpty(id) && id.Length <= 80)));
            if (collectedIds.Count > 256) throw new InvalidOperationException("Arquivo contém registros demais.");
            if (templeKey) { questAccepted = true; crystals = 3; }
            if (gateOpen || bossDefeated) { templeKey = true; questAccepted = true; crystals = 3; }
            if (bossDefeated) gateOpen = true;
        }
        public bool Collect(string id)
        {
            if (string.IsNullOrEmpty(id) || collectedIds.Contains(id)) return false;
            collectedIds.Add(id); return true;
        }
        public bool CollectCrystal(string id)
        {
            if (crystals >= 3 || !Collect(id)) return false;
            crystals++; return true;
        }
        public bool UsePotion()
        {
            if (potions <= 0 || hp <= 0 || hp >= maxHp) return false;
            potions--; hp = Math.Min(maxHp, hp + 3); return true;
        }
        public bool ClaimKey()
        {
            if (!questAccepted || crystals < 3 || templeKey) return false;
            templeKey = true; coins += 10; return true;
        }
        public bool BuyPotion()
        {
            if (coins < 5 || potions >= 99) return false;
            coins -= 5; potions++; return true;
        }
        public bool Damage(int amount)
        {
            if (amount <= 0 || hp <= 0) return false;
            hp = Math.Max(0, hp - amount); return true;
        }
        public string Objective()
        {
            if (bossDefeated) return "Aurora voltou a brilhar. Fale com Íris na vila.";
            if (!questAccepted) return "Encontre Íris, a guardiã, no centro da vila.";
            if (crystals < 3) return "Recupere os cristais do bosque: " + crystals + "/3.";
            if (!templeKey) return "Volte à vila e entregue os cristais a Íris.";
            if (!gateOpen) return "Entre no templo. Acenda as runas na ordem Sol, Lua, Estrela.";
            return "Derrote o Sentinela e restaure o farol de Aurora.";
        }
    }
}
