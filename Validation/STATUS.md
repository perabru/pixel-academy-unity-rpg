# Estado da validação — 24/11/2026

Executado:

- 28 verificações do domínio em .NET 8: poções, dano, coleta idempotente, chave, loja, limites de inventário, integridade de dados e versão de salvamento.
- Compilação estática de todos os scripts de runtime e do editor com Roslyn e assemblies reais do Unity 6000.3.29f1, incluindo UGUI: zero erros e zero avisos.
- Inspeção de 19 PNGs RGBA 32×32 e 5 WAVs mono PCM 16 bits 44.100Hz.
- Revisão estática do fluxo da missão e dos pontos de salvamento.

Pendente:

- Importação do projeto e execução dos 12 testes EditMode dentro do Unity.
- Teste de física, UI, áudio, cena e partida completa no Editor.
- Build Windows e teste do executável em Windows.

O Unity Editor 6000.3.29f1 foi instalado, mas sua inicialização em lote terminou com código 199: o serviço Unity Licensing Client não pôde ser conectado. Não houve importação nem geração de um player. A compilação estática acima valida sintaxe e referências; não substitui a importação pelo Unity, o teste de gameplay ou o build nativo.

Os scripts de build abortam se não houver executável real. Ative uma licença válida pelo Unity Hub, instale o módulo Windows Build Support e execute o roteiro de aceitação antes de distribuir uma versão final.
