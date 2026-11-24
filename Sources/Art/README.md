# Arte e áudio originais — Ecos de Aurora

19 sprites e tiles de 32 × 32 pixels, desenhados como vetores editáveis alinhados
à grade e renderizados para PNG RGBA. Lia usa capa azul; Elara é a mentora; o
Sentinela tem armadura violeta e um núcleo de aurora. São personagens originais.

Os arquivos PNG usados pelo Unity estão em `Assets/Resources/Art/`. Os SVGs desta
pasta são as fontes editáveis. Os tiles `grass`, `path`, `water`, `wall` e `floor`
preenchem completamente a célula; os demais sprites têm fundo transparente.

Use **Sprite (2D and UI)**, **Single**, **32 Pixels Per Unit**, filtro **Point**,
compressão **None** e **Generate Mip Maps** desativado no importador do Unity.
Os sprites são poses únicas, próprias para o protótipo; movimento e impacto
podem ser mostrados por deslocamento, escala e troca breve de cor no código.

| Grupo | Arquivos |
| --- | --- |
| Personagens | `hero`, `slime`, `sentinel`, `elder` |
| Objetos | `chest`, `potion`, `crystal`, `shrine`, `switch_off`, `switch_on`, `gate` |
| Ambiente | `grass`, `path`, `water`, `wall`, `floor`, `tree` |
| Interface | `heart`, `coin` |

Cinco efeitos curtos, sintetizados sem amostras externas, ficam em
`Assets/Resources/Audio/`: `hit`, `pickup`, `door`, `win` e `step`. São WAV PCM
mono, 16 bits, 44.100 Hz. Importe como **Decompress On Load** para reprodução
rápida; ajuste o volume do AudioSource conforme o restante do jogo.

## Reproduzir

Na raiz do projeto:

```sh
python Sources/Art/generate_assets.py
```

Requer Python 3.10 ou superior, Pillow e um renderizador SVG: CairoSVG ou
ImageMagick (`magick`/`convert`). O script não acessa a rede. Os SVGs são
reescritos a partir das geometrias declaradas nele; para manter uma edição
manual no SVG, preserve uma cópia ou transfira a alteração para o script.

Ao terminar, o script verifica dimensões, canal alpha e formato dos efeitos.
Todos os arquivos aqui descritos seguem a licença MIT em `LICENSE.txt`.
