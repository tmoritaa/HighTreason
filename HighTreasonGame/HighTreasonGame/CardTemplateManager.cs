﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame.CardTemplates;
using HighTreasonGame.ChoiceHandlers;

namespace HighTreasonGame
{
    public class CardTemplateManager
    {
        private static CardTemplateManager instance = null;
        public static CardTemplateManager Instance {
            get {
                if (instance == null)
                {
                    instance = new CardTemplateManager();
                }

                return instance;
            }
        }

        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }

        public List<CardTemplate> GetAllCards()
        {
            List<CardTemplate> cards = CardTemplates.Values.ToList();

            // TODO: only for now.
            while (CardTemplates.Keys.Count < 45)
            {
                CardTemplate tmp1 = new JohnAstleyCardTemplate();
                tmp1.SetName(tmp1.Name + CardTemplates.Keys.Count);
                CardTemplates.Add(tmp1.Name, tmp1);

                CardTemplate tmp2 = new PurelyConstitutionalCardTemplate();
                tmp2.SetName(tmp2.Name + CardTemplates.Keys.Count);
                CardTemplates.Add(tmp2.Name, tmp2);
            }

            return CardTemplates.Values.ToList();
        }

        public void Test()
        {
            CardTemplate template = CardTemplates["\"A Purely Constitutional Movement\""];
            BoardChoices choices = template.SelectionEventChoices[0](0, new TestChoiceHandler());
            template.SelectionEvents[0](0, choices);
        }

        private CardTemplateManager()
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates();
        }

        private void generateCardTemplates()
        {
            addCardTemplate(new JohnAstleyCardTemplate());
            addCardTemplate(new PurelyConstitutionalCardTemplate());
        }

        private void addCardTemplate(CardTemplate template)
        {
            CardTemplates.Add(template.Name, template);
        }
    }
}
