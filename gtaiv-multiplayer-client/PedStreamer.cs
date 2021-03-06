// Copyright 2014 Adrian Chlubek. This file is part of GTA Multiplayer IV project.
// Use of this source code is governed by a MIT license that can be
// found in the LICENSE file.
using GTA;
using System;
using System.Linq;

//using MIVSDK;

namespace MIVClient
{
    public class PedStreamer : StreamerBase
    {
        private System.Drawing.Color nickcolor, chatcolor;
        private GTA.Font nickfont;
        public float DrawDistance;
        public bool checkLOS;

        public PedStreamer(Client client, float range)
            : base(client, range)
        {
            nickfont = new Font("Segoe UI", 24, FontScaling.Pixel, false, false);
            nickfont.Effect = FontEffect.None;
            nickcolor = System.Drawing.Color.LightYellow;
            chatcolor = System.Drawing.Color.White;
            DrawDistance = 120.0f;
            checkLOS = true;
            //fa_font = new Font("FotntAwesome", 24, FontScaling.Pixel, false, false);
        }

        public override void UpdateGfx()
        {
            if (!client.getPlayerPed().Exists())
            {
                return;
            }
            var player_projected = (Vector2)World.WorldToScreenProject(client.getPlayerPed().Position);
            for (int i=0;i<instances.Count;i++)
            {
                StreamedPed ped = (StreamedPed)instances[i];
                try
                {
                    if (ped.IsStreamedIn())
                    {
                        Vector2 projected = new Vector2();
                        if (client.getPlayerPed().Exists() && ped.gameReference.Exists())
                        {
                            projected = (Vector2)World.WorldToScreenProject(ped.gameReference.Position);
                        }
                        else return;
                        Vector3 peddelta = Vector3.Zero;
                        if (client.getPlayerPed().Exists() && ped.gameReference.Exists())
                        {
                            peddelta = ped.gameReference.Position - client.getPlayerPed().Position;
                        }
                        else return;
                        float distance = peddelta.Length();
                        //float distance_from_centerscreen = (projected - new Vector2(Game.Resolution.Width, Game.Resolution.Height)).Length();
                        int alpha = (int)Math.Round((255.0f * (distance / -DrawDistance + 1.0f)));
                        if (alpha > 255) alpha = 255;
                        if (alpha < 0) alpha = 0;
                        if (!ped.gameReference.Exists()) return;
                        if (projected.X < -120 || projected.X > Game.Resolution.Width || projected.Y < -50 || projected.Y > Game.Resolution.Height ||
                            (peddelta + Game.CurrentCamera.Direction).Length() < distance || (checkLOS && !Game.CurrentCamera.isSphereVisible(ped.gameReference.Position, 3.0f)))
                        {
                            if (!ped.gameReference.Exists()) return;
                            ped.nickDraw.destroy();
                            ped.healthDraw.destroy();
                            ped.healthDraw2.destroy();
                            if (ped.carHealthDraw != null)ped.carHealthDraw.destroy();
                            if (ped.carHealthDraw2 != null) ped.carHealthDraw2.destroy();
                            ped.chatDraw.destroy();
                            ped.nickDraw = null;
                            ped.healthDraw = null;
                            ped.healthDraw2 = null;
                            ped.carHealthDraw = null;
                            ped.carHealthDraw2 = null;
                            ped.chatDraw = null;
                        }
                        else
                        {
                            var rect = new System.Drawing.RectangleF(projected.X - 100, projected.Y - 50, 200, 30);

                            var rect2 = new System.Drawing.RectangleF(projected.X - 37, projected.Y - 22, 37 * 2, 11);
                            var rect22 = new System.Drawing.RectangleF(projected.X - 35, projected.Y - 20, (35.0f * 2.0f) * (ped.last_game_health < 0 ? 0 : ped.last_game_health / 100.0f), 7);

                            var rect3 = new System.Drawing.RectangleF(projected.X - 300, projected.Y - 10, 600, 130);

                            System.Drawing.RectangleF carrect1 = System.Drawing.RectangleF.Empty;
                            System.Drawing.RectangleF carrect2 = System.Drawing.RectangleF.Empty;
                            bool invehicle = ped.gameReference.isInVehicle();
                            if (invehicle)
                            {
                                carrect1 = new System.Drawing.RectangleF(projected.X - 37, projected.Y - 2, 37 * 2, 11);
                                carrect2 = new System.Drawing.RectangleF(projected.X - 35, projected.Y, (35.0f * 2.0f) * (ped.gameReference.CurrentVehicle.Health < 0 ? 0 : ped.gameReference.CurrentVehicle.Health / 1000.0f), 7);
                            }

                            var chaticonframe = new System.Drawing.RectangleF(projected.X - 30, projected.Y - 80, 60, 30);
                            if (ped.nickDraw == null)
                            {
                                ped.nickDraw = new ClientTextView(rect, TextAlignment.Center, ped.networkname, nickfont, System.Drawing.Color.FromArgb(alpha, 255, 255, 255));
                                ped.healthDraw = new ClientRectangleView(rect2, System.Drawing.Color.FromArgb(alpha, 0, 0, 0));
                                ped.healthDraw2 = new ClientRectangleView(rect22, System.Drawing.Color.FromArgb(alpha, 80, 80, 255));
                                ped.chatDraw = new ClientTextView(rect3, TextAlignment.Center, ped.CurrentChatMessage, nickfont, System.Drawing.Color.FromArgb(alpha, 255, 255, 255));
                                if (invehicle)
                                {
                                    ped.carHealthDraw = new ClientRectangleView(carrect1, System.Drawing.Color.FromArgb(alpha, 0, 0, 0));
                                    ped.carHealthDraw2 = new ClientRectangleView(carrect2, System.Drawing.Color.FromArgb(alpha, 255, 80, 80));
                                }
                            }
                            else
                            {
                                ped.nickDraw.Box = rect;
                                ped.nickDraw.color = System.Drawing.Color.FromArgb(alpha, 255, 255, 255);
                                ped.healthDraw.Box = rect2;
                                ped.healthDraw.color = System.Drawing.Color.FromArgb(alpha, 0, 0, 0);
                                ped.healthDraw2.Box = rect22;
                                ped.healthDraw2.color = System.Drawing.Color.FromArgb(alpha, 80, 80, 255);
                                ped.chatDraw.Box = rect3;
                                ped.chatDraw.text = ped.CurrentChatMessage;
                                ped.chatDraw.color = System.Drawing.Color.FromArgb(alpha, 255, 255, 255);
                                if (invehicle)
                                {
                                    if (ped.carHealthDraw != null)
                                    {
                                        ped.carHealthDraw.Box = carrect1;
                                        ped.carHealthDraw.color = System.Drawing.Color.FromArgb(alpha, 0, 0, 0);
                                        ped.carHealthDraw2.Box = carrect2;
                                        ped.carHealthDraw2.color = System.Drawing.Color.FromArgb(alpha, 255, 80, 80);
                                    }
                                    else
                                    {
                                        ped.carHealthDraw = new ClientRectangleView(carrect1, System.Drawing.Color.FromArgb(alpha, 0, 0, 0));
                                        ped.carHealthDraw2 = new ClientRectangleView(carrect2, System.Drawing.Color.FromArgb(alpha, 255, 80, 80));
                                    }

                                }
                                else
                                {
                                    if (ped.carHealthDraw != null) ped.carHealthDraw.destroy();
                                    if (ped.carHealthDraw2 != null) ped.carHealthDraw2.destroy();
                                    ped.carHealthDraw = null;
                                    ped.carHealthDraw2 = null;
                                }
                            }
                        }
                    }
                }
                catch
                { }
            }
        }

        public override void UpdateNormalTick()
        {
        }

        public override void UpdateSlow()
        {
            World.PedDensity = 0.0f;
            if (client.getPlayerPed().Exists())
            {
                var pedss = World.GetPeds(client.getPlayerPed().Position, 200.0f);
                foreach (Ped a in pedss)
                {
                    if (a.Exists() && a.isAlive && a != client.getPlayerPed() && instances.Count(ax => ((StreamedPed)ax).gameReference != null && ((StreamedPed)ax).gameReference.Exists() && ((StreamedPed)ax).gameReference == a) == 0)
                    {
                        if (a.Exists()) a.Delete();
                    }
                }
            }
        }
    }

    public class StreamedPed : StreamedObjectBase
    {
        public PedAnimationManager animator;
        public Blip blip;
        public ClientTextView chatDraw;
        public BlipColor color;
        public string CurrentChatMessage;
        public Ped gameReference;
        public bool hasNetworkName;
        public float heading;
        public ClientRectangleView healthDraw, healthDraw2, carHealthDraw, carHealthDraw2;
        public bool immortal;
        public int last_game_health;
        public string model, networkname;
        public ClientTextView nickDraw, iconDraw;
        public Vector3 position, direction;
        public uint vehicle_id;
        public Vector3 cameraDirection;

        public StreamedPed(PedStreamer streamer, string model, string networkname, Vector3 position, float heading, BlipColor color)
            : base(streamer)
        {
            this.position = position;
            this.heading = heading;
            this.networkname = networkname;
            this.model = model;
            direction = Vector3.Zero;
            hasNetworkName = false;
            vehicle_id = 0;
            CurrentChatMessage = "";
            animator = new PedAnimationManager(this);
        }

        public override Vector3 GetPosition()
        {
            return position;
        }

        public override bool IsStreamedIn()
        {
            return StreamedIn && gameReference != null && gameReference.Exists();
        }

        public override bool NeedRestream()
        {
            return last_game_health > 0 && (gameReference.isDead || !gameReference.isAlive || gameReference.Health == 0);
        }

        public override void StreamIn()
        {
            gameReference = World.CreatePed(model, position, RelationshipGroup.NetworkPlayer_01);

            blip = Blip.AddBlip(gameReference);
            blip.Color = color;
            blip.Display = BlipDisplay.MapOnly;
            blip.ShowOnlyWhenNear = false;
            blip.Icon = BlipIcon.Misc_Destination;
            blip.Name = networkname;

            gameReference.Heading = heading;

            var projected = (Vector2)World.WorldToScreenProject(position);
            var rand = new System.Random();

            Weapon weapon = Weapon.Rifle_M4;
            gameReference.Weapons.AssaultRifle_M4.Ammo = 999;
            gameReference.Weapons.AssaultRifle_M4.AmmoInClip = 999;
            gameReference.Weapons.Select(weapon);

            gameReference.SenseRange = 0;
            gameReference.Task.GuardCurrentPosition();
            gameReference.BlockGestures = true;
            gameReference.BlockPermanentEvents = true;
            gameReference.Task.AlwaysKeepTask = true;
            gameReference.CowerInsteadOfFleeing = true;
        }

        public override void StreamOut()
        {
            if (blip != null && blip.Exists()) blip.Delete();
            if (nickDraw != null) nickDraw.destroy();
            if (healthDraw != null) healthDraw.destroy();
            if (healthDraw2 != null) healthDraw2.destroy();
            if (carHealthDraw != null) carHealthDraw.destroy();
            if (carHealthDraw2 != null) carHealthDraw2.destroy();
            if (chatDraw != null) chatDraw.destroy();
            if (iconDraw != null) iconDraw.destroy();
            nickDraw = null;
            if (gameReference != null && gameReference.Exists()) gameReference.Delete();
            gameReference = null;
            hasNetworkName = false;
        }
    }
}