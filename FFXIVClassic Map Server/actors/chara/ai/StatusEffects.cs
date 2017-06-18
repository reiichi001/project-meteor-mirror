using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.actors.area;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    enum StatusEffectId
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
        Rampage = 223085,
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
        RagingStrike = 223105,
        HawksEye = 223106,
        SubtleRelease = 223107,
        Decoy = 223108,
        Profundity = 223109,
        TranceChant = 223110,
        RoamingSoul = 223111,
        Purge = 223112,
        Spiritsong = 223113,
        Resonance = 223114,
        Soughspeak = 223115,
        PresenceofMind2 = 223116,
        SanguineRite = 223117,
        PunishingBarbs = 223118,
        DarkSeal = 223119,
        Emulate = 223120,
        ParadigmShift = 223121,
        ConcussiveBlowx1 = 223123,
        ConcussiveBlowx2 = 223124,
        ConcussiveBlowx3 = 223125,
        SkullSunder = 223126,
        Bloodletter = 223127,
        Levinbolt = 223128,
        Protect = 223129,
        Shell = 223130,
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
        Berserk2 = 223207,
        Rampage2 = 223208,
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
        DarkSeal2 = 223229,
        Resonance2 = 223230,
        Excruciate = 223231,
        Necrogenesis = 223232,
        Parsimony = 223233,
        SanguineRite2 = 223234,
        Aero = 223235,
        Outmaneuver2 = 223236,
        Blindside2 = 223237,
        Decoy2 = 223238,
        Protect2 = 223239,
        SanguineRite3 = 223240,
        Bloodletter2 = 223241,
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
    }

    [Flags]
    enum StatusEffectFlags
    {
        None = 0x00,
        Silent = 0x01,             // dont display effect loss message
        LoseOnDeath = 0x02,        // effects removed on death
        LoseOnZoning = 0x04,       // effects removed on zoning
        LoseOnEsuna = 0x08,        // effects which can be removed with esuna (debuffs)
        LoseOnDispel = 0x10,       // some buffs which player might be able to dispel from mob
        LoseOnLogout = 0x20,       // effects removed on logging out
        LoseOnAttacking = 0x40,    // effects removed when owner attacks another entity
        LoseOnCasting = 0x80,      // effects removed when owner starts casting
        LoseOnDamageTaken = 0x100, // effects removed when owner takes damage

        PreventAction = 0x200,     // effects which prevent actions such as sleep/paralyze/petrify
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
        private StatusEffectId id;
        private string name;        // name of this effect
        private DateTime startTime; // when was this effect added
        private DateTime lastTick;  // when did this effect last tick
        private uint durationMs;    // how long should this effect last in ms
        private uint tickMs;        // how often should this effect proc
        private int magnitude;      // a value specified by scripter which is guaranteed to be used by all effects
        private byte tier;          // same effect with higher tier overwrites this
        private Dictionary<string, UInt64> variables; // list of variables which belong to this effect, to be set/retrieved with GetVariable(key), SetVariable(key, val)
        private StatusEffectFlags flags;              // death/erase/dispel etc
        private StatusEffectOverwrite overwrite;      // how to handle adding an effect with same id (see StatusEfectOverwrite)

        public StatusEffect(Character owner, uint id, int magnitude, uint tickMs, uint durationMs, byte tier = 0)
        {
            this.owner = owner;
            this.id = (StatusEffectId)id;
            this.magnitude = magnitude;
            this.tickMs = tickMs;
            this.durationMs = durationMs;
            this.tier = tier;
            
            // todo: use tick instead of now?
            this.startTime = DateTime.Now;
            this.lastTick = startTime;

            // todo: set the effect name to be called by scripts or just lookup effects in db
            // name = WorldManager.GetEffectInfo(id).Name;
            // todo: check if can gain effect
            // todo: call effect's onGain
            // todo: broadcast effect gain packet
        }

        public StatusEffect(Character owner, StatusEffect effect)
        {
            this.owner = owner;
            this.id = effect.id;
            this.magnitude = effect.magnitude;
            this.tickMs = effect.tickMs;
            this.durationMs = effect.durationMs;
            this.tier = effect.tier;
            this.startTime = effect.startTime;
            this.lastTick = effect.lastTick;

            this.name = effect.name;
            this.flags = effect.flags;
            this.overwrite = effect.overwrite;
            this.variables = effect.variables;
        }

        // return true when duration has elapsed
        public bool Update(DateTime tick)
        {
            // todo: maybe not tick if already reached duration?
            if (tickMs != 0 && (lastTick - startTime).Milliseconds >= tickMs)
            {
                // todo: call effect's onTick
                // todo: maybe keep a global lua object instead of creating a new one each time we wanna call a script
                lastTick = tick;
            }
            // todo: handle infinite duration effects?
            if (durationMs != 0 && startTime.Millisecond + durationMs >= tick.Millisecond)
            {
                // todo: call effect's onLose
                // todo: broadcast effect lost packet
                return true;
            }
            return false;
        }

        public Character GetOwner()
        {
            return owner;
        }

        public uint GetEffectId()
        {
            return (uint)id;
        }

        public string GetName()
        {
            return name;
        }

        public uint GetDurationMs()
        {
            return durationMs;
        }

        public uint GetTickMs()
        {
            return tickMs;
        }

        public int GetMagnitude()
        {
            return magnitude;
        }

        public byte GetTier()
        {
            return tier;
        }

        public UInt64 GetVariable(string key)
        {
            return variables?[key] ?? 0;
        }

        public uint GetFlags()
        {
            return (uint)flags;
        }

        public byte GetOverwritable()
        {
            return (byte)overwrite;
        }

        public void SetOwner(Character owner)
        {
            this.owner = owner;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetDurationMs(uint durationMs)
        {
            this.durationMs = durationMs;
        }

        public void SetTickMs(uint tickMs)
        {
            this.tickMs = tickMs;
        }

        public void SetTier(byte tier)
        {
            this.tier = tier;
        }

        public void SetVariable(string key, UInt64 val)
        {
            if (variables != null)
            {
                variables[key] = val;
            }
            else
            {
                variables = new Dictionary<string, ulong>();
                variables[key] = val;
            }
        }

        public void SetFlags(uint flags)
        {
            this.flags = (StatusEffectFlags)flags;
        }

        public void SetOverwritable(byte overwrite)
        {
            this.overwrite = (StatusEffectOverwrite)overwrite;
        }
    }

    class StatusEffects
    {
        private Character owner;
        private List<StatusEffect> effects;

        public StatusEffects(Character owner)
        {
            this.owner = owner;
            this.effects = new List<StatusEffect>();
        }

        public void Update(DateTime tick)
        {
            // list of effects to remove
            var removeEffects = new List<StatusEffect>();
            foreach (var effect in effects)
            {
                // effect's update function returns true if effect has completed
                if (effect.Update(tick))
                    removeEffects.Add(effect);
            }

            // remove effects from this list
            foreach (var effect in removeEffects)
                effects.Remove(effect);
        }

        public bool AddStatusEffect(StatusEffect effect)
        {
            // todo: check flags/overwritable and add effect to list
            effects.Add(effect);
            return true;
        }

        public StatusEffect CopyEffect(StatusEffect effect)
        {
            var newEffect = new StatusEffect(this.owner, effect);
            newEffect.SetOwner(this.owner);

            return AddStatusEffect(newEffect) ? newEffect : null;
        }

        public bool RemoveStatusEffectsByFlags(uint flags)
        {
            // build list of effects to remove
            var removeEffects = new List<StatusEffect>();
            foreach (var effect in effects)
                if ((effect.GetFlags() & flags) > 0)
                    removeEffects.Add(effect);

            // remove effects from main list
            foreach (var effect in removeEffects)
                effects.Remove(effect);

            // removed an effect with one of these flags
            return removeEffects.Count > 0;
        }

        public StatusEffect GetStatusEffectById(uint id, uint tier = 0xFF)
        {
            foreach (var effect in effects)
            {
                if (effect.GetEffectId() == id && (tier != 0xFF ? effect.GetTier() == tier : true))
                    return effect;
            }
            return null;
        }

        public List<StatusEffect> GetStatusEffectsByFlag(uint flag)
        {
            var list = new List<StatusEffect>();
            foreach (var effect in effects)
            {
                if ((effect.GetFlags() & flag) > 0)
                {
                    list.Add(effect);
                }
            }
            return list;
        }

        public bool HasStatusEffectsByFlag(uint flag)
        {
            foreach (var effect in effects)
            {
                if ((effect.GetFlags() & flag) > 0)
                    return true;
            }
            return false;
        }
    }
}
