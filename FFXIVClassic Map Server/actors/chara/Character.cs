
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Map_Server.actors.chara.ai;
using System;

namespace FFXIVClassic_Map_Server.Actors
{
    class Character:Actor
    {
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int MAINHAND = 5;
        public const int OFFHAND = 6;
        public const int SPMAINHAND = 7;
        public const int SPOFFHAND = 8;
        public const int THROWING = 9;
        public const int PACK = 10;
        public const int POUCH = 11;
        public const int HEADGEAR = 12;
        public const int BODYGEAR = 13;
        public const int LEGSGEAR = 14;
        public const int HANDSGEAR = 15;
        public const int FEETGEAR = 16;
        public const int WAISTGEAR = 17;
        public const int NECKGEAR = 18;
        public const int L_EAR = 19;
        public const int R_EAR = 20;
        public const int R_WRIST = 21;
        public const int L_WRIST = 22;
        public const int R_RINGFINGER = 23;
        public const int L_RINGFINGER = 24;
        public const int R_INDEXFINGER = 25;
        public const int L_INDEXFINGER = 26;
        public const int UNKNOWN = 27;

        public bool isStatic = false;

        public bool isMovingToSpawn = false;

        public uint modelId;
        public uint[] appearanceIds = new uint[28];

        public uint animationId = 0;

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public uint currentActorIcon = 0;

        public Work work = new Work();
        public CharaWork charaWork = new CharaWork();

        public Group currentParty = null;
        public ContentGroup currentContentGroup = null;

        public DateTime lastAiUpdate;

        public AIContainer aiContainer;
        public StatusEffects statusEffects;

        public Character(uint actorID) : base(actorID)
        {            
            //Init timer array to "notimer"
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
                charaWork.statusShownTime[i] = 0xFFFFFFFF;

            this.statusEffects = new StatusEffects(this);
        }

        public SubPacket CreateAppearancePacket()
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelId, appearanceIds);
            return setappearance.BuildPacket(actorId);
        }

        public SubPacket CreateInitStatusPacket()
        {
            return (SetActorStatusAllPacket.BuildPacket(actorId, charaWork.status));                      
        }

        public SubPacket CreateSetActorIconPacket()
        {
            return SetActorIconPacket.BuildPacket(actorId, currentActorIcon);
        }

        public SubPacket CreateIdleAnimationPacket()
        {
            return SetActorSubStatPacket.BuildPacket(actorId, 0, 0, 0, 0, 0, 0, animationId);
        }

        public void SetQuestGraphic(Player player, int graphicNum)
        {
            player.QueuePacket(SetActorQuestGraphicPacket.BuildPacket(actorId, graphicNum));
        }

        public void SetCurrentContentGroup(ContentGroup group)
        {
            if (group != null)
                charaWork.currentContentGroup = group.GetTypeId();
            else
                charaWork.currentContentGroup = 0;

            currentContentGroup = group;

            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("charaWork/currentContentGroup", this);
            propPacketUtil.AddProperty("charaWork.currentContentGroup");            
            zone.BroadcastPacketsAroundActor(this, propPacketUtil.Done());

        }     
   
        public void PlayAnimation(uint animId, bool onlySelf = false)
        {            
            if (onlySelf)
            {
                if (this is Player)
                    ((Player)this).QueuePacket(PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
            }
            else
                zone.BroadcastPacketAroundActor(this, PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
        }

        public void PathTo(float x, float y, float z, float stepSize = 0.70f, int maxPath = 40, float polyRadius = 0.0f)
        {
            var pos = new Vector3(positionX, positionY, positionZ);
            var dest = new Vector3(x, y, z);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var path = utils.NavmeshUtils.GetPath(((Zone)GetZone()), pos, dest, stepSize, maxPath, polyRadius);

            if (path != null)
            {
                if (oldPositionX == 0.0f && oldPositionY == 0.0f && oldPositionZ == 0.0f)
                {
                    oldPositionX = positionX;
                    oldPositionY = positionY;
                    oldPositionZ = positionZ;
                }

                // todo: something went wrong
                if (path.Count == 0)
                {
                    positionX = oldPositionX;
                    positionY = oldPositionY;
                    positionZ = oldPositionZ;
                }

                positionUpdates = path;

                this.hasMoved = true;
                this.isAtSpawn = false;

                sw.Stop();
                ((Zone)zone).pathCalls++;
                ((Zone)zone).pathCallTime += sw.ElapsedMilliseconds;

                if (path.Count == 1)
                    Program.Log.Info($"mypos: {positionX} {positionY} {positionZ} | targetPos: {x} {y} {z} | step {stepSize} | maxPath {maxPath} | polyRadius {polyRadius}");

                Program.Log.Error("[{0}][{1}] Created {2} points in {3} milliseconds", actorId, actorName, path.Count, sw.ElapsedMilliseconds);
            }
        }

        public void FollowTarget(Actor target, float stepSize = 1.2f, int maxPath = 25, float radius = 0.0f)
        {
            var player = target as Player;

            if (player != null)
            {
                if (this.target != player)
                {
                    this.target = target;
                }
                this.moveState = player.moveState;
                this.moveSpeeds = player.moveSpeeds;

                PathTo(player.positionX, player.positionY, player.positionZ, stepSize, maxPath, radius);
            }
        }

        public void OnPath(Vector3 point)
        {
            if (positionUpdates != null && positionUpdates.Count > 0)
            {
                if (point == positionUpdates[positionUpdates.Count - 1])
                {
                    var myPos = new Vector3(positionX, positionY, positionZ);
                    //point = NavmeshUtils.GetPath((Zone)zone, myPos, point, 0.35f, 1, 0.000001f, true)?[0];
                }
            }
        }

        public override void Update(DateTime tick)
        {
            // todo: actual ai controllers
            // todo: mods to control different params instead of hardcode
            // todo: other ai helpers

            // time elapsed since last ai update

            this.aiContainer?.Update(tick);
    
            /*
            var diffTime = (tick - lastAiUpdate);

            if (this is Player)
            {
                // todo: handle player stuff here
            }
            else
            {
                // todo: handle mobs only?
                //if (this.isStatic)
                //    return;
                
                // todo: this too
                if (diffTime.Milliseconds >= 10)
                {
                    bool foundActor = false;

                    // leash back to spawn
                    if (!isMovingToSpawn && this.oldPositionX != 0.0f && this.oldPositionY != 0.0f && this.oldPositionZ != 0.0f)
                    {
                        //var spawnDistanceSq = Utils.DistanceSquared(positionX, positionY, positionZ, oldPositionX, oldPositionY, oldPositionZ);

                        // todo: actual spawn leash and modifiers read from table
                        // set a leash to path back to spawn even if have target
                        // (50 yalms)
                        if (Utils.DistanceSquared(positionX, positionY, positionZ, oldPositionX, oldPositionY, oldPositionZ) >= 3025)
                        {
                            this.isMovingToSpawn = true;
                            this.target = null;
                            this.lastMoveUpdate = this.lastMoveUpdate.AddSeconds(-5);
                            this.hasMoved = false;
                            ClearPositionUpdates();
                        }
                    }

                    // check if player
                    if (target != null && target is Player)
                    {
                        var player = target as Player;

                        // deaggro if zoning/logging
                        // todo: player.isZoning seems to be busted
                        if (player.playerSession.isUpdatesLocked)
                        {
                            target = null;
                            ClearPositionUpdates();
                        }
                    }

                    Player closestPlayer = null;
                    float closestPlayerDistanceSq = 1000.0f;

                    // dont bother checking for any in-range players if going back to spawn
                    if (!this.isMovingToSpawn)
                    {
                        foreach (var actor in zone.GetActorsAroundActor(this, 65))
                        {
                            if (actor is Player && actor != this)
                            {
                                var player = actor as Player;

                                // skip if zoning/logging
                                // todo: player.isZoning seems to be busted
                                if (player != null && player.playerSession.isUpdatesLocked)
                                    continue;

                                // find distance between self and target
                                var distanceSq = Utils.DistanceSquared(positionX, positionY, positionZ, player.positionX, player.positionY, player.positionZ);

                                int maxDistanceSq = player == target ? 900 : 100;

                                // check target isnt too far
                                // todo: create cone thing for IsFacing
                                if (distanceSq <= maxDistanceSq && distanceSq <= closestPlayerDistanceSq && (IsFacing(player) || true))
                                {
                                    closestPlayerDistanceSq = distanceSq;
                                    closestPlayer = player;
                                    foundActor = true;
                                }
                            }
                        }

                        // found a target
                        if (foundActor)
                        {
                            // make sure we're not already moving so we dont spam packets
                            if (!hasMoved)
                            {
                                // todo: include model size and mob specific distance checks
                                if (closestPlayerDistanceSq >= 9)
                                {
                                    FollowTarget(closestPlayer, 2.5f, 4);
                                }
                                // too close, spread out
                                else if (closestPlayerDistanceSq <= 0.85f)
                                {
                                    QueuePositionUpdate(target.FindRandomPointAroundActor(0.65f, 0.85f));
                                }

                                // we have a target, face them
                                if (target != null)
                                {
                                    LookAt(target);
                                }
                            }
                        }
                    }

                    // time elapsed since last move update
                    var diffMove = (tick - lastMoveUpdate);

                    // todo: modifier for DelayBeforeRoamToSpawn
                    // player disappeared
                    if (!foundActor && diffMove.Seconds >= 5)
                    {
                        // dont path if havent moved before
                        if (!hasMoved && oldPositionX != 0.0f && oldPositionY != 0.0f && oldPositionZ != 0.0f)
                        {
                            // check within spawn radius
                            this.isAtSpawn = Utils.DistanceSquared(positionX, positionY, positionZ, oldPositionX, oldPositionY, oldPositionZ) <= 625.0f;

                            // make sure we have no target
                            if (this.target == null)
                            {
                                // path back to spawn
                                if (!this.isAtSpawn)
                                {
                                    PathTo(oldPositionX, oldPositionY, oldPositionZ, 2.8f);
                                }
                                // within spawn range, find a random point
                                else if (diffMove.Seconds >= 15)
                                {
                                    // todo: polyRadius isnt euclidean distance..
                                    // pick a random point within 10 yalms of spawn
                                    PathTo(oldPositionX, oldPositionY, oldPositionZ, 2.5f, 7, 2.5f);

                                    // face destination
                                    if (positionUpdates.Count > 0)
                                    {
                                        var destinationPos = positionUpdates[positionUpdates.Count - 1];
                                        LookAt(destinationPos.X, destinationPos.Y);
                                    }
                                    if (this.isMovingToSpawn)
                                    {
                                        this.isMovingToSpawn = false;
                                        this.ResetMoveSpeedsToDefault();
                                        this.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD2);
                                    }
                                }
                            }
                        }
                        // todo: this is retarded. actually no it isnt, i didnt deaggro if out of range..
                        target = null;
                    }
                    // update last ai update time to now
                    lastAiUpdate = DateTime.Now;
                }
            }
            */
        }
        public virtual void Spawn(DateTime tick)
        {

        }

        public virtual void Die(DateTime tick)
        {

        }

        protected virtual void Despawn(DateTime tick)
        {

        }

        public bool IsDead()
        {
            return currentMainState == SetActorStatePacket.MAIN_STATE_DEAD || currentMainState == SetActorStatePacket.MAIN_STATE_DEAD2;
        }

        public bool IsAlive()
        {
            return !IsDead();
        }
    }

}
