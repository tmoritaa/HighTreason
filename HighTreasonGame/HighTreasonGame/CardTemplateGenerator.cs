using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;
using HighTreasonGame.CardTemplates;

namespace HighTreasonGame
{
    public class CardTemplateGenerator
    {
        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }

        // TODO: only necessary since generating cards. Should be deleted once enough cards are implemented.
        public JObject InfoRoot;

        public CardTemplateGenerator(string cardInfoJson)
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates(cardInfoJson);
        }

        public List<CardTemplate> GetAllCardTemplates()
        {
            List<CardTemplate> cards = CardTemplates.Values.ToList();

            // TODO: only for now until we have enough cards.
            while (CardTemplates.Keys.Count < 45)
            {
                CardTemplate tmp1 = new JohnAstleyCardTemplate();
                tmp1.Init(InfoRoot);
                tmp1.SetName(tmp1.Name + CardTemplates.Keys.Count.ToString().PadLeft(2, '0'));
                CardTemplates.Add(tmp1.Name, tmp1);

                CardTemplate tmp2 = new PurelyConstitutionalCardTemplate();
                tmp2.Init(InfoRoot);
                tmp2.SetName(tmp2.Name + CardTemplates.Keys.Count.ToString().PadLeft(2, '0'));
                CardTemplates.Add(tmp2.Name, tmp2);
            }

            return CardTemplates.Values.ToList();
        }

        private void generateCardTemplates(string cardInfoJson)
        {
            var cardTemplateTypes =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(CardTemplateAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<CardTemplateAttribute>() };

            InfoRoot = JObject.Parse(cardInfoJson);

            foreach (var type in cardTemplateTypes)
            {
                CardTemplate template = (CardTemplate)Activator.CreateInstance(type.Type);
                template.Init(InfoRoot);
                CardTemplates.Add(template.Name, template);
            }
        }
    }
}
