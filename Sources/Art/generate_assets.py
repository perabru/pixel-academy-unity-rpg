#!/usr/bin/env python3
"""Rebuild original Ecos de Aurora pixel art and sound effects.

Python 3.10+, ImageMagick `convert` (or CairoSVG), and Pillow for validation.
All graphics are authored as editable SVG geometry, then rendered to PNG.
No external images, fonts, recordings, or network access are used.
"""
from pathlib import Path
import json
import math
import random
import shutil
import struct
import subprocess
import wave

ROOT = Path(__file__).resolve().parents[2]
SVG_DIR = ROOT / "Sources/Art"
PNG_DIR = ROOT / "Assets/Resources/Art"
AUDIO_DIR = ROOT / "Assets/Resources/Audio"
INK = "#202b3d"


class Art:
    def __init__(self, name, title):
        self.name, self.title, self.shapes = name, title, []

    def rect(self, x, y, w, h, fill):
        self.shapes.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{fill}"/>')
        return self

    def poly(self, coords, fill):
        points = " ".join(f"{x},{y}" for x, y in coords)
        self.shapes.append(f'<polygon points="{points}" fill="{fill}"/>')
        return self

    def save(self):
        text = '\n'.join([
            '<?xml version="1.0" encoding="UTF-8"?>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32" shape-rendering="crispEdges">',
            f'<title>{self.title} — Ecos de Aurora</title>',
            '<desc>Original pixel art. Copyright 2026 Pixel Academy. MIT License; see LICENSE.txt.</desc>',
            *self.shapes, '</svg>', ''])
        svg = SVG_DIR / f"{self.name}.svg"
        svg.write_text(text, encoding="utf-8")
        png = PNG_DIR / f"{self.name}.png"
        try:
            import cairosvg
        except ImportError:
            convert = shutil.which("magick") or shutil.which("convert")
            if not convert:
                raise RuntimeError("Install CairoSVG (pip install cairosvg) or ImageMagick to render the SVG sources.")
            subprocess.run([convert, "-background", "none", str(svg), "-define", "png:color-type=6", str(png)], check=True)
        else:
            cairosvg.svg2png(url=str(svg), write_to=str(png), output_width=32, output_height=32)
        from PIL import Image
        with Image.open(png) as rendered:
            rgba=rendered.convert('RGBA')
        rgba.save(png)
        return self.name


def sprites():
    out = []
    a = Art("hero", "Lia, exploradora de capa azul")
    a.rect(8,29,17,2,"#26334755")
    a.rect(10,25,5,5,INK).rect(17,25,5,5,INK)
    a.rect(10,27,4,2,"#735049").rect(18,27,4,2,"#9d6b4e")
    a.poly([(9,14),(23,14),(26,26),(6,26)],INK)
    a.poly([(10,15),(22,15),(24,25),(8,25)],"#267cb0")
    a.poly([(10,15),(14,16),(12,25),(8,25)],"#4fb6d0")
    a.poly([(20,15),(22,15),(24,25),(20,25)],"#205079")
    a.rect(11,17,10,7,"#e6c083").rect(12,17,7,6,"#ffdfa0")
    a.rect(11,23,10,2,"#815a47").rect(15,23,3,2,"#f1d75c")
    a.rect(8,17,3,6,"#398ba8").rect(21,17,3,6,"#28618a")
    a.rect(8,22,3,3,"#e7a46c").rect(21,22,3,3,"#f5bd80")
    a.poly([(12,2),(20,2),(20,3),(23,3),(23,6),(25,6),(25,13),(23,13),(23,16),(10,16),(10,13),(8,13),(8,6),(10,6),(10,3),(12,3)],INK)
    a.rect(11,4,11,11,"#885641").rect(9,7,3,6,"#62413b")
    a.rect(12,6,10,9,"#f1b77c").rect(14,7,8,7,"#ffd49a")
    a.rect(11,4,10,3,"#a86741").rect(11,6,4,3,"#885641")
    a.rect(15,6,3,2,"#885641").rect(21,6,2,7,"#67453b")
    a.rect(13,10,2,2,INK).rect(19,10,2,2,INK).rect(14,10,1,1,"#ffffff")
    a.rect(16,13,3,1,"#b86851").rect(9,6,2,2,"#6ad7e0")
    a.rect(11,15,11,2,"#357d9d").rect(15,16,4,2,"#5bc4d3")
    out.append(a)

    a=Art("slime","Gota selvagem")
    a.rect(6,27,21,2,"#233c4c55")
    a.poly([(10,10),(22,10),(22,12),(25,12),(25,15),(27,15),(27,24),(25,24),(25,27),(7,27),(7,25),(5,25),(5,17),(7,17),(7,13),(10,13)],INK)
    a.poly([(10,12),(22,12),(22,14),(24,14),(24,17),(25,17),(25,24),(23,24),(23,25),(8,25),(8,23),(7,23),(7,17),(9,17),(9,14),(10,14)],"#32b892")
    a.rect(10,13,10,3,"#75e4b9").rect(9,16,5,2,"#75e4b9")
    a.rect(8,23,16,2,"#19866d").rect(12,19,2,3,INK).rect(20,19,2,3,INK)
    a.rect(15,23,5,1,"#d2ffe0").rect(9,20,2,1,"#73e4bc")
    out.append(a)

    a=Art("sentinel","Sentinela violeta")
    a.rect(5,29,23,2,"#23344766")
    a.rect(7,25,7,5,INK).rect(19,25,7,5,INK)
    a.poly([(10,5),(22,5),(22,8),(27,8),(27,13),(30,13),(30,24),(25,24),(25,28),(7,28),(7,24),(2,24),(2,13),(5,13),(5,8),(10,8)],INK)
    a.rect(7,9,18,17,"#6f4aa2").rect(9,10,14,15,"#9471cc")
    a.rect(3,14,6,9,"#573779").rect(24,14,5,9,"#573779")
    a.rect(5,12,5,4,"#ba97ef").rect(22,12,5,4,"#aa85e5")
    a.poly([(8,4),(12,7),(16,2),(20,7),(24,4),(22,12),(10,12)],"#ecd279")
    a.rect(10,11,12,5,"#3c315d").rect(11,12,3,2,"#77f3f4").rect(18,12,3,2,"#77f3f4")
    a.poly([(16,17),(20,21),(16,25),(12,21)],"#342746")
    a.poly([(16,18),(19,21),(16,24),(13,21)],"#7be8e9")
    a.poly([(16,18),(19,21),(16,21)],"#ecfff7")
    a.rect(7,27,6,2,"#8c67bf").rect(20,27,6,2,"#8c67bf")
    out.append(a)

    a=Art("elder","Mestra Elara")
    a.rect(7,29,19,2,"#24344955").rect(25,7,2,23,INK).rect(25,9,1,20,"#b58958")
    a.rect(23,4,5,5,INK).rect(24,5,3,3,"#76ddc8")
    a.poly([(10,13),(20,13),(24,29),(6,29)],INK)
    a.poly([(11,15),(19,15),(22,28),(8,28)],"#64547a")
    a.poly([(12,15),(15,17),(14,28),(9,28)],"#9c88ad")
    a.rect(7,18,4,7,"#78628c").rect(20,18,4,7,"#78628c")
    a.rect(7,23,3,3,"#dfaa7c").rect(21,22,4,3,"#efc28c")
    a.rect(10,3,10,2,INK).rect(8,5,14,10,INK)
    a.rect(10,5,10,9,"#dca778").rect(12,5,8,8,"#f0c799")
    a.rect(10,4,10,3,"#edf1e5").rect(9,7,3,6,"#a7b7bb").rect(20,7,2,6,"#dce9dc")
    a.rect(12,9,2,2,INK).rect(18,9,2,2,INK)
    a.poly([(11,12),(14,13),(17,12),(20,12),(19,16),(16,19),(12,16)],"#dce9dc")
    a.rect(14,14,4,2,"#ffffff").rect(14,20,4,3,"#e7cc7a")
    out.append(a)

    a=Art("chest","Baú do vale")
    a.rect(3,26,26,3,"#26344755").rect(3,11,26,16,INK).rect(5,8,22,5,INK)
    a.rect(5,12,22,13,"#7d4a35").rect(6,10,20,8,"#b87945")
    a.rect(7,10,18,2,"#e5a763").rect(5,18,22,2,INK)
    a.rect(7,10,3,15,"#e0b85d").rect(22,10,3,15,"#e0b85d")
    a.rect(8,10,1,15,"#fff0a4").rect(23,10,1,15,"#f6db87")
    a.rect(13,16,6,7,INK).rect(14,17,4,5,"#f3d477").rect(15,19,2,2,"#754b37")
    out.append(a)

    a=Art("potion","Poção rubi")
    a.rect(10,28,13,2,"#26344755").rect(12,3,8,5,INK).rect(13,4,6,3,"#aa7a4e")
    a.rect(11,8,10,6,INK)
    a.poly([(11,12),(21,12),(25,17),(25,25),(22,28),(10,28),(7,25),(7,17)],INK)
    a.poly([(12,13),(20,13),(23,17),(23,24),(21,26),(11,26),(9,24),(9,17)],"#c6f0e4")
    a.rect(13,8,6,5,"#c6f0e4").rect(14,8,3,6,"#f4fff0")
    a.poly([(10,18),(22,18),(22,24),(20,25),(12,25),(10,23)],"#e75c76")
    a.rect(12,19,3,3,"#ffa2ac").rect(17,23,4,2,"#b94366").rect(10,15,2,3,"#faffed")
    out.append(a)

    a=Art("crystal","Fragmento de aurora")
    a.rect(9,29,14,1,"#26344744")
    a.poly([(16,2),(25,12),(23,22),(16,29),(9,22),(7,12)],INK)
    a.poly([(16,4),(23,12),(21,21),(16,27),(11,21),(9,12)],"#2bbccf")
    a.poly([(16,4),(16,15),(9,12)],"#c3fbeb")
    a.poly([(16,4),(23,12),(16,15)],"#70e9e0")
    a.poly([(9,12),(16,15),(16,27),(11,21)],"#1b8cb4")
    a.poly([(23,12),(21,21),(16,27),(16,15)],"#43cdd7")
    a.rect(3,7,1,5,"#cdf9ee").rect(1,9,5,1,"#cdf9ee")
    out.append(a)

    a=Art("shrine","Santuário da aurora")
    a.rect(3,27,26,4,INK).rect(5,24,22,4,"#566d7d").rect(7,21,18,4,"#899c9c")
    a.rect(10,11,12,11,INK).rect(12,12,8,9,"#718b91").rect(12,12,2,9,"#b9c9bb")
    a.rect(8,10,16,3,INK).rect(9,10,14,2,"#d9d2a0")
    a.poly([(16,1),(22,6),(20,10),(12,10),(10,6)],INK)
    a.poly([(16,2),(20,6),(16,10),(12,6)],"#64e7da")
    a.poly([(16,2),(16,7),(12,6)],"#e3ffe4")
    a.rect(14,15,4,4,"#243e57").rect(15,15,2,3,"#f1d77e")
    a.rect(4,28,24,1,"#b0bcad").rect(8,22,16,1,"#d0d6bb")
    out.append(a)

    for enabled in (False, True):
        a=Art("switch_on" if enabled else "switch_off","Interruptor ativado" if enabled else "Interruptor em repouso")
        a.rect(5,8,22,20,INK).rect(7,6,18,3,INK).rect(6,9,20,17,"#697f89").rect(8,7,16,3,"#a7b8b2")
        a.rect(9,12,14,11,"#263e50").rect(11,10,10,2,"#263e50")
        a.rect(11,13 if enabled else 11,10,8,"#64cfc0" if enabled else "#a2775a")
        a.rect(12,13 if enabled else 11,8,2,"#d4ffe1" if enabled else "#e3b079")
        a.rect(14,16 if enabled else 14,4,3,"#245b63" if enabled else "#614841")
        if enabled:a.rect(3,3,2,3,"#a5fbdc").rect(27,3,2,3,"#a5fbdc")
        out.append(a)

    a=Art("gate","Portão do santuário")
    a.rect(1,4,30,27,INK).rect(5,1,22,5,INK)
    a.rect(2,6,5,24,"#75898c").rect(25,6,5,24,"#75898c").rect(6,2,20,5,"#a1b2a4")
    a.rect(7,7,18,23,"#503e3d").rect(8,8,16,21,"#8d5b40")
    for x in (9,14,19):a.rect(x,8,1,21,"#bf8c52").rect(x+3,8,1,21,"#694737")
    a.rect(7,12,18,3,"#c3a85c").rect(7,24,18,3,"#c3a85c")
    a.rect(14,17,5,6,INK).rect(15,18,3,4,"#efd284").rect(16,19,1,2,"#6a513e")
    for y in (11,19,27):a.rect(2,y,5,1,"#33465a").rect(25,y,5,1,"#33465a")
    a.rect(3,6,1,23,"#bdc5ae").rect(26,6,1,23,"#bdc5ae")
    out.append(a)

    a=Art("grass","Relva do vale").rect(0,0,32,32,"#477b55")
    for x,y in [(2,4),(13,1),(23,7),(6,18),(19,23),(28,28),(3,29),(26,16)]:
        a.rect(x,y,2,2,"#66a16a").rect(x+2,y-1,1,3,"#66a16a").rect(x+4,y+1,2,1,"#38634d")
    a.rect(16,12,2,1,"#96b579").rect(10,27,1,1,"#a7c182")
    out.append(a)

    a=Art("path","Caminho de terra").rect(0,0,32,32,"#bb9866")
    for x,y,w in [(1,5,4),(12,2,3),(22,7,5),(5,15,3),(18,19,4),(2,26,5),(26,28,3)]:
        a.rect(x,y,w,1,"#d3b67e").rect(x+1,y+2,2,1,"#a77e55")
    a.rect(13,27,3,2,"#9e805e").rect(13,27,2,1,"#e1c48f")
    out.append(a)

    a=Art("water","Água azul").rect(0,0,32,32,"#2d7194")
    for x,y,w in [(1,3,9),(19,8,11),(5,15,12),(23,22,8),(0,29,13)]:
        a.rect(x,y,w,1,"#63b4c5").rect(x+2,y+1,max(w-4,1),1,"#3d92af")
    a.rect(13,6,4,1,"#255d85").rect(3,23,8,1,"#255d85").rect(16,26,3,1,"#b5e8d7")
    out.append(a)

    a=Art("wall","Parede antiga").rect(0,0,32,32,"#354657")
    for y in range(0,32,8):
        offset=0 if y%16==0 else -8
        for x in range(offset,32,16):
            a.rect(x+1,y+1,14,6,"#718489").rect(x+1,y+1,14,1,"#a5b3a8").rect(x+1,y+6,14,1,"#526777")
    a.rect(3,23,5,2,"#71946a").rect(23,6,4,2,"#71946a")
    out.append(a)

    a=Art("floor","Piso do santuário").rect(0,0,32,32,"#536877")
    for x,y in ((1,1),(17,1),(1,17),(17,17)):
        a.rect(x,y,14,14,"#84989b").rect(x,y,14,1,"#b1bfb1").rect(x+13,y,1,14,"#71878e")
    a.rect(8,7,4,1,"#72858c").rect(10,8,1,4,"#72858c").rect(22,23,3,1,"#bfd0bc")
    out.append(a)

    a=Art("tree","Árvore de copa verde")
    a.rect(5,28,23,3,"#243a4255").rect(13,19,7,11,INK).rect(14,20,5,9,"#80583e").rect(14,22,2,7,"#b48750")
    a.poly([(10,1),(23,1),(23,3),(27,3),(27,7),(30,7),(30,17),(27,17),(27,21),(22,21),(22,23),(9,23),(9,21),(4,21),(4,17),(2,17),(2,8),(5,8),(5,4),(10,4)],INK)
    a.poly([(10,3),(22,3),(22,5),(25,5),(25,9),(28,9),(28,16),(25,16),(25,19),(21,19),(21,21),(10,21),(10,19),(6,19),(6,15),(4,15),(4,9),(7,9),(7,6),(10,6)],"#358363")
    a.poly([(10,4),(21,4),(21,7),(24,7),(24,11),(20,11),(20,13),(7,13),(7,9),(9,9),(9,6),(10,6)],"#69ad70")
    a.rect(11,5,8,2,"#a2ce82").rect(9,8,5,2,"#82bf78")
    a.rect(10,17,9,3,"#286348").rect(21,15,4,2,"#4e9b65")
    a.rect(8,14,3,2,"#62a76c").rect(24,10,2,2,"#8ac379")
    out.append(a)

    a=Art("heart","Coração de energia")
    a.poly([(4,6),(12,6),(16,10),(20,6),(28,6),(31,9),(31,17),(16,31),(1,17),(1,9)],INK)
    a.poly([(5,8),(11,8),(16,13),(21,8),(27,8),(29,10),(29,16),(16,28),(3,16),(3,10)],"#e86578")
    a.poly([(3,13),(8,19),(16,25),(24,19),(29,13),(29,16),(16,28),(3,16)],"#b63d62")
    a.rect(6,10,5,2,"#ffb2ad").rect(5,12,2,4,"#ffb2ad")
    out.append(a)

    a=Art("coin","Moeda de aurora")
    a.poly([(11,2),(22,2),(27,7),(29,12),(29,21),(25,27),(21,30),(10,30),(5,25),(3,20),(3,11),(6,6)],INK)
    a.poly([(11,4),(21,4),(25,8),(27,12),(27,21),(23,26),(20,28),(11,28),(7,24),(5,20),(5,12),(8,7)],"#d8a543")
    a.poly([(11,6),(20,6),(24,10),(25,14),(25,20),(21,25),(12,25),(8,21),(7,16),(9,10)],"#f3d273")
    a.rect(10,9,3,3,"#fff2ae").rect(8,12,2,5,"#fff2ae")
    a.poly([(16,10),(20,16),(16,22),(12,16)],"#b57a38")
    a.poly([(16,11),(18,16),(16,19),(14,16)],"#ffe69a")
    out.append(a)
    return out


def effect(name, duration, fn, gain=.45):
    rate=44100
    samples=[]
    for i in range(int(duration*rate)):
        t=i/rate
        attack=min(1,t/.005)
        release=min(1,(duration-t)/.015)
        value=max(-1,min(1,fn(t)*gain*attack*release))
        samples.append(struct.pack('<h',round(value*32767)))
    with wave.open(str(AUDIO_DIR/f"{name}.wav"),'wb') as out:
        out.setparams((1,2,rate,0,'NONE','not compressed'))
        out.writeframes(b''.join(samples))


def audio():
    rng=random.Random(1706)
    effect('hit',.17,lambda t:(math.sin(2*math.pi*(145*t-240*t*t))*.7+rng.uniform(-1,1)*.3)*math.exp(-17*t))
    effect('pickup',.30,lambda t:math.sin(2*math.pi*(660 if t<.12 else 990)*t)*math.exp(-5*t))
    effect('door',.42,lambda t:(math.sin(2*math.pi*65*t)*.6+math.sin(2*math.pi*93*t)*.2+rng.uniform(-1,1)*.2)*math.sin(math.pi*t/.42))
    notes=[523.25,659.25,783.99,1046.50]
    def victory(t):
        n=min(int(t/.16),3)
        local=t-n*.16
        envelope=min(1,local/.006)*math.exp(-4*local)
        if n<3:envelope*=min(1,(.16-local)/.012)
        return (math.sin(2*math.pi*notes[n]*local)+.2*math.sin(4*math.pi*notes[n]*local))*envelope
    effect('win',.85,victory,.30)
    effect('step',.08,lambda t:rng.uniform(-1,1)*math.exp(-45*t),.22)


def validate(names):
    from PIL import Image
    details=[]
    for name in names:
        path=PNG_DIR/f"{name}.png"
        with Image.open(path) as im:
            assert im.size==(32,32),(name,im.size)
            assert im.mode=='RGBA',(name,im.mode)
            alpha=im.getchannel('A')
            assert alpha.getextrema()[1]==255,name
            if name not in {'grass','path','water','wall','floor'}:
                assert alpha.getextrema()[0]==0,name
            details.append({'name':name,'size':[32,32],'mode':im.mode,'alpha':list(alpha.getextrema())})
    for name in ['hit','pickup','door','win','step']:
        with wave.open(str(AUDIO_DIR/f'{name}.wav')) as f:
            assert f.getnchannels()==1 and f.getsampwidth()==2 and f.getframerate()==44100,name
    print(json.dumps({'graphics':details,'audio':5,'validation':'passed'},ensure_ascii=False,indent=2))


if __name__=='__main__':
    for folder in (SVG_DIR,PNG_DIR,AUDIO_DIR):folder.mkdir(parents=True,exist_ok=True)
    names=[art.save() for art in sprites()]
    audio()
    validate(names)
