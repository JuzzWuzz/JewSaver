﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MenuJewSaver:Menu
{
    Texture2D buttonTexture;
    Texture2D background;
    MenuButton newGame;
    MenuButton options;
    MenuButton exit;
    Sprite back;
    SpriteFont font;
    JewSaver jewSaver;

    public MenuJewSaver(JewSaver game)
        : base(game)
    {
        jewSaver = game;
    }

    public override void Initialize()
    {
        base.Initialize();
        exit = new MenuButton(buttonTexture, new Point(256, 64), new Point(0, 0), new Point(256, 64), new Point(384, 240), "EXIT");
        newGame = new MenuButton(buttonTexture, new Point(256, 64), new Point(0, 0), new Point(256, 64), new Point(384, 80), "NEW GAME");
        options = new MenuButton(buttonTexture, new Point(256, 64), new Point(0, 0), new Point(256, 64), new Point(384, 160), "OPTIONS");
        exit.buttonPressed += OnExitClicked;
        newGame.buttonPressed += OnNewGameClicked;
        options.buttonPressed += OnOptionsClicked;
        menuInputElements.Add(newGame);
        menuInputElements.Add(options);
        menuInputElements.Add(exit);
        newGame.font = font;
        options.font = font;
        exit.font = font;
        back = new Sprite(background, 1024, 384, 0, 0, 1024, 384, 0, 0);
        menuDecorElements.Add(back);
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        Color[] textureData = new Color[64 * 256];
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 256; j++)
            {
                textureData[i * 256 + j] = Color.White;
            }
        }
        buttonTexture = new Texture2D(Game.GraphicsDevice, 256, 64);
        buttonTexture.SetData<Color>(textureData);
        Color[] backgroundData = new Color[384 * 1024];
        Random random = new Random();
        for (int i = 0; i < 384; i++)
        {
            for (int j = 0; j < 1024; j++)
            {
                float sin =(float)Math.Abs(Math.Sin(j/1024.0f * 2 * Math.PI));
                float cos =(float)Math.Abs(Math.Cos(j/1024.0f * 2 * Math.PI));
                backgroundData[i * 1024 + j] = new Color(sin,0,0, 255);
            }
        }
        background = new Texture2D(Game.GraphicsDevice, 1024, 384);
        background.SetData<Color>(backgroundData);
        font = Game.Content.Load<SpriteFont>("ButtonText");
    }

    void OnExitClicked()
    {
        Game.Exit();
    }

    void OnNewGameClicked()
    {
        jewSaver.SwitchState(GameState.LEVEL_1);
    }

    void OnOptionsClicked()
    {
    }
}
