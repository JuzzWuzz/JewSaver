﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LevelBase:DrawableGameComponent
{
    protected enum LevelMode {EDIT, PLAY};
    Texture2D background;
    float[] heightMap;

    /// <summary>
    /// Base constructor for a level
    /// </summary>
    /// <param name="game">pointer to main game</param>
    /// <param name="levelLength">level length in pixels</param>
    public LevelBase(JewSaver game, int levelLength)
        : base(game)
    {
        heightMap = new float [levelLength];
        for (int i = 0; i < levelLength; i++)
        {
            float sin = (float)(Math.Abs(384 * Math.Sin(i/(float)levelLength) * 2 * Math.PI));
            heightMap[i] = sin;
            //Console.WriteLine(sin);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        background = new Texture2D(Game.GraphicsDevice, heightMap.Length, 384);
        Color[] data = new Color[384 * heightMap.Length];
        for (int i = 0; i < 384; i++)
        {
            for (int j = 0; j < heightMap.Length; j++)
            {
                data[i * heightMap.Length + j] = i < heightMap[j] ? Color.Yellow : Color.PowderBlue;
            }
        }
        background.SetData<Color>(data);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        JewSaver.spriteBatch.Begin();
        JewSaver.spriteBatch.Draw(background, new Vector2(0, 384), new Rectangle(0, 0, 1024, 384), Color.White);
        JewSaver.spriteBatch.End();
    }
}
