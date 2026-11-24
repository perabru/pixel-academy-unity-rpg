# Roteiro de aceitação no Unity e no Player

Execute com a Console limpa, em uma cópia de teste do projeto. Registre versão, sistema, resultado e evidências. Este roteiro não é registro de testes já executados.

| Caso | Procedimento | Resultado esperado |
|---|---|---|
| Primeira abertura | Preparar projeto, abrir Bootstrap, Play | Menu abre, nenhum erro vermelho |
| Movimento | Andar em linha reta e diagonal por 5 segundos | Diagonal não é mais rápida; sem atravessar árvores |
| Pausa | Esc, aguardar e continuar | Jogador, inimigos e projéteis param durante a pausa |
| Missão | Falar com Íris | Objetivo muda para três cristais |
| Cristais | Tentar coletar antes da missão | Mensagem orienta falar com Íris |
| Coleta | Pegar cristal, trocar de área e retornar | Item permanece coletado; contador não duplica |
| Baú | Abrir duas vezes e retornar ao mapa | Recompensa ocorre uma única vez |
| Espada | Atacar inimigo à frente, atrás e através da parede | Só atinge alvos no arco, alcance e sem obstáculo |
| Dano | Encostar em slime | Vida cai sem dano em todos os frames; breve invulnerabilidade |
| Poção | Usar com vida cheia, ferido e sem estoque | Não desperdiça com vida cheia; cura até3; não usa sem estoque |
| Chave | Voltar a Íris com2 e3 cristais | Chave exige3; recompensa não repete |
| Loja | Comprar com4 e5 moedas | Saldo insuficiente rejeitado; compra consome5 |
| Templo | Entrar sem e com chave | Entrada bloqueada sem chave |
| Runas | Lua primeiro; depois Sol, Lua, Estrela | Erro reinicia; sequência correta abre selo |
| Chefe | Entrar na arena, reduzir vida à metade | Perseguição e rajada aceleram na segunda fase |
| Derrota | Perder toda a vida | Retorna ao altar com objetivos preservados; perde até3 moedas |
| Vitória | Derrotar Sentinela e voltar a Íris | Final aparece; chefe não reaparece após carregar |
| Persistência | Salvar, fechar Player e continuar | Local, itens, missão e vida são restaurados |
| Recuperação | Em cópia de teste, corromper JSON principal | Cópia .bak é tentada; sem backup inicia estado válido |
| Resolução | Redimensionar janela entre1280×720 e1920×1080 | HUD e menus permanecem legíveis |
| Build | Gerar Windows e abrir fora do Editor | Jogo inicia com todos os arquivos da pasta distribuídos |

Os 12 testes NUnit de domínio estão em `Assets/Tests/EditMode`. Execute em **Window → General → Test Runner → EditMode → Run All**. Para testes sem motor: `dotnet run --project Validation/CoreChecks.csproj`.
