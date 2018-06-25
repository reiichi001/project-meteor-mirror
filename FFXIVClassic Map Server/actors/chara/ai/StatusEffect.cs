using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    enum StatusEffectId : uint
    {
        RageofHalone = 221021,
        
        Quick = 223001,
        Haste = 223002,
        Slow = 223003,
        Petrification = 223004,
        Paralysis = 223005,
        Silence = 223006,
        Blind = 223007,
        Mute = 223008,
        Slowcast = 223009,
        Glare = 223010,
        Poison = 223011,
        Transfixion = 223012,
        Pacification = 223013,
        Amnesia = 223014,
        Stun = 223015,
        Daze = 223016,
        ExposedFront = 223017,
        ExposedRight = 223018,
        ExposedRear = 223019,
        ExposedLeft = 223020,
        Incapacitation = 223021,
        Incapacitation2 = 223022,
        Incapacitation3 = 223023,
        Incapacitation4 = 223024,
        Incapacitation5 = 223025,
        Incapacitation6 = 223026,
        Incapacitation7 = 223027,
        Incapacitation8 = 223028,
        HPBoost = 223029,
        HPPenalty = 223030,
        MPBoost = 223031,
        MPPenalty = 223032,
        AttackUp = 223033,
        AttackDown = 223034,
        AccuracyUp = 223035,
        AccuracyDown = 223036,
        DefenseUp = 223037,
        DefenseDown = 223038,
        EvasionUp = 223039,
        EvasionDown = 223040,
        MagicPotencyUp = 223041,
        MagicPotencyDown = 223042,
        MagicAccuracyUp = 223043,
        MagicAccuracyDown = 223044,
        MagicDefenseUp = 223045,
        MagicDefenseDown = 223046,
        MagicResistanceUp = 223047,
        MagicResistanceDown = 223048,
        CombatFinesse = 223049,
        CombatHindrance = 223050,
        MagicFinesse = 223051,
        MagicHindrance = 223052,
        CombatResilience = 223053,
        CombatVulnerability = 223054,
        MagicVulnerability = 223055,
        MagicResilience = 223056,
        Inhibited = 223057,
        AegisBoon = 223058,
        Deflection = 223059,
        Outmaneuver = 223060,
        Provoked = 223061,
        Sentinel = 223062,
        Cover = 223063,
        Rampart = 223064,
        StillPrecision = 223065,
        Cadence = 223066,
        DiscerningEye = 223067,
        TemperedWill = 223068,
        Obsess = 223069,
        Ambidexterity = 223070,
        BattleCalm = 223071,
        MasterofArms = 223072,
        Taunted = 223073,
        Blindside = 223074,
        Featherfoot = 223075,
        PresenceofMind = 223076,
        CoeurlStep = 223077,
        EnduringMarch = 223078,
        MurderousIntent = 223079,
        Entrench = 223080,
        Bloodbath = 223081,
        Retaliation = 223082,
        Foresight = 223083,
        Defender = 223084,
        Rampage = 223085,           //old effect
        Enraged = 223086,
        Warmonger = 223087,
        Disorientx1 = 223088,
        Disorientx2 = 223089,
        Disorientx3 = 223090,
        KeenFlurry = 223091,
        ComradeinArms = 223092,
        Ferocity = 223093,
        Invigorate = 223094,
        LineofFire = 223095,
        Jump = 223096,
        Collusion = 223097,
        Diversion = 223098,
        SpeedSurge = 223099,
        LifeSurge = 223100,
        SpeedSap = 223101,
        LifeSap = 223102,
        Farshot = 223103,
        QuellingStrike = 223104,
        RagingStrike = 223105,      //old effect
        HawksEye = 223106,
        SubtleRelease = 223107,
        Decoy = 223108,             //Untraited
        Profundity = 223109,
        TranceChant = 223110,
        RoamingSoul = 223111,
        Purge = 223112,
        Spiritsong = 223113,
        Resonance = 223114,         //Old Resonance? Both have the same icons and description
        Soughspeak = 223115,
        PresenceofMind2 = 223116,
        SanguineRite = 223117,      //old effect
        PunishingBarbs = 223118,
        DarkSeal = 223119,          //old effect
        Emulate = 223120,
        ParadigmShift = 223121,
        ConcussiveBlowx1 = 223123,
        ConcussiveBlowx2 = 223124,
        ConcussiveBlowx3 = 223125,
        SkullSunder = 223126,
        Bloodletter = 223127,       //comboed effect
        Levinbolt = 223128,
        Protect = 223129,           //untraited protect
        Shell = 223130,             //old shell
        Reraise = 223131,
        ShockSpikes = 223132,
        Stoneskin = 223133,
        Scourge = 223134,
        Bio = 223135,
        Dia = 223136,
        Banish = 223137,
        StygianSpikes = 223138,
        ATKAbsorbed = 223139,
        DEFAbsorbed = 223140,
        ACCAbsorbed = 223141,
        EVAAbsorbed = 223142,
        AbsorbATK = 223143,
        AbsorbDEF = 223144,
        AbsorbACC = 223145,
        AbsorbEVA = 223146,
        SoulWard = 223147,
        Burn = 223148,
        Frost = 223149,
        Shock = 223150,
        Drown = 223151,
        Choke = 223152,
        Rasp = 223153,
        Flare = 223154,
        Freeze = 223155,
        Burst = 223156,
        Flood = 223157,
        Tornado = 223158,
        Quake = 223159,
        Berserk = 223160,
        RegimenofRuin = 223161,
        RegimenofTrauma = 223162,
        RegimenofDespair = 223163,
        RegimenofConstraint = 223164,
        Weakness = 223165,
        Scavenge = 223166,
        Fastcast = 223167,
        MidnightHowl = 223168,
        Outlast = 223169,
        Steadfast = 223170,
        DoubleNock = 223171,
        TripleNock = 223172,
        Covered = 223173,
        PerfectDodge = 223174,
        ExpertMining = 223175,
        ExpertLogging = 223176,
        ExpertHarvesting = 223177,
        ExpertFishing = 223178,
        ExpertSpearfishing = 223179,
        Regen = 223180,
        Refresh = 223181,
        Regain = 223182,
        TPBleed = 223183,
        Empowered = 223184,
        Imperiled = 223185,
        Adept = 223186,
        Inept = 223187,
        Quick2 = 223188,
        Quick3 = 223189,
        WristFlick = 223190,
        Glossolalia = 223191,
        SonorousBlast = 223192,
        Comradery = 223193,
        StrengthinNumbers = 223194,

        BrinkofDeath = 223197,
        CraftersGrace = 223198,
        GatherersGrace = 223199,
        Rebirth = 223200,
        Stealth = 223201,
        StealthII = 223202,
        StealthIII = 223203,
        StealthIV = 223204,
        Combo = 223205,
        GoringBlade = 223206,
        Berserk2 = 223207,              //new effect
        Rampage2 = 223208,              //new effect
        FistsofFire = 223209,
        FistsofEarth = 223210,
        FistsofWind = 223211,
        PowerSurgeI = 223212,
        PowerSurgeII = 223213,
        PowerSurgeIII = 223214,
        LifeSurgeI = 223215,
        LifeSurgeII = 223216,
        LifeSurgeIII = 223217,
        DreadSpike = 223218,
        BloodforBlood = 223219,
        Barrage = 223220,
        RagingStrike2 = 223221,

        Swiftsong = 223224,
        SacredPrism = 223225,
        ShroudofSaints = 223226,
        ClericStance = 223227,
        BlissfulMind = 223228,
        DarkSeal2 = 223229,         //new effect
        Resonance2 = 223230,
        Excruciate = 223231,
        Necrogenesis = 223232,
        Parsimony = 223233,
        SanguineRite2 = 223234,     //untraited effect
        Aero = 223235,
        Outmaneuver2 = 223236,
        Blindside2 = 223237,
        Decoy2 = 223238,            //Traited
        Protect2 = 223239,          //Traited
        SanguineRite3 = 223240,     //Traited
        Bloodletter2 = 223241,      //uncomboed effect
        FullyBlissfulMind = 223242,
        MagicEvasionDown = 223243,
        HundredFists = 223244,
        SpinningHeel = 223245,
        DivineVeil = 223248,
        HallowedGround = 223249,
        Vengeance = 223250,
        Antagonize = 223251,
        MightyStrikes = 223252,
        BattleVoice = 223253,
        BalladofMagi = 223254,
        PaeonofWar = 223255,
        MinuetofRigor = 223256,
        GoldLung = 223258,
        Goldbile = 223259,
        AurumVeil = 223260,
        AurumVeilII = 223261,
        Flare2 = 223262,
        Resting = 223263,
        DivineRegen = 223264,
        DefenseAndEvasionUp = 223265,
        MagicDefenseAndEvasionUp = 223266,
        AttackUp2 = 223267,
        MagicPotencyUp2 = 223268,
        DefenseAndEvasionDown = 223269,
        MagicDefenseAndEvasionDown = 223270,
        Poison2 = 223271,
        DeepBurn = 223272,
        LunarCurtain = 223273,
        DefenseUp2 = 223274,
        AttackDown2 = 223275,
        Sanction = 223992,
        IntactPodlingToting = 223993,
        RedRidingHooded = 223994,
        Medicated = 223998,
        WellFed = 223999,

        Sleep = 228001,
        Bind = 228011,
        Fixation = 228012,
        Bind2 = 228013,
        Heavy = 228021,
        Charm = 228031,
        Flee = 228041,
        Doom = 228051,
        SynthesisSupport = 230001,
        WoodyardAccess = 230002,
        SmithsForgeAccess = 230003,
        ArmorersForgeAccess = 230004,
        GemmaryAccess = 230005,
        TanneryAccess = 230006,
        ClothshopAccess = 230007,
        LaboratoryAccess = 230008,
        CookeryAccess = 230009,
        MinersSupport = 230010,
        BotanistsSupport = 230011,
        FishersSupport = 230012,
        GearChange = 230013,
        GearDamage = 230014,
        HeavyGearDamage = 230015,
        Lamed = 230016,
        Lamed2 = 230017,
        Lamed3 = 230018,
        Poison3 = 231002,
        Envenom = 231003,
        Berserk4 = 231004,
        GuardiansAspect = 253002,


        // custom effects here
        // status for having procs fall off
        EvadeProc = 253003,
        BlockProc = 253004,
        ParryProc = 253005,
        MissProc = 253006,
        EXPChain = 253007
    }

    [Flags]
    enum StatusEffectFlags : uint
    {
        None = 0,
        Silent = 1 << 0,                            // dont display effect loss message

        //Loss flags
        LoseOnDeath =                   1 << 1,     // effects removed on death
        LoseOnZoning =                  1 << 2,     // effects removed on zoning
        LoseOnEsuna =                   1 << 3,     // effects which can be removed with esuna (debuffs)
        LoseOnDispel =                  1 << 4,     // some buffs which player might be able to dispel from mob
        LoseOnLogout =                  1 << 5,     // effects removed on logging out
        LoseOnAttacking =               1 << 6,     // effects removed when owner attacks another entity
        LoseOnCastStart =               1 << 7,     // effects removed when owner starts casting
        LoseOnAggro =                   1 << 8,     // effects removed when owner gains enmity (swiftsong)

        //Activate flags
        ActivateOnCastStart =           1 << 9,     //Activates when a cast starts.
        ActivateOnCommandStart =        1 << 10,    //Activates when a command is used, before iterating over targets. Used for things like power surge, excruciate.
        ActivateOnCommandFinish =       1 << 11,    //Activates when the command is finished, after all targets have been iterated over. Used for things like Excruciate and Resonance falling off.
        ActivateOnPreactionTarget =     1 << 12,    //Activates after initial rates are calculated for an action against owner
        ActivateOnPreactionCaster =     1 << 13,    //Activates after initial rates are calculated for an action by owner
        ActivateOnDamageTaken =         1 << 14,
        ActivateOnHealed =              1 << 15,

        //Should these be rolled into DamageTaken?
        ActivateOnMiss =                1 << 16,    //Activates when owner misses
        ActivateOnEvade =               1 << 17,    //Activates when owner evades
        ActivateOnParry =               1 << 18,    //Activates when owner parries
        ActivateOnBlock =               1 << 19,    //Activates when owner evades
        ActivateOnHit =                 1 << 20,    //Activates when owner hits
        ActivateOnCrit =                1 << 21,    //Activates when owner crits

        //Prevent flags. Sleep/stun/petrify/etc combine these
        PreventSpell =                  1 << 22,    // effects which prevent using spells, such as silence
        PreventWeaponSkill =            1 << 23,    // effects which prevent using weaponskills, such as pacification
        PreventAbility =                1 << 24,    // effects which prevent using abilities, such as amnesia
        PreventAttack =                 1 << 25,    // effects which prevent basic attacks
        PreventMovement =               1 << 26,    // effects which prevent movement such as bind, still allows turning in place
        PreventTurn =                   1 << 27,    // effects which prevent turning, such as stun
        PreventUntarget =               1 << 28,    // effects which prevent changing targets, such as fixation

        Stealth =                       1 << 29,    // sneak/invis
        Stance =                        1 << 30,    // effects that do not have a timer
    }

    enum StatusEffectOverwrite : byte
    {
        None,
        Always,
        GreaterOrEqualTo,
        GreaterOnly,
    }

    class StatusEffect
    {
        // todo: probably use get;set;
        private Character owner;
        private Character source;
        private StatusEffectId id;
        private string name;        // name of this effect
        private DateTime startTime; // when was this effect added
        private DateTime endTime;   // when this status falls off
        private DateTime lastTick;  // when did this effect last tick
        private uint duration;    // how long should this effect last in seconds
        private uint tickMs;        // how often should this effect proc
        private double magnitude;   // a value specified by scripter which is guaranteed to be used by all effects
        private byte tier;          // same effect with higher tier overwrites this
        private double extra;       // optional value
        private StatusEffectFlags flags;         // death/erase/dispel etc
        private StatusEffectOverwrite overwrite; // how to handle adding an effect with same id (see StatusEfectOverwrite)
        private bool silent = false;             // do i send a message on losing effect 
        private bool hidden = false;
        public LuaScript script;

        HitEffect animationEffect;

        public StatusEffect(Character owner, uint id, double magnitude, uint tickMs, uint duration, byte tier = 0)
        {
            this.owner = owner;
            this.source = owner;
            this.id = (StatusEffectId)id;
            this.magnitude = magnitude;
            this.tickMs = tickMs;
            this.duration = duration;
            this.tier = tier;

            this.startTime = DateTime.Now;
            this.lastTick = startTime;
        }

        public StatusEffect(Character owner, StatusEffect effect)
        {
            this.owner = owner;
            this.source = owner;
            this.id = effect.id;
            this.magnitude = effect.magnitude;
            this.tickMs = effect.tickMs;
            this.duration = effect.duration;
            this.tier = effect.tier;
            this.startTime = effect.startTime;
            this.lastTick = effect.lastTick;

            this.name = effect.name;
            this.flags = effect.flags;
            this.overwrite = effect.overwrite;
            this.extra = effect.extra;
            this.script = effect.script;
        }

        public StatusEffect(uint id, string name, uint flags, uint overwrite, uint tickMs)
        {
            this.id = (StatusEffectId)id;
            this.name = name;
            this.flags = (StatusEffectFlags)flags;
            this.overwrite = (StatusEffectOverwrite)overwrite;
            this.tickMs = tickMs;
        }

        // return true when duration has elapsed
        public bool Update(DateTime tick)
        {
            if (tickMs != 0 && (tick - lastTick).TotalMilliseconds >= tickMs)
            {
                lastTick = tick;
                if (LuaEngine.CallLuaStatusEffectFunction(this.owner, this, "onTick", this.owner, this) > 0)
                    return true;
            }

            if (duration >= 0 && tick >= endTime)
            {
                return true;
            }
            return false;
        }

        public int CallLuaFunction(Character chara, string functionName, params object[] args)
        {

            DynValue res = new DynValue();

            return lua.LuaEngine.CallLuaStatusEffectFunction(chara, this, functionName, args);
            if (!script.Globals.Get(functionName).IsNil())
            {
                res = script.Call(script.Globals.Get(functionName), args);
                if (res != null)
                    return (int)res.Number;
            }

        }

        public Character GetOwner()
        {
            return owner;
        }

        public Character GetSource()
        {
            return source ?? owner;
        }

        public uint GetStatusEffectId()
        {
            return (uint)id;
        }

        public ushort GetStatusId()
        {
            return (ushort)(id - 200000);
        }

        public DateTime GetStartTime()
        {
            return startTime;
        }

        public DateTime GetEndTime()
        {
            return endTime;
        }

        public string GetName()
        {
            return name;
        }

        public uint GetDuration()
        {
            return duration;
        }

        public uint GetTickMs()
        {
            return tickMs;
        }

        public double GetMagnitude()
        {
            return magnitude;
        }

        public byte GetTier()
        {
            return tier;
        }

        public double GetExtra()
        {
            return extra;
        }

        public uint GetFlags()
        {
            return (uint)flags;
        }

        public byte GetOverwritable()
        {
            return (byte)overwrite;
        }

        public bool GetSilent()
        {
            return silent;
        }

        public bool GetHidden()
        {
            return hidden;
        }

        public void SetStartTime(DateTime time)
        {
            this.startTime = time;
            this.lastTick = time;
        }

        public void SetEndTime(DateTime time)
        {
            endTime = time;
        }

        //Refresh the status, updating the end time based on the duration of the status and broadcasts the new time
        public void RefreshTime()
        {
            endTime = DateTime.Now.AddSeconds(GetDuration());
            int index = Array.IndexOf(owner.charaWork.status, GetStatusId());

            if (index >= 0)
                owner.statusEffects.SetTimeAtIndex(index, (uint) Utils.UnixTimeStampUTC(endTime));
        }

        public void SetOwner(Character owner)
        {
            this.owner = owner;
        }

        public void SetSource(Character source)
        {
            this.source = source;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetMagnitude(double magnitude)
        {
            this.magnitude = magnitude;
        }

        public void SetDuration(uint duration)
        {
            this.duration = duration;
        }

        public void SetTickMs(uint tickMs)
        {
            this.tickMs = tickMs;
        }

        public void SetTier(byte tier)
        {
            this.tier = tier;
        }

        public void SetExtra(double val)
        {
            this.extra = val;
        }

        public void SetFlags(uint flags)
        {
            this.flags = (StatusEffectFlags)flags;
        }

        public void SetOverwritable(byte overwrite)
        {
            this.overwrite = (StatusEffectOverwrite)overwrite;
        }

        public void SetSilent(bool silent)
        {
            this.silent = silent;
        }

        public void SetHidden(bool hidden)
        {
            this.hidden = hidden;
        }

        public void SetAnimation(uint hitEffect)
        {
            animationEffect = (HitEffect)hitEffect;
        }

        public uint GetAnimation()
        {
            return (uint)animationEffect;
        }
    }
}
