using Godot;
using System;

public class player : KinematicBody2D
{
    Vector2 UP = new Vector2(0, -1);
    const int GRAVITY = 20;
    const int MAXFALLSPEED = 200;
    const int MAXSPEED = 100;
    const int JUMPFORCE = 300;
    private enum states{
        IDLE,
        MOUVE,
        ATTAK
    }
    private states CurrentState = states.IDLE;
    const int ACCEL = 10;
    Vector2 vZero = new Vector2();

    bool facing_right = true;


    Vector2 motion = new Vector2();

    Sprite currentSprite;
    private AnimationPlayer animationPlayer = null;
    private AnimationTree animationtree = null;
    private AnimationNodeStateMachinePlayback animationState = null;
    
        // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentSprite = GetNode<Sprite>("Sprite");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationtree = GetNode<AnimationTree>("AnimationTree");
        animationState = (AnimationNodeStateMachinePlayback)animationtree.Get("parameters/playback");
    }

    public override void _PhysicsProcess(float delta)
    {
        motion.y += GRAVITY;

        if(motion.y > MAXFALLSPEED) {
            motion.y = MAXFALLSPEED;
        }

        if (facing_right) {
            currentSprite.FlipH = false;
        } else {
            currentSprite.FlipH = true;
        }

         motion.x = motion.Clamped(MAXSPEED).x;
        if(CurrentState!=states.ATTAK)
        {
            if (Input.IsActionPressed("ui_left")) {
                motion.x -= ACCEL;
                facing_right = false;
                if(IsOnFloor())
                {
                    CurrentState = states.MOUVE;
                    animationtree.Set("parameters/Mouve/blend_position",motion);
                }
                //animPlayer.Play("Run");
            } else if (Input.IsActionPressed("ui_right")) {
                motion.x += ACCEL;
                facing_right = true;
                if(IsOnFloor())
                {
                    CurrentState = states.MOUVE;
                    animationtree.Set("parameters/Mouve/blend_position",motion);
                }
                //animPlayer.Play("Run");
            } else {
                motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
                if(IsOnFloor())
                {
                    CurrentState = states.IDLE;
                    animationtree.Set("parameters/Iddle/blend_position",motion);
                }
                //animPlayer.Play("Idle");
            }
        }

        if (IsOnFloor())
        {
            // On ne regarde qu'un seul fois et non le maintient de la touche
            if (Input.IsActionJustPressed("ui_jump")) {
                motion.y = -JUMPFORCE;
                GD.Print($"motion.y = {motion.y}");
                Console.WriteLine($"motion.y = {motion.y}");
                CurrentState = states.MOUVE;
                animationtree.Set("parameters/Mouve/blend_position",motion);
            }
            if(Input.IsActionJustPressed("ui_attak"))
            {
                CurrentState = states.ATTAK;
            }
        }
        else
        {
            if(motion.y > 0)
            {
                CurrentState = states.MOUVE;
                animationtree.Set("parameters/Mouve/blend_position",motion);
            }
        }
        if (!IsOnFloor()) {
            if (motion.y < 0) {
                //animPlayer.Play("jump");
            } else if (motion.y > 0) {
                //animPlayer.Play("fall");
            }
        }
        motion = MoveAndSlide(motion, UP);
        switch (CurrentState)
        {
            case states.IDLE:
                animationState.Travel("Iddle");
                break;
            case states.MOUVE:
                animationState.Travel("Mouve");
                break;
            case states.ATTAK:
                animationState.Travel("Attak");
                motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
                break;
        }
    }

    public void AfterAttak()
    {
        CurrentState = states.IDLE;
    }
}
