﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

public class Stickfigure
{
    const float gravity = 500.0f;
    const float jumpForce = gravity * 30.0f;
    const double jumpAngle = Math.PI / 180.0f * 45.0f;
    
    private bool isPlayer = false;
    private bool newStickie;
    public bool inactive;

    public bool dead;
    public bool saved;
    public bool jumping;
    public bool sprinting;

    private Vector2 crotch, shoulder, lHand, rHand, lFoot, rFoot, neck, head;

    private int headSize;
    private int scale;
    private int thickness;

    public Vector2 position;
    private Vector2 origPosition;
    protected Vector2 velocity;
    protected float moveForce;
    protected float mass;
    protected float timer;
    protected float spawnTimer;
    protected Color color;
    protected Color colorHead;
    protected int curJumpIdx;
    protected int curSprintIdx;
    protected int stickieIndex;
    public bool isFemale;
    public bool isFattie;

    public Stickfigure(Vector2 position, int index)
    {
        origPosition = position;
        stickieIndex = index;
        Initialize();
    }

    public void SetIsPlayer()
    {
        isPlayer = true;
        color = Color.Brown;
        mass = 1.2f;
        thickness = (int)(scale * mass * mass * 0.75);
        isFemale = false;
    }

    public void Initialize()
    {
        shoulder = new Vector2(0, -10);
        lHand = rHand = new Vector2(5, 0);
        lFoot = rFoot = new Vector2(2, 7);

        this.scale = 2;
        headSize = 3;

        setLimbs(0.0f);

        this.isFemale = (!isPlayer && LevelBase.random.NextDouble() > 0.6);

        this.position = this.origPosition;
        this.velocity = Vector2.Zero;
        this.moveForce = 50.0f;
        this.mass = 1.0f + (1.0f - ((isFemale && !isPlayer) ? 0.5f : 0.0f)) * (float)LevelBase.random.NextDouble();
        this.timer = 0.0f;
        this.spawnTimer = 0.0f;
        this.curJumpIdx = 0;
        this.curSprintIdx = 0;
        this.inactive = true;
        this.dead = false;
        this.saved = false;
        this.jumping = true;
        this.sprinting = false;
        this.newStickie = true;
        this.color = (isFemale) ? Color.Pink : Color.Yellow;
        this.colorHead = Color.Orange;

        this.thickness = (int)(scale * mass * mass * 0.75);

        if (isPlayer)
        {
            mass = 1.2f;
            thickness = (int)(scale * mass * mass * 0.75);
        }

        this.isFattie = (mass > 1.5f);
    }

    // Update Method
    public void update(float dt)
    {
        if ((dead || saved) && inactive)
            return;

        // If a new stickie make invulnarable when starting and spawn one after the other
        if (newStickie)
        {
            spawnTimer += dt;
            dead = false;
            // If stickie is active then allow spawning
            if (!inactive && spawnTimer > 0.75f * stickieIndex)
            {
                spawnTimer = 0.0f;
                newStickie = false;
            }
        }

        // Save stickes that make it to the end
        if (position.X > LevelBase.levelLength - LevelBase.scrollX + 5 * scale)
        {
            saved = true;
            inactive = true;
            return;
        }

        // If not moving or dead then do this until stickie has moved off of screen
        if (dead)
        {
            setLimbs(dt);
            if (position.X < -5 * scale)
                inactive = true;
            return;
        }

        // Set the initial force value
        Vector2 force = new Vector2(0.0f, gravity);

        // Get the ground value at stickies x
        float ground;
        float drag = 0.99f;

        float gp = JewSaver.height - LevelBase.heightMap[Math.Min(Math.Max(0, (int)(LevelBase.scrollX + position.X - scale)), LevelBase.levelLength - 1)];
        float gn = JewSaver.height - LevelBase.heightMap[Math.Min(Math.Max(0, (int)(LevelBase.scrollX + position.X + scale)), LevelBase.levelLength - 1)];
        double angle = Math.Atan((double)Math.Abs(gn - gp) / (double)(scale * 2));
        ground = JewSaver.height - LevelBase.heightMap[Math.Min(Math.Max(0, (int)(LevelBase.scrollX + position.X)), LevelBase.levelLength - 1)];

        setLimbs(dt);
        if (!jumping)
        {
            if (gn > gp)
            {
                // Gradient = \
                //ground = JewSaver.height - heightmap[(int)LevelBase.scrollX + (int)rFoot.X];
                angle -= Math.PI / 36;
            }
            else
            {
                // Gradient = /
                //ground = JewSaver.height - heightmap[(int)LevelBase.scrollX + (int)lFoot.X];
                if (!isPlayer && angle > Math.PI / 180.0f * 80.0f)
                {
                    drag = 0.0f;
                    // Add dt to the timer value and if they are stuck for 4 seconds then kill them
                    timer += dt;
                    if (timer >= 4.0f)
                        dead = true;
                }
                else
                {
                    // Check to see if they climbing more than 22.5 degrees then start to apply
                    // the drag value based on that up to 67.5 degrees
                    float newAngle = Math.Abs((float)angle - (float)Math.PI / 180.0f * 20.0f);
                    drag -= 0.03f * newAngle / ((float)Math.PI / 180.0f * 60.0f);
                    // Minus dt from timer to reduce death chance
                    timer -= dt;
                }
            }
        }

        // Basic collision response thingy
        if (lowestPoint().Y > ground)
        {
            force = -velocity * mass / dt;
            // If the impact force is higher than this value then kill stickie
            if (!isPlayer && Math.Abs((force.Y / gravity)) >= 55.0f && !newStickie)
                dead = true;

            force = Vector2.Zero;

            // First inpact if new stickie so make them active now
            if (newStickie)
                inactive = false;

            LevelBase.TerrainType terrType = LevelBase.canSculpt[Math.Min(Math.Max(0, (int)(LevelBase.scrollX + position.X)), LevelBase.levelLength - 1)];
            if (terrType == LevelBase.TerrainType.CANYON || terrType == LevelBase.TerrainType.ROCK)
            {
                dead = true;
            }
            else if (terrType == LevelBase.TerrainType.WATER)
            {
                if (!isPlayer)
                    dead = true;
            }
        }
        // Factor in walking if not dead
        if (!dead && !newStickie && !jumping)
        {
            Vector2 newForce = new Vector2(moveForce * (float)Math.Cos(angle), moveForce * (float)Math.Sin(angle));
            if (sprinting)
                newForce *= 2.0f;

            force += newForce;
        }


        // If not jumping already then lookup to see if a jump location has been passed
        if (!jumping)
        {
            for (int i = curJumpIdx; i < LevelBase.jumpMarkers.Count; i++)
            {
                if (position.X > LevelBase.moses.position.X)
                    curJumpIdx++;
                else if (position.X + LevelBase.scrollX >= LevelBase.jumpMarkers[i])
                {
                    force = new Vector2(jumpForce * (float)Math.Cos(jumpAngle), -jumpForce * (float)Math.Sin(jumpAngle));
                    if (sprinting)
                        force *= 1.25f;
                    jumping = true;
                    curJumpIdx++;
                    timer = 0.0f;
                }
            }
        }

        // Do checks for sprint markers and activate sprinting
        for (int i = curSprintIdx; i < LevelBase.sprintMarkers.Count; i++)
        {
            if (position.X > LevelBase.moses.position.X)
                curSprintIdx++;
            else if (position.X + LevelBase.scrollX >= LevelBase.sprintMarkers[i])
            {
                sprinting ^= true;
                curSprintIdx++;
            }
        }

        // Calculate the acceleration value
        Vector2 accel = force / mass;
        // Add the acceleration to velocity and factor in dt
        velocity += accel * dt;
        // Apply drag 
        velocity *= drag;
        // Update the position
        position += velocity * dt;

        // Keep stickie above ground
        if (jumping)
        {
            timer += dt;
        }
        if (!jumping || (jumping && timer > 0.1f))
        {
            float diff = lowestPoint().Y - ground;
            if (diff > 0)
            {
                if (jumping)
                {
                    velocity *= 0.1f;
                }
                jumping = false;
                position.Y -= diff;
            }
        }

        if (dead)
            position.Y = ground;

        setLimbs(dt);
    }

    private double change = 0;
    private void setLimbs(float dt)
    {
        change += dt;

        crotch = position;
        rFoot = crotch + new Vector2(-2.5f * scale, 7 * scale);
        lFoot = crotch + new Vector2(2.5f * scale, 7 * scale);

        if (!dead)
        {
            rFoot += Vector2.Multiply(new Vector2(2.5f * scale, 0), (float)Math.Cos(change));
            lFoot += Vector2.Multiply(new Vector2(-2.5f * scale, 0), (float)Math.Cos(change));
            crotch -= Vector2.Multiply(new Vector2(0, 1.5f * scale), (float)Math.Cos(change) + 1);
        }

        shoulder = crotch + new Vector2(0, -8 * scale);
        // Make females smaller
        if (isFemale)
            shoulder.Y += 6;
        if (!dead)
            shoulder += Vector2.Multiply(new Vector2(4, 0), (float)Math.Cos(change * 2));
        neck = shoulder + new Vector2(0, -2 * scale);
        head = neck + new Vector2(0, -headSize * scale);

        lHand = shoulder + new Vector2(5 * scale, 0);
        rHand = shoulder + new Vector2(-5 * scale, 0);

        // If dead move all body parts to ground
        if (dead)
        {
            crotch.Y = position.Y;
            shoulder.Y = position.Y;
            neck.Y = position.Y;
            head.Y = position.Y;
            lHand.Y = position.Y;
            rHand.Y = position.Y;
            lFoot.Y = position.Y;
            rFoot.Y = position.Y;

            // Make them a pile of blood
            color = Color.Red;
            colorHead = Color.Yellow;
        }
    }

    private int vecComp(Vector2 x, Vector2 y)
    {
        return (-x.Y.CompareTo(y.Y));
    }

    public Vector2 lowestPoint()
    {
        List<Vector2> l = new List<Vector2>();
        l.Add(crotch);
        l.Add(lFoot);
        l.Add(rFoot);
        l.Sort(vecComp);
        return (l[0]);
    }

    public void draw()
    {
        if ((dead || saved) && inactive)
            return;

        // Draw the head
        JewSaver.primitiveBatch.DrawCircle(head, colorHead, headSize * scale);

        // Begin primitive batch
        JewSaver.primitiveBatch.Begin(PrimitiveType.LineList);

        // Draw the main body
        JewSaver.primitiveBatch.AddLine(crotch, neck, color, color, thickness);

        // Draw the arms
        JewSaver.primitiveBatch.AddLine(shoulder, lHand, color, color, thickness);
        JewSaver.primitiveBatch.AddLine(shoulder, rHand, color, color, thickness);

        // Draw the feet
        JewSaver.primitiveBatch.AddLine(crotch, lFoot, color, color, thickness);
        JewSaver.primitiveBatch.AddLine(crotch, rFoot, color, color, thickness);

        if (isPlayer)
            JewSaver.primitiveBatch.AddLine(crotch + new Vector2(8, 14),
                shoulder + new Vector2(8, -10), Color.Black, 2);

        // End primitive batch
        JewSaver.primitiveBatch.End();
    }
}
