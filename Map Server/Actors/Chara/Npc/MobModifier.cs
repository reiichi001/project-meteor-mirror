/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

namespace Meteor.Map.actors.chara.npc
{
    enum MobModifier
    {
        None              = 0,
        SpawnLeash        = 1, // how far can i move before i deaggro target
        SightRange        = 2, // how close does target need to be for me to detect by sight
        SoundRange        = 3, // how close does target need to be for me to detect by sound
        BuffChance        = 4, 
        HealChance        = 5,
        SkillUseChance    = 6,
        LinkRadius        = 7,
        MagicDelay        = 8,
        SpecialDelay      = 9,
        ExpBonus          = 10, // 
        IgnoreSpawnLeash  = 11, // pursue target forever
        DrawIn            = 12, // do i suck people in around me
        HpScale           = 13, //
        Assist            = 14, // gotta call the bois 
        NoMove            = 15, // cant move
        ShareTarget       = 16, // use this actor's id as target id
        AttackScript      = 17, // call my script's onAttack whenever i attack
        DefendScript      = 18, // call my script's onDamageTaken whenever i take damage
        SpellScript       = 19, // call my script's onSpellCast whenever i finish casting
        WeaponSkillScript = 20, // call my script's onWeaponSkill whenever i finish using a weaponskill
        AbilityScript     = 21, // call my script's onAbility whenever i finish using an ability
        CallForHelp       = 22, // actor with this id outside of target's party with this can attack me
        FreeForAll        = 23, // any actor can attack me
        Roams             = 24, // Do I walk around?
        RoamDelay         = 25, // What is the delay between roam ticks
        Linked            = 26, // Did I get aggroed via linking? 
        LinkCount         = 27  // How many BattleNPCs got linked with me
    }
}
