# Ecos de Aurora — projeto Unity da Pixel Academy

RPG 2D original com visão superior, três áreas, missão dos cristais, combate de espada, mochila, poções, puzzle de runas, chefe e salvamento local. O projeto usa **Unity 6.3 LTS 6000.3.29f1** e C#. Os cenários, personagens e nomes são autorais; a referência a aventuras clássicas descreve o gênero, sem copiar personagens, mapas ou assets de Zelda.

**Estado desta entrega:** fontes, assets, material didático e scripts de build preparados. A compilação do Player e o teste dentro do Unity ainda precisam ser executados em um Editor ativado. Não há um `.exe` validado neste ZIP. `Validation/STATUS.md` separa verificações executadas das pendentes.

## Abrir pela primeira vez

1. Instale o Unity Hub a partir de https://unity.com/download e entre na sua conta. Ative uma licença válida; Unity Personal está sujeita às condições da Unity.
2. Instale o Editor **6000.3.29f1**. Para gerar Windows a partir de Linux/macOS, inclua o módulo **Windows Build Support (Mono)**. A página da versão é https://unity.com/releases/editor/whats-new/6000.3.29f1.
3. Extraia o ZIP inteiro em uma pasta gravável, como `C:\Projetos\EcosDeAurora`. Não abra o projeto de dentro do ZIP.
4. No Hub, escolha **Add / Add project from disk** e selecione a pasta que contém `Assets`, `Packages` e `ProjectSettings`.
5. Abra o projeto. Aguarde o Package Manager importar UGUI e o Test Framework. A primeira abertura requer internet para os pacotes.
6. No Editor, execute **Ecos de Aurora → Preparar projeto**. O comando configura as texturas e cria a cena `Assets/Scenes/Bootstrap.unity`. Se o Editor solicitar reinício por causa do input, reinicie.
7. Abra `Bootstrap.unity` e pressione **Play**. Clique em **Nova aventura**. Se a Console apresentar erros, corrija-os antes de testar o jogo; não ignore falhas de importação.

O gerador facilita abrir o projeto de referência. O curso ensina como construir os mesmos sistemas passo a passo; não exige que o aluno compreenda toda a geração antes do primeiro laboratório.

## Controles

| Ação | Tecla |
|---|---|
| Mover | WASD ou setas |
| Atacar com espada | Espaço |
| Conversar, abrir baú, ativar runa ou atravessar portal | E |
| Usar poção | Q |
| Mochila | Tab |
| Pausa | Esc |

Menus aceitam mouse e navegação padrão de teclado do UGUI. Esta versão é para computador; não inclui comandos de toque nem um build Android.

## Percurso completo

1. Na vila, vá até Íris, a guardiã, e pressione E. Ela pede três cristais.
2. Abra o baú de viagem no sul da vila. A saída para o bosque fica a leste.
3. No bosque, procure os três cristais: clareira noroeste, região ao lado do lago e sudeste. Use a espada contra os slimes, mantenha distância e tome poções quando necessário.
4. Volte à vila e fale novamente com Íris. Ela entrega a chave do templo e 10 moedas. É possível comprar poções por 5 moedas; cada uma cura até 3 pontos.
5. A entrada do templo fica no sudeste do bosque. No templo, ative as runas **Sol → Lua → Estrela**. Errar reinicia a sequência; interagir com Sol começa uma nova tentativa.
6. Atravesse o selo e enfrente o Sentinela. Ele tem 18 pontos de vida e fica mais rápido na segunda metade do combate. Desvie dos três projéteis e ataque quando se aproximar.
7. A vitória salva a aventura. Volte a Íris para encerrar a história.

Altares restauram toda a vida e salvam. A derrota retorna à vila com os objetivos preservados e desconta até 3 moedas.

## Organização

| Pasta / arquivo | Uso |
|---|---|
| `Assets/Scripts/Core/GameState.cs` | Regras de itens, missão e estado, sem dependência do motor |
| `Assets/Scripts/GameSession.cs` | Sessão, transições, pausa, dano, salvamento e vitória |
| `Assets/Scripts/WorldBuilder.cs` | Grade de 32×22, pisos, obstáculos, personagens e interações |
| `Assets/Scripts/PlayerController.cs` | Entrada, movimento físico, espada e interação |
| `Assets/Scripts/EnemyController.cs` | Slimes, Sentinela e projéteis |
| `Assets/Scripts/Interactable.cs` | NPC, altar, portal, baú, cristal e runas |
| `Assets/Scripts/GameHUD.cs` | Canvas, menus, HUD, inventário e diálogos |
| `Assets/Scripts/SaveSystem.cs` | JSON, validação e cópia de recuperação |
| `Assets/Editor/ProjectBuilder.cs` | Preparação e compilação do projeto |
| `Assets/Resources/Art` | 19 PNGs originais, 32×32 |
| `Assets/Resources/Audio` | 5 efeitos WAV originais |
| `Sources/Art` | Fontes SVG, gerador dos assets e licença |
| `Assets/Tests/EditMode` | Testes de domínio para o Unity Test Runner |
| `Validation` | Verificações de domínio fora do Editor e registro de limites |
| `Docs` | Roteiro de testes e instruções de publicação |

Os mapas são produzidos por uma grade determinística no código. A linha Y da grade vira coordenada negativa no mundo (`worldY = -linha`). Os PNGs são importados como **Default**, pois o runtime carrega `Texture2D` e chama `Sprite.Create` com 32 pixels por unidade. Usar importação `Sprite` é um laboratório alternativo; não é necessário para este projeto final.

## Compilar no Windows

No Editor: **Ecos de Aurora → Build Windows**. O resultado esperado é `Builds/Windows/EcosDeAurora.exe` acompanhado de `EcosDeAurora_Data`, `UnityPlayer.dll` e demais arquivos gerados. Distribua **a pasta inteira**, não só o `.exe`.

Ou abra PowerShell na raiz do projeto:

```powershell
./BuildWindows.ps1 -UnityEditor "C:\Program Files\Unity\Hub\Editor\6000.3.29f1\Editor\Unity.exe"
```

Esse comando usa o Editor já instalado e ativado. Não faz login, não recebe senhas e não compra licenças. Só gera o ZIP de distribuição se o processo terminar sem erro e o executável realmente existir.

No Linux, `./BuildLinux.sh /caminho/Editor/Unity` segue a mesma regra e gera um Player Linux. Um build Linux não substitui o executável Windows.

## Salvamento e limites

O JSON é armazenado em `Application.persistentDataPath/aurora-save-v1.json`, com uma cópia `.bak`. O caminho exato depende do sistema operacional; no Windows costuma estar em `AppData/LocalLow/Pixel Academy/Ecos de Aurora`. O jogo não envia dados para servidores e não inclui anúncios ou pagamentos.

Uma nova aventura substitui o slot ao salvar. A cópia `.bak` pode ajudar a recuperar o estado anterior. O jogo possui uma missão principal e três áreas; é um projeto didático compacto. Os personagens usam poses estáticas, com feedback de dano e efeitos de espada. Expansões como animação quadro a quadro, novos mapas e gamepad são atividades posteriores, não funcionalidades já entregues.

## Licença e estudo

Código e assets originais: MIT, conforme `LICENSE` e `Sources/Art/LICENSE.txt`. Unity e seus componentes permanecem sujeitos às licenças da Unity. O curso está na Pixel Academy; o jogo é independente e não representa produto oficial da Unity ou da Nintendo.
