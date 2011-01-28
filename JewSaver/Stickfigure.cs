﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Stickfigure
{
    private Vector2 position;
    private bool moving = false;

    private Vector2 crotch, shoulder, lHand, rHand, lFoot, rFoot, neck, head;
    private float roShoulder, roLHand, roRHand, roLFoot, roRFoot;

    private int headSize;
    private int scale;

    Vector2[] points;

    protected Vector2 velocity;
    protected float mass;

    public Stickfigure(Vector2 position)
    {
        this.position = position;
        roShoulder = roLHand = roRHand = roLFoot = roRFoot = 0f;

        shoulder = new Vector2(0, -10);
        lHand = rHand = new Vector2(5, 0);
        lFoot = rFoot = new Vector2(2, 7);

        this.scale = 3;
        headSize = 3;

        setLimbs();

        this.velocity = Vector2.Zero;
        this.mass = 1.0f;
        moving = true;
    }

    // Update Method
    public void update(float dt)
    {
        if (!moving)
            return;

        Vector2 accel = new Vector2(0.0f, -100.0f) / mass;
        if (position.Y < JewSaver.height / 2.0f)
            accel *= -1f;
        velocity += accel * dt;

        velocity *= 0.99f;

        position += velocity * dt;

        setLimbs();
    }

    private void setLimbs()
    {
        crotch = position;
        shoulder = crotch + new Vector2(0, -10 * scale);
        neck = crotch + new Vector2(0, -12 * scale);
        head = neck + new Vector2(0, -headSize * scale);
        lHand = shoulder + new Vector2(4 * scale, 0);
        rHand= shoulder + new Vector2(-4 * scale, 0);
        rFoot = crotch + new Vector2(-3 * scale, 6 * scale);
        lFoot = crotch + new Vector2(3 * scale, 6 * scale);

    }

    public void draw()
    {
        JewSaver.primitiveBatch.DrawCircle(head, Color.Blue, headSize * scale);
        
        JewSaver.primitiveBatch.Begin(PrimitiveType.LineList);

        JewSaver.primitiveBatch.AddLine(crotch, neck, Color.Red, Color.Yellow, scale);

        JewSaver.primitiveBatch.AddLine(crotch, lFoot, Color.Yellow, Color.Yellow, scale);
        JewSaver.primitiveBatch.AddLine(crotch, rFoot, Color.Yellow, Color.Yellow, scale);

        JewSaver.primitiveBatch.AddLine(shoulder, lHand, Color.Yellow, Color.Yellow, scale);
        JewSaver.primitiveBatch.AddLine(shoulder, rHand, Color.Yellow, Color.Yellow, scale);
        /*
        JewSaver.primitiveBatch.AddVertex(crotch, Color.Red);
        JewSaver.primitiveBatch.AddVertex(shoulder, Color.Yellow);

        JewSaver.primitiveBatch.AddVertex(crotch, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(lFoot, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(crotch, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(rFoot, Color.Yellow);

        JewSaver.primitiveBatch.AddVertex(shoulder, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(lHand, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(shoulder, Color.Yellow);
        JewSaver.primitiveBatch.AddVertex(rHand, Color.Yellow);
         */

        JewSaver.primitiveBatch.End();
    }
}
