﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using POESKillTree.Model.Items.StatTranslation;
using POESKillTree.Utils;

namespace UnitTests.Model.Items.StatTranslations
{
    [TestClass]
    public class StatTranslatorTest
    {
        private Task<StatTranslator> _translator;

        [TestInitialize]
        public void TestInitialize()
        {
            _translator = InitializeAsync();
        }

        private static async Task<StatTranslator> InitializeAsync()
        {
            var translations = await new RePoELoader(new HttpClient(), false)
                .LoadAsync<List<JsonStatTranslation>>("stat_translations");
            return new StatTranslator(translations);
        }

        [TestMethod]
        public async Task GetTranslations_VentorsGamble()
        {
            IReadOnlyDictionary<string, int> statDict = new Dictionary<string, int>
            {
                { "base_maximum_life", 30 },
                { "base_item_found_quantity_+%", 0 },
                { "base_item_found_rarity_+%", -15 },
                { "base_fire_damage_resistance_%", 33 },
                { "base_cold_damage_resistance_%", 0 },
                { "base_lightning_damage_resistance_%", -1 },
            };
            string[] expected =
            {
                "+30 to maximum Life",
                null,
                "15% reduced Rarity of Items found",
                "+33% to Fire Resistance",
                null,
                "-1% to Lightning Resistance",
            };
            var actual = (await _translator).GetTranslations(statDict);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public async Task GetTranslations_DoomfletchsPrism()
        {
            IReadOnlyDictionary<string, int> statDict = new Dictionary<string, int>
            {
                { "local_minimum_added_physical_damage", 10 },
                { "local_maximum_added_physical_damage", 20 },
                { "local_attack_speed_+%", 13 },
                { "local_critical_strike_chance_+%", 30 },
                { "weapon_physical_damage_%_to_add_as_each_element", 110 },
                { "weapon_physical_damage_%_to_add_as_random_element", 0 }, // -110 + 110
            };
            string[] expected =
            {
                "Adds 10 to 20 Physical Damage",
                "13% increased Attack Speed",
                "30% increased Critical Strike Chance",
                "Gain 110% of Bow Physical Damage as Extra Damage of each Element",
                null,
            };
            var actual = (await _translator).GetTranslations(statDict);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

    }
}