using UnityEngine;

namespace Aurora
{
    public enum InteractionKind { Elder, Shrine, Portal, Chest, Crystal, Rune }
    public sealed class Interactable : MonoBehaviour
    {
        public string id, label; public InteractionKind kind; public int target;
        public void Use()
        {
            var game = GameSession.Instance; var state = game.State;
            switch (kind)
            {
                case InteractionKind.Elder:
                    if (state.bossDefeated) game.HUD.Dialogue("Íris", "A luz voltou ao farol. Você ouviu o bosque, enfrentou seus ecos e trouxe a esperança de volta. Obrigada, exploradora! Você concluiu Ecos de Aurora.");
                    else if (!state.questAccepted) { state.questAccepted = true; game.HUD.Dialogue("Íris · a guardiã", "O farol se apagou. Três cristais no Bosque dos Ecos guardam sua luz. Siga a trilha para leste, encontre os cristais e volte até mim. Use Espaço para defender-se e Q para tomar uma poção."); }
                    else if (state.ClaimKey()) game.HUD.Dialogue("Íris", "Você reuniu os três cristais! Leve esta chave ao templo no sudeste do bosque. Para abrir o selo, lembre-se: primeiro o Sol, depois a Lua e, por fim, a Estrela. Receba também 10 moedas.");
                    else game.HUD.ShowShop();
                    game.Save(); break;
                case InteractionKind.Shrine:
                    state.hp = state.maxHp; game.Save(); AudioFeedback.Play("pickup"); game.HUD.Toast("Vida restaurada. Progresso salvo."); break;
                case InteractionKind.Portal:
                    if (target == 2 && !state.templeKey) { game.HUD.Toast("O templo pede a chave de Íris. Complete a missão dos cristais."); return; }
                    game.Enter(target, target == 1 && state.sceneIndex == 2 ? new Vector2(27, -18) : game.World.SafeSpawn(target)); break;
                case InteractionKind.Chest:
                    if (state.Collect(id)) { state.coins += 5; state.potions = Mathf.Min(99, state.potions + 1); AudioFeedback.Play("pickup"); game.HUD.Toast("Baú: 5 moedas e 1 poção."); Destroy(gameObject); game.Save(); } break;
                case InteractionKind.Crystal:
                    if (!state.questAccepted) { game.HUD.Toast("Uma luz misteriosa... Fale com Íris na vila."); return; }
                    if (state.CollectCrystal(id)) { AudioFeedback.Play("pickup"); game.HUD.Toast("Cristais: " + state.crystals + "/3"); Destroy(gameObject); game.Save(); } break;
                case InteractionKind.Rune: game.Rune(target); break;
            }
        }
    }
}
